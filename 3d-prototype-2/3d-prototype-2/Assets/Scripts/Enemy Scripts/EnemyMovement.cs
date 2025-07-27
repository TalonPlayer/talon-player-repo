using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Components")]
    public NavMeshAgent navMeshAgent;

    [Header("Move States")]
    public bool isStationery;
    public bool isFacing;
    public bool strafes;
    protected bool isStrafing;
    protected bool isPatrolling = false;
    protected bool isIdle = false;

    [Header("Aggro Info")]
    public float viewAngle = 25f;
    public float turnSpeed = 5f;
    public float stunTime = 1f;
    public float threshold = .1f;
    public float retreatRange;
    public float strafeRange = 5f;
    [Header("Patrol Info")]
    public float walkSpeed;
    public List<Waypoint> waypoints;
    public int patrolIndex = -1;
    protected Waypoint currentwaypoint;

    protected float moveSpeed;
    protected float stoppingDistance;
    protected float dir;
    protected int movDir = 0;
    protected Vector3 previousPosition;
    protected Coroutine strafeRoutine;
    protected Coroutine positionRoutine;
    protected Coroutine idleRoutine;
    public Coroutine kbRoutine;
    protected Enemy enemy;
    public void SetEnemy(Enemy e) => enemy = e;

    protected virtual void Start()
    {
        stoppingDistance = navMeshAgent.stoppingDistance;
        moveSpeed = navMeshAgent.speed;
        navMeshAgent.stoppingDistance = 0f;

        Invoke(nameof(LateInit), .25f);
    }

    public virtual void Init(bool _isStationery, List<Waypoint> _patrolPoints)
    {
        isStationery = _isStationery;
        waypoints = _patrolPoints;
    }

    public virtual void LateInit()
    {
        if (isStationery)
        {
            isIdle = true;
            StopMovement();
        }
        else
        {
            isPatrolling = true;
            navMeshAgent.speed = walkSpeed;
            idleRoutine = StartCoroutine(IdleRoutine(.5f));
        }

        enemy.body.animator.SetFloat("Offset", Random.Range(0, 1f));
    }

    void Update()
    {
        if (isPatrolling && currentwaypoint != null)
        {
            PatrolState();
        }
        if (!enemy.isAggro ||
            enemy.isKnocked ||
            !enemy.isAlive ||
            enemy.combat.isAttacking ||
            !navMeshAgent.isOnNavMesh) return;

        NavMeshPath path = new NavMeshPath();
        navMeshAgent.CalculatePath(navMeshAgent.destination, path);
        Quaternion targetRotation;
        if (path.corners.Length == 2)
        {
            navMeshAgent.updateRotation = false;

            // Constantly look at the target
            Vector3 lookDirection = enemy.target.position - enemy.head.position;
            lookDirection.y = 0f;
            if (lookDirection != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(lookDirection);
                enemy.head.rotation = Quaternion.Slerp(enemy.head.rotation, targetRotation, Time.deltaTime * turnSpeed);
            }
            isFacing = Vector3.Angle(enemy.head.forward, lookDirection.normalized) <= viewAngle;
        }
        else
        {
            //navMeshAgent.updateRotation = true;
            targetRotation = Quaternion.LookRotation(navMeshAgent.velocity.normalized);
            enemy.head.rotation = Quaternion.Slerp(enemy.head.rotation, targetRotation, Time.deltaTime * 5f);
        }

        MoveState();
    }

    protected virtual void MoveState()
    {
        // Retreat
        if (enemy.distance < retreatRange)
        {
            isStrafing = false;
            if (strafeRoutine != null)
                StopCoroutine(strafeRoutine);

            Vector3 oppositeDirection = (transform.position - enemy.target.position).normalized;
            Vector3 retreatTarget = transform.position + oppositeDirection * retreatRange;

            SamplePosition(retreatTarget, oppositeDirection);
            navMeshAgent.speed = moveSpeed;
            
        }
        // Strafing Zone
        else if (enemy.distance <= stoppingDistance + 2f && !enemy.combat.canAttack && strafes)
        {
            if (!isStrafing)
            {
                dir = Random.Range(0, 2) == 0 ? -1 : 1;
                strafeRoutine = StartCoroutine(StrafeRoutine(5f));
                isStrafing = true;
                navMeshAgent.speed = moveSpeed / 3;
            }

            Vector3 strafeDirection = enemy.head.right * dir;
            Vector3 strafeTarget = transform.position + strafeDirection * strafeRange;

            SamplePosition(strafeTarget, strafeDirection);
        }
        // Chase Player
        else
        {
            isStrafing = false;
            if (strafeRoutine != null)
                StopCoroutine(strafeRoutine);
                
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.stoppingDistance = stoppingDistance;
            navMeshAgent.destination = enemy.target.position;
        }
    }
    protected void SamplePosition(Vector3 newTarget, Vector3 direction)
    {
        NavMeshHit hit;

        bool foundPosition = NavMesh.SamplePosition(newTarget, out hit, 5f, NavMesh.AllAreas);


        if (!foundPosition)
        {
            Vector3 fallbackTarget = transform.position + direction * (retreatRange * 0.5f);

            if (NavMesh.Raycast(transform.position, fallbackTarget, out hit, NavMesh.AllAreas))
            {
                // If ray hits an obstacle, use the hit.position (just before obstacle)
                newTarget = hit.position;
            }
            else
            {
                // Otherwise, use the fallback target directly
                newTarget = fallbackTarget;
            }
            NavMesh.SamplePosition(newTarget, out hit, 2f, NavMesh.AllAreas);
        }

        navMeshAgent.stoppingDistance = 0f;
        navMeshAgent.destination = hit.position;
    }
    public void PatrolState()
    {
        if (!navMeshAgent.isOnNavMesh) return;

        if (navMeshAgent.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion rotation = Quaternion.LookRotation(navMeshAgent.velocity.normalized);
            enemy.head.rotation = Quaternion.Slerp(enemy.head.rotation, rotation, Time.deltaTime * 5f);
        }

        if (navMeshAgent.remainingDistance <= currentwaypoint.waypointDistance)
        {
            if (currentwaypoint.pause)
            {
                // currentwaypoint.RandomIdleAnimation(enemy.body.animator, enemy.body.idleAnimations);
                idleRoutine = StartCoroutine(IdleRoutine(currentwaypoint.idleTime));
            }
            else
            {
                Patrol();
            }
        }
    }
    public void Patrol()
    {
        if (waypoints.Count == 0) return;
        isPatrolling = true;
        patrolIndex++;

        if (patrolIndex >= waypoints.Count)
            patrolIndex = 0;

        currentwaypoint = waypoints[patrolIndex];
        navMeshAgent.destination = currentwaypoint.transform.position;
        enemy.body.IntAnim("MovingDirection", 1);
    }
    public IEnumerator IdleRoutine(float idleTime)
    {
        isPatrolling = false;
        StopMovement();
        enemy.body.IntAnim("MovingDirection", 0);
        yield return new WaitForSeconds(idleTime);
        navMeshAgent.isStopped = false;
        Patrol();
    }
    public virtual void OnDeath()
    {
        navMeshAgent.enabled = false;
    }

    public void ResetAfterKnockback()
    {
        if (kbRoutine != null)
        {
            StopCoroutine(kbRoutine);
        }

        if (!enemy.isAlive) return;
        enemy.rb.isKinematic = true;
        kbRoutine = StartCoroutine(KBResetRoutine(stunTime));
    }

    public IEnumerator KBResetRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (enemy.isAlive)
        {
            enemy.isKnocked = false;
            enemy.body.BoolAnim("IsKnocked", false);
            navMeshAgent.enabled = true;
        }
    }

    public IEnumerator StrafeRoutine(float strafeTime)
    {
        yield return new WaitForSeconds(strafeTime);
        dir = Random.Range(0, 2) == 0 ? -1 : 1; // -1 = left, 1 = right

        StartCoroutine(StrafeRoutine(5f));
    }

    public IEnumerator PrevPosition()
    {
        yield return new WaitForSeconds(.1f);
        if (enemy.isAggro || !enemy.isKnocked || enemy.isAlive || enemy.combat.isAttacking)
        {
            Vector3 currentPosition = transform.position;
            Vector3 movementDelta = currentPosition - previousPosition;
            previousPosition = currentPosition;

            Vector3 localMovement = enemy.head.InverseTransformDirection(movementDelta.normalized);
            

            float x = localMovement.x;
            float z = localMovement.z;

            if (navMeshAgent.isOnNavMesh)
            {
                if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance || movementDelta.magnitude >= threshold)
                {
                    if (Mathf.Abs(z) > Mathf.Abs(x))
                    {
                        if (z > threshold)
                            movDir = 1;
                        else if (z < -threshold)
                            movDir = -1;
                    }
                    else if (x > threshold)
                    {
                        movDir = 2;
                    }
                    else if (x < -threshold)
                    {
                        movDir = -2;
                    }

                    enemy.body.IntAnim("MovingDirection", movDir);
                }
            }
        }
        positionRoutine = StartCoroutine(PrevPosition());
    }

    public virtual void StopMovement()
    {
        navMeshAgent.isStopped = true;
        enemy.body.IntAnim("MovingDirection", 0);
    }
    public virtual void ChasePlayer()
    {
        Debug.Log(name + " Before");
        if (!isStationery && !enemy.isKnocked) navMeshAgent.isStopped = false;
        Debug.Log(name + " Passed");
    }
    public virtual void BeginChase()
    {
        if (idleRoutine != null)
        {
            StopCoroutine(idleRoutine);
        }
        isPatrolling = false;
        navMeshAgent.stoppingDistance = stoppingDistance;
        navMeshAgent.speed = moveSpeed;
        previousPosition = transform.position;
        positionRoutine = StartCoroutine(PrevPosition());
        ChasePlayer();
    }

    public virtual void SpotPlayer()
    {
        navMeshAgent.destination = enemy.target.position;
    }
}
