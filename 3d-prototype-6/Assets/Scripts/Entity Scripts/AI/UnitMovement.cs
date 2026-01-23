using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Use this for setting movement and checking location
/// </summary>
public class UnitMovement : MonoBehaviour
{
    private Unit main;
    public NavMeshAgent agent;
    public bool isMoving;
    public bool isFacingTarget;
    public float rotSpeed = 5f;
    public float sprintSpeed;
    public float distFromTarget;
    public bool attackingFromCover;
    public bool inCover = false;
    public float DefaultSpeed { get { return _defSpeed; } }
    private float _defSpeed;
    public bool HasReachedDestination { get { return agent.remainingDistance <= agent.stoppingDistance + .1f; } }

    public CoverPoint coverPoint;
    private Vector3 _lookDir;
    void Awake()
    {
        main = GetComponent<Unit>();
    }

    void Start()
    {
        _defSpeed = agent.speed;
    }

    public void FSM()
    {
        UpdateAnimations();

        if (inCover) return;

        switch (main.ai.orientation)
        {
            case FacingMode.FaceMoveDirection:
                agent.updateRotation = true;
                break;
            case FacingMode.FaceDirection:
                agent.updateRotation = false;
                RotateToDirection(_lookDir);
                break;
            case FacingMode.FaceTarget:
                agent.updateRotation = !main.vision.isLosClear;
                if (main.vision.isLosClear)
                {
                    RotateToTarget();
                }
                break;
        }
    }

    public UnitWorldState Tick(UnitWorldState s)
    {
        distFromTarget = Vector3.Distance(PlayerManager.Instance.PlayerPos, transform.position);
        s.InCover = inCover;
        s.HasAimOnTarget = isFacingTarget && s.HasClearShot;
        return s;
    }


    public void OnDeath()
    {
        agent.isStopped = true;
    }


    public void MoveTo(Vector3 pos)
    {
        if (TrySampleOnNavMesh(pos, 35f, out Vector3 sampled))
            agent.SetDestination(sampled);
        else
            agent.SetDestination(pos);
    }

    public void Strafe(float strafeAmount)
    {
        MoveTo((strafeAmount * transform.forward) + transform.position);
    }

    public bool IsInRange(float radius)
    {
        return agent.remainingDistance <= radius;
    }

    public void ToggleMovement(bool canMove)
    {
        agent.isStopped = !canMove;
        main.body.Play("IsMoving", canMove);
    }

    public void SetLookDirection(Vector3 dir)
    {
        _lookDir = dir;
    }

    public void RotateToDirection(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Adjust speed
        }
    }

    public void RotateToTarget()
    {
        if (main.target == null) return;

        agent.updateRotation = false;
        Vector3 toTarget = main.TargetPos - transform.position;
        Vector3 direction = toTarget.normalized;
        direction.y = 0f; // Ignore vertical difference

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed); // Adjust speed
        }

        float angle = Vector3.Angle(direction, transform.forward);
        isFacingTarget = angle <= 7f;

    }
    private void UpdateAnimations()
    {
        Vector3 vel = GetForwardVelocity().normalized;
        isMoving = vel.magnitude > .001f;

        float forwardAmount = Vector3.Dot(vel, transform.forward);
        float strafeAmount = Vector3.Dot(vel, transform.right);

        main.body.Play("IsMoving", isMoving);
        main.body.Play("Forward", forwardAmount);
        main.body.Play("Strafe", strafeAmount);
        main.body.Play("IsAiming", true);

        if (!agent.updateRotation)
            agent.speed = DefaultSpeed;
        else agent.speed = sprintSpeed;
    }

    private Vector3 GetForwardVelocity()
    {
        // A - B
        Vector3 fwd = agent.velocity;
        fwd.y = 0f;
        return fwd;
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
        sampled = target;
        return false;
    }
    public void Orbit(float orbitRadius = 6f, Vector3 targetPos = new Vector3())
    {
        Vector3 offset = Helper.RandomVectorInRadius(orbitRadius);
        if (targetPos == Vector3.zero) targetPos = transform.position;

        if (TrySampleOnNavMesh(targetPos + offset, orbitRadius, out Vector3 sampled))
        {
            MoveTo(sampled);
            return;
        }
        MoveTo(main.TargetPos + offset);
    }
    public void MoveToCover()
    {
        int index = main.ai.AILevelChoice(main.vision.validCovers.Length);
        coverPoint = main.vision.validCovers[index];
        if (coverPoint == null) coverPoint = main.vision.validCovers[0];
        if (coverPoint == null)
        {
            // Debug.Log("There is no valid cover"); 
            return;
        }
        coverPoint.TargetCover(main);
        MoveTo(coverPoint.transform.position);
    }

    public void MoveToCoverPeek()
    {
        main.movement.MoveTo(coverPoint.peekLocation.position);
        // UnCover();
    }

    public void InCover()
    {
        string side = coverPoint.coverDirection == CoverDirection.Right ? "CoverRight" : "CoverLeft";
        main.body.Play(side);
        //main.body.Play("InCover", true);
        inCover = true;
    }

    public void UnCover()
    {
        inCover = false;
        //main.body.Play("InCover", false);
        coverPoint.RemoveOccupant();
    }

    public void AnglePeek(float strafeDistance, Vector3 targetPos)
    {
        Vector3 pos = main.transform.position;
        Vector3 toTarget = targetPos - pos;
        Vector3 direction = toTarget.normalized;
        Vector3 facingWall = Vector3.zero;
        Vector3 otherSide = Vector3.zero;
        LayerMask mask = transform.GetChild(0).gameObject.layer | Layer.Wall;
        RaycastHit hit;

        if (Physics.Raycast(targetPos, -direction, out hit, toTarget.magnitude, mask, QueryTriggerInteraction.Ignore))
        {
            Vector3 tangent = Vector3.Cross(Vector3.up, hit.normal);

            Vector3 left = hit.point + tangent * strafeDistance;
            Vector3 right = hit.point - tangent * strafeDistance;


            //Debug.DrawLine(left, hit.point, Color.red, 1f);
            //Debug.DrawLine(right, hit.point, Color.blue, 1f);

            if (Vector3.Distance(pos, left) < Vector3.Distance(pos, right))
                otherSide = left;
            else
                otherSide = right;
        }

        if (Physics.Raycast(pos, direction, out hit, toTarget.magnitude, mask, QueryTriggerInteraction.Ignore))
        {
            Vector3 tangent = Vector3.Cross(Vector3.up, hit.normal);

            Vector3 left = hit.point + tangent * strafeDistance;
            Vector3 right = hit.point - tangent * strafeDistance;


            //Debug.DrawLine(left, hit.point, Color.red, 1f);
            //Debug.DrawLine(right, hit.point, Color.blue, 1f);

            if (Vector3.Distance(targetPos, left) < Vector3.Distance(targetPos, right))
                facingWall = left;
            else
                facingWall = right;
        }


        if (facingWall != Vector3.zero && otherSide != Vector3.zero)
        {
            Vector3 dest = Vector3.Lerp(facingWall, otherSide, Random.value);

            //Debug.DrawLine(facingWall, otherSide, Color.magenta, 1f);
            //Debug.DrawLine(pos, dest, Color.green, 1f);
            main.movement.MoveTo(dest);
        }
    }
}
