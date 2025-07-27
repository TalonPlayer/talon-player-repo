using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRangeMovement : EnemyMovement
{
    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        if (isPatrolling && currentwaypoint != null)
        {
            PatrolState();
        }
        if (
            !enemy.isAggro ||
            enemy.isKnocked ||
            !enemy.isAlive ||
            !navMeshAgent.isOnNavMesh) return;


        Vector3 lookDirection = enemy.target.position - enemy.head.position;
        RaycastHit hit;
        Ray ray = new Ray(transform.position, lookDirection);
        if (Physics.Raycast(ray, out hit, 50f))
        {
            if (hit.collider.tag == "Player")
            {
                navMeshAgent.updateRotation = false;

                // Constantly look at the target
                lookDirection.y = 0f;

                if (lookDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    enemy.head.rotation = Quaternion.Slerp(enemy.head.rotation, targetRotation, Time.deltaTime * turnSpeed);
                }
                isFacing = Vector3.Angle(enemy.head.forward, lookDirection.normalized) <= viewAngle;
                if (!enemy.combat.isAttacking) MoveState();

            }
            else
            {
                navMeshAgent.stoppingDistance = 0f;
                navMeshAgent.destination = enemy.target.position;
            }
        }


        
    }



    protected override void MoveState()
    {
        // Retreat logic stays the same
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
        // Strafe when within distance regardless of attack state
        else if (enemy.distance <= stoppingDistance + 2f && strafes)
        {
            if (!isStrafing)
            {
                dir = Random.Range(0, 2) == 0 ? -1 : 1;
                strafeRoutine = StartCoroutine(StrafeRoutine(5f));
                isStrafing = true;
                navMeshAgent.speed = walkSpeed;
            }

            Vector3 strafeDirection = enemy.head.right * dir;
            Vector3 strafeTarget = transform.position + strafeDirection * strafeRange;
            SamplePosition(strafeTarget, strafeDirection);
        }
        // Chase if too far
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
}
