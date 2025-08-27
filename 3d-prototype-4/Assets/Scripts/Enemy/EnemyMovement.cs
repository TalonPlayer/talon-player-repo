using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public float facingThreshold = .65f; // value must be between 0 and 1
    public float attackRange = 0.5f;
    private Enemy enemy;
    private float radius;
    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    void Start()
    {
        radius = agent.radius;
    }

    public void Init(int _min, int _max)
    {
        // Randomize move speed
        float speed = Random.Range(_min, _max);
        agent.speed = speed;

        // Only run if the movement speed is high enough
        enemy.body.Play("IsRunning", speed > 2.5f);

        if (speed > 9f)
            agent.angularSpeed *= 4f;
    }

    void Update()
    {
        // If enemy is not aggro, has no target, or the player is dead, don't move
        if (!enemy.isAggro || !PlayerManager.Instance.player.isAlive || enemy.target == null)
        {
            ToggleMovement(false);
            return;
        }
        ToggleMovement(true);
        agent.destination = enemy.target.transform.position;
        float distanceToTarget = Vector3.Distance(transform.position, enemy.target.transform.position);
        if (IsFacingTarget() && distanceToTarget <= agent.stoppingDistance + attackRange)
        {
            // Don't move if close to target
            ToggleMovement(false);

            // If the player is immune, kill the enemy
            if (enemy.target.name == "Player")
            {
                if (PlayerManager.Instance.player.isImmune
                || PlayerManager.Instance.player.movement.isDashing)
                    enemy.OnDeath();
            }
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

    /// <summary>
    /// Is the enemy facing the target given the threshold
    /// </summary>
    /// <returns></returns>
    private bool IsFacingTarget()
    {
        if (enemy.target == null || enemy.combat.isAttacking) return false;

        Vector3 toTarget = (enemy.target.transform.position - transform.position).normalized;
        toTarget.y = 0f;

        float dot = Vector3.Dot(transform.forward, toTarget);

        return dot > facingThreshold;
    }

    public void AlterSpeed(float multiplier)
    {
        agent.speed *= multiplier;
        enemy.body.Play("IsRunning", agent.speed > 2.5f);
    }
}
