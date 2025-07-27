using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public float facingThreshold = .65f; // value must be between 0 and 1
    private Enemy enemy;
    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public void Init(int _min, int _max)
    {
        float speed = Random.Range(_min, _max);
        agent.speed = speed;

        enemy.body.Play("IsRunning", speed > 2.5f);

        if (speed > 9f)
            agent.angularSpeed *= 4f;
    }

    void Update()
    {
        if (!enemy.isAggro || !PlayerManager.Instance.player.isAlive || enemy.target == null)
        {
            ToggleMovement(false);
            return;
        }

        ToggleMovement(true);
        agent.destination = enemy.target.transform.position;
        float distanceToTarget = Vector3.Distance(transform.position, enemy.target.transform.position);
        if (IsFacingTarget() && distanceToTarget <= agent.stoppingDistance + 0.1f)
        {
            ToggleMovement(false);
            if (PlayerManager.Instance.player.isImmune)
                enemy.OnDeath();
            else
                enemy.combat.StartAttack();
        }
    }

    public void OnDeath()
    {
        ToggleMovement(false);
    }

    public void ToggleMovement(bool on)
    {
        // If on is true, don't stop the movement
        agent.isStopped = !on;

        if (enemy.isAlive) enemy.body.PlayRandom("IsChasing", 4, on);
    }

    private bool IsFacingTarget()
    {
        if (enemy.target == null || enemy.combat.isAttacking) return false;

        Vector3 toTarget = (enemy.target.transform.position - transform.position).normalized;
        toTarget.y = 0f;

        float dot = Vector3.Dot(transform.forward, toTarget);

        return dot > facingThreshold; // You can adjust the threshold (1 = perfect alignment)
    }
}
