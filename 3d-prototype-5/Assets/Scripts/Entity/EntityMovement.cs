using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityMovement : MonoBehaviour
{

    public NavMeshAgent agent;
    public Vector3 targetPos;
    public Vector3 randomDirection;
    public bool isMoving;
    public bool isAiming;
    public bool blocked;

    [Header("Move Speed")]
    public MoveType moveType;
    public float walkSpeed;
    public float jogSpeed;
    public float sprintSpeed;
    public float panicSpeed;
    public float speedMultiplier;
    public float fleeDistance = 15f;
    public int strafeSide;

    private Entity entity;
    private EntityBrain brain;
    void Awake()
    {

    }
    void Start()
    {
        entity = GetComponent<Entity>();
        brain = GetComponent<EntityBrain>();

        speedMultiplier = entity.speed / 250.0f;
        speedMultiplier = Random.Range(1f - speedMultiplier, 1.5f + speedMultiplier);
        walkSpeed *= speedMultiplier;
        jogSpeed *= speedMultiplier;
        sprintSpeed *= speedMultiplier;
        panicSpeed *= speedMultiplier;

        EntityManager.moveTick += Move;
        entity.body.Play("RandomFloat", Random.Range(0f, 2f));
        randomDirection = Helper.RandomVectorInRadius(6f);
        ChangeSpeed((int)moveType);
    }

    public void OnDeath()
    {
        EntityManager.moveTick -= Move;
    }

    void Move()
    {
        UpdateAnimations();
        if (isAiming) // Move to Position while aiming at target
        {
            agent.SetDestination(targetPos);

            if (brain.target != null && brain.facingTarget != null)
                RotateToTarget((brain.facingTarget.position - transform.position).normalized);
            else
                RotateToTarget(randomDirection);
        }

        if (isMoving) agent.SetDestination(targetPos);

    }

    public void StopMovement()
    {
        isMoving = false;
        agent.isStopped = true;
    }

    public void ResumeMovement()
    {
        isMoving = true;
        agent.isStopped = false;
    }

    public void ChangeSpeed(int type)
    {

        moveType = (MoveType)type;
        switch (type)
        {
            case 0: // Walking
                agent.speed = walkSpeed;
                break;
            case 1: // Jogging
                agent.speed = jogSpeed;
                break;
            case 2: // Sprinting
                agent.speed = sprintSpeed;
                ToggleAiming(false);
                break;
        }
    }

    public void MoveTo(Vector3 pos)
    {
        isMoving = true;
        NavMeshHit hit;
        // Sample a point on the current path within 10 units
        if (NavMesh.SamplePosition(pos, out hit, 25f, NavMesh.AllAreas))
            targetPos = hit.position;
        else targetPos = pos;
        ResumeMovement();

    }

    public Vector3 EnemyCohesion()
    {
        var enemies = entity.brain.visibleEnemies;
        if (enemies == null || enemies.Count == 0)
            return transform.position;

        Vector3 center = Vector3.zero;
        int count = 0;

        for (int i = 0; i < enemies.Count; i++)
        {
            var e = enemies[i];
            if (e == null) continue;
            center += e.transform.position;
            count++;
        }

        return (count > 0) ? center / count : transform.position;
    }

    public Vector3 AllyCohesion()
    {
        var allies = entity.brain.nearbyAllies;
        if (allies == null || allies.Count == 0)
            return Vector3.zero;

        Vector3 center = Vector3.zero;
        int count = 0;

        for (int i = 0; i < allies.Count; i++)
        {
            var a = allies[i];
            if (a == null) continue;
            center += a.transform.position;
            count++;
        }

        return (count > 0) ? center / count : transform.position;
    }

    public void Flee()
    {
        Vector3 pos = transform.position;
        Vector3 enemyCenter = EnemyCohesion();
        bool toRight = false;
        Vector3 toL = Vector3.zero;
        Vector3 toR = Vector3.zero;
        // Base flee direction: from enemy center to us (XZ)
        Vector3 eDir = pos - enemyCenter;
        eDir.y = 0f;
        if (eDir.sqrMagnitude < 1e-6f) eDir = transform.forward;
        eDir.Normalize();

        Vector3 finalDir = eDir;

        // Check a short wall probe in our flee direction
        Vector3 origin = pos + Vector3.up * 1f;
        float wallProbe = 6f;                // how far ahead to look for a wall
        int wallMask = (1 << 12) | (1 << 16);              // your wall layer

        if (Physics.Raycast(origin, eDir, out RaycastHit hit, wallProbe, wallMask))
        {
            if (hit.collider.CompareTag("Safe Zone"))
            {
                MoveTo(hit.transform.position);
                return;
            }
            // Circle radius = distance from enemy center to wall-hit point
            float radius = Vector3.Distance(enemyCenter, hit.point) / 3f;

            // Perpendiculars to enemy->us direction on XZ
            Vector3 perpL = Vector3.Cross(Vector3.up, eDir); // left
            Vector3 perpR = -perpL;                          // right

            // Tangent candidates on the circle centered at enemy center
            Vector3 pL = enemyCenter + perpL * radius;
            Vector3 pR = enemyCenter + perpR * radius;

            // Pick the closer tangent point to our current position
            toL = pL - pos; toL.y = 0f;
            toR = pR - pos; toR.y = 0f;

            if (toL.sqrMagnitude >= toR.sqrMagnitude)
            {
                finalDir = (eDir + toL).normalized;
            }
            else
            {
                finalDir = (eDir + toR).normalized;
                toRight = true;
            }

            // Debug
            Debug.DrawLine(enemyCenter, pL, Color.red);
            Debug.DrawLine(enemyCenter, pR, Color.blue);
            Debug.DrawRay(pos, finalDir, Color.cyan);
        }

        float fleeDistance = 10f;
        Vector3 desired = pos + finalDir * fleeDistance;

        Vector3 sampled;
        if (TrySampleOnNavMesh(desired, 2f, out sampled))
        {
            MoveTo(sampled);
        }
        else
        {
            if (toRight) finalDir = (eDir + toL).normalized;
            else finalDir = (eDir + toR).normalized;
            desired = pos + finalDir * fleeDistance;
            if (TrySampleOnNavMesh(desired, 2f, out sampled))
                MoveTo(sampled);
        }
    }

    public void StrafeForLineOfSight(string tag, float strafeAmount = 4f)
    {
        if (brain.target == null) return;

        // Eye origin slightly raised to avoid hitting our own collider
        Vector3 origin = transform.position;

        // Direction to target (planar)
        Vector3 dir = brain.target.position - origin;
        dir.y = 0f; dir.Normalize();

        // First hit decides if LOS is blocked
        blocked = false;
        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, 10f))
        {
            if (hit.collider.CompareTag(tag) && hit.transform != transform)
                blocked = true;
        }

        if (!blocked) { strafeSide = 0; return; }

        // Need to strafe. If we don't have a chosen side yet, pick one.
        if (strafeSide == 0)
        {
            // Perpendiculars on XZ plane
            Vector3 left = Vector3.Cross(Vector3.up, dir);   // 90° left of dir
            Vector3 right = -left;

            // Sample short side probes; prefer the side with more clearance
            float sideCheck = 1.0f;
            float rightClear;
            float leftClear;
            if (Physics.Raycast(origin, left, out hit, sideCheck))
                leftClear = hit.distance;
            else
                leftClear = (origin + left).magnitude;

            if (Physics.Raycast(origin, right, out hit, sideCheck))
                rightClear = hit.distance;
            else
                rightClear = (origin + right).magnitude;

            strafeSide = (leftClear >= rightClear) ? -1 : 1; // -1 = left, 1 = right
        }

        // Move along the chosen perpendicular
        Vector3 perp = (strafeSide == -1)
            ? Vector3.Cross(Vector3.up, dir)   // left
            : Vector3.Cross(dir, Vector3.up);  // right

        perp.y = 0f;
        perp.Normalize();

        Vector3 strafeTarget = transform.position + perp * strafeAmount;

        // Drive movement (use your existing movement call)
        MoveTo(strafeTarget);
    }
    /// <summary>
    /// Returns true if a nearby NavMesh position was found for 'target'.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="sampleRadius"></param>
    /// <param name="sampled"></param>
    /// <returns></returns>
    private bool TrySampleOnNavMesh(Vector3 target, float sampleRadius, out Vector3 sampled)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(target, out hit, sampleRadius, NavMesh.AllAreas))
        {
            sampled = hit.position;
            return true;
        }
        sampled = Vector3.zero;
        return false;
    }
    public void Orbit(Vector3 target, float radius = 6f)
    {
        Vector3 offset;

        // If there are visible enemies, bias orbit to be perpendicular (90°) to the direction facing cohesion
        if (brain != null && entity.brain.visibleEnemies != null && entity.brain.visibleEnemies.Count > 0)
        {
            Vector3 cohesion = EnemyCohesion();                 // world position of enemy center
            Vector3 toCoh = cohesion - transform.position;      // face the cohesion
            toCoh.y = 0f;
            if (toCoh.sqrMagnitude < 1e-6f) toCoh = transform.forward;
            toCoh.Normalize();

            // 90° perpendicular on XZ (left/right of the vector facing cohesion)
            Vector3 perp = Vector3.Cross(Vector3.up, toCoh).normalized;
            if (Random.value < 0.5f) perp = -perp;              // randomly choose left or right

            // Small jitter so positions vary around that perpendicular, not a perfect line
            const float angleJitter = 25f;                      // degrees
            Quaternion jitter = Quaternion.AngleAxis(Random.Range(-angleJitter, angleJitter), Vector3.up);
            Vector3 dir = (jitter * perp).normalized;

            // Pick distance within radius
            float dist = Random.Range(radius * 0.5f, radius);
            offset = dir * dist;
        }
        else
        {
            // Default orbit when no enemies are visible
            offset = Helper.RandomVectorInRadius(radius);
        }

        MoveTo(target + offset);
    }

    public void Surrounding()
    {
        // Config
        float innerDangerRadius = 17.5f;   // inside this is "danger"
        float outerRoamRadius = 20f;   // don't roam beyond this
        float minSeparation = 6f;      // at least this far from current position
        int maxAttempts = 16;      // tries to find a valid NavMesh point
        float sampleRadius = 4f;      // NavMesh.SamplePosition radius
        float inwardTolerance = 0.2f;    // how strictly we reject inward moves (0 = strict)

        Vector3 center = (brain != null && brain.target != null)
            ? brain.target.position
            : transform.position;

        if (outerRoamRadius <= innerDangerRadius)
            outerRoamRadius = innerDangerRadius + 1f;

        Vector3 pos = transform.position;
        Vector3 radial = pos - center; radial.y = 0f;
        float currentR = radial.magnitude;
        Vector3 inward = (center - pos); inward.y = 0f; // points toward humans

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Uniform random in annulus
            float r = Mathf.Sqrt(Random.Range(innerDangerRadius * innerDangerRadius,
                                               outerRoamRadius * outerRoamRadius));
            float ang = Random.Range(0f, 360f) * Mathf.Deg2Rad;

            Vector3 candidate = center + new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang)) * r;

            // Must be at least minSeparation from current position
            if ((candidate - pos).sqrMagnitude < (minSeparation * minSeparation))
                continue;

            // Do NOT move inward toward the humans:
            // 1) candidate radius must not be smaller than our current radius
            if (r + 1e-3f < currentR) continue;

            // 2) and the move direction shouldn't point inward
            Vector3 move = candidate - pos; move.y = 0f;
            if (move.sqrMagnitude < 1e-6f) continue;
            if (Vector3.Dot(move.normalized, inward.normalized) > inwardTolerance)
                continue;

            if (TrySampleOnNavMesh(candidate, sampleRadius, out Vector3 sampled))
            {
                MoveTo(sampled);
                return;
            }
        }

        // Fallback: take a tangential step (orbit) that preserves distance
        if (currentR < 1e-3f) currentR = innerDangerRadius; // degenerate case
        Vector3 radialDir = (radial.sqrMagnitude > 1e-6f) ? radial.normalized : transform.forward;
        Vector3 tangent = Vector3.Cross(Vector3.up, radialDir);          // left
        if (Random.value < 0.5f) tangent = -tangent;                        // or right

        float step = Mathf.Clamp(minSeparation * 1.5f, 1.5f, 4f);
        Vector3 fallback = center + radialDir * Mathf.Clamp(currentR, innerDangerRadius, outerRoamRadius)
                                 + tangent * step;

        if (TrySampleOnNavMesh(fallback, sampleRadius, out Vector3 fallbackSample))
            MoveTo(fallbackSample);
    }




    public bool IsPathSafe(Vector3 pos, string tag)
    {
        Vector3 thisPos = transform.position;
        thisPos.y += 1f;
        Vector3 direction = pos - thisPos;
        if (Physics.Raycast(thisPos, direction, out RaycastHit hit, direction.magnitude, (1 << 6)))
        {
            if (hit.collider.CompareTag(tag))
            {
                Debug.Log(hit.collider.name);
                return false;
            }

        }
        return true;
    }

    public bool HasReached(float radius = .25f)
    {
        return agent.remainingDistance <= agent.stoppingDistance + radius;
    }

    public bool HasReached(Transform transform, float radius = .25f)
    {
        return HasReached(transform.position, radius);
    }

    public bool HasReached(Vector3 pos, float radius = .25f)
    {
        float dist = Vector3.Distance(pos, transform.position);

        return dist <= agent.stoppingDistance + radius;
    }

    public bool HasReachedPosition(float radius = 2f)
    {
        float dist = Vector3.Distance(targetPos, transform.position);

        return dist <= agent.stoppingDistance + radius;
    }

    public void RotateToTarget()
    {
        if (brain.target == null || brain.facingTarget == null) return;

        Vector3 direction = (brain.facingTarget.position - transform.position).normalized;
        direction.y = 0f; // Ignore vertical difference

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Adjust speed
        }
    }

    public void RotateToTarget(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Adjust speed
        }
    }

    /// <summary>
    /// Aiming is also strafing
    /// </summary>
    /// <param name="isAiming"></param>
    public void ToggleAiming(bool isAiming)
    {
        // If Aiming is true, agent can't update rotation
        agent.updateRotation = !isAiming;

        this.isAiming = isAiming;
    }

    /// <summary>
    /// A From B
    /// </summary>
    /// <returns></returns>
    public Vector3 GetForwardVelocity()
    {
        // A - B
        Vector3 fwd = agent.velocity;
        fwd.y = 0f;
        return fwd;
    }

    void UpdateAnimations()
    {
        entity.body.Play("IsMoving", isMoving);
        if (!isMoving) return;

        Vector3 vel = GetForwardVelocity().normalized;

        float forwardAmount = Vector3.Dot(vel, transform.forward);
        float strafeAmount = Vector3.Dot(vel, transform.right);

        entity.body.Play("Forward", forwardAmount);
        entity.body.Play("Strafe", strafeAmount);
        entity.body.Play("Running", (int)moveType);
    }
}

public enum MoveType
{
    Walking = 0,
    Jogging = 1,
    Sprinting = 2,
}
