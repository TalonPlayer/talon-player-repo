using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public float facingThreshold = .65f; // value must be between 0 and 1
    public float minSpeed = 10f;
    public float maxSpeed = 12f;
    public float rotationSpeed = 5f;
    public bool rotatesToTarget = false;
    private Unit unit;
    private float radius;
    void Awake()
    {
        unit = GetComponent<Unit>();
    }
    void Start()
    {
        if (rotatesToTarget) agent.updateRotation = false;
        radius = agent.radius;
        float speed = Random.Range(minSpeed, maxSpeed);
        agent.speed = speed;

        unit.body.Play("IsRunning", speed > 2.5f);
        unit.body.Play("RandomFloat", Random.Range(0, 1f));
    }
    void Update()
    {
        if (!unit.isAggro || unit.target == null || unit.combat.isAttacking)
        {
            ToggleMovement(false);
            return;
        }

        Vector3 targetPos = unit.target.transform.position;
        Debug.DrawRay(transform.position, transform.forward * 2f, Color.magenta);
        if (Physics.Raycast(transform.position, transform.forward, 2f, (1 << 16)))
        {
            ToggleMovement(false);
            RotateToTarget();
        }
        else
        {
            agent.SetDestination(targetPos);
            ToggleMovement(true);
        }


        if (rotatesToTarget) RotateToTarget();

        float distanceToTarget = Vector3.Distance(transform.position, unit.target.transform.position);
        if (distanceToTarget <= agent.stoppingDistance + unit.combat.attackRange)
        {
            if (IsFacingTarget())
            {
                if (unit.combat.canAttack)
                    unit.combat.StartAttack();
            }

        }
    }

    private void RotateToTarget()
    {
        if (unit.target == null || unit.combat.isAttacking) return;

        Vector3 direction = (unit.target.position - transform.position).normalized;
        direction.y = 0f; // Ignore vertical difference

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed); // Adjust speed
        }
    }

    public void OnDeath()
    {
        ToggleMovement(false);
    }

    public void ToggleMovement(bool on)
    {
        agent.isStopped = !on;

        if (unit.isAlive && !unit.combat.isAttacking)
            unit.body.Play("IsChasing", on);
    }

    private bool IsFacingTarget()
    {
        if (unit.target == null || unit.combat.isAttacking) return false;

        Vector3 toTarget = (unit.target.transform.position - transform.position).normalized;
        toTarget.y = 0f;

        float dot = Vector3.Dot(transform.forward, toTarget);

        return dot > facingThreshold; // You can adjust the threshold (1 = perfect alignment)
    }
}
