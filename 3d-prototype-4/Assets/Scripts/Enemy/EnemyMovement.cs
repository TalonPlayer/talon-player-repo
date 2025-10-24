using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public float facingThreshold = .65f; // value must be between 0 and 1
    public float attackRange = 0.5f;
    public bool pauseAttack;
    private Enemy enemy;
    private float radius;
    public bool canJump = false;
    public bool isJumping = false;
    public float speedMultiplier = 1f;
    Coroutine jumpRoutine;
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
        agent.speed = speed * speedMultiplier;

        // Only run if the movement speed is high enough
        enemy.body.Play("IsRunning", speed > 2.5f);

        if (speed > 9f)
            agent.angularSpeed *= 4f;

        pauseAttack = false;
        if (canJump) 
        {
            jumpRoutine = StartCoroutine(JumpRoutine());
        }
    }
    IEnumerator JumpRoutine()
    {
        while (enemy.isSpawning)
        {
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        Vector3 dir = enemy.target.transform.position - transform.position;
        if (dir.magnitude >= 3f)
        {
            if (enemy.combat.isAttacking)
                yield return new WaitForSeconds(1.5f);

            if (Random.Range(0, 2) == 0) // Jump
            {
                ObstacleAvoidanceType defaultObsType = agent.obstacleAvoidanceType;
                float defaultSpeed = agent.speed;
                isJumping = true;

                Vector3 targetPos = enemy.target.transform.position;

                Vector2 circle = Random.insideUnitCircle * 1.5f; // radius = 1.5, adjust as needed
                Vector3 offset = new Vector3(circle.x, 0f, circle.y);

                agent.destination = targetPos + offset + dir;
                agent.speed *= 3f;

                enemy.body.Play("Jump");

                agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                yield return new WaitForSeconds(1.25f);
                agent.obstacleAvoidanceType = defaultObsType;
                agent.destination = enemy.target.transform.position;
                agent.speed = defaultSpeed;
                isJumping = false;
            }
        }

        jumpRoutine = StartCoroutine(JumpRoutine());
    }
    void Update()
    {
        if (isJumping) return;
        if (pauseAttack && enemy.combat.isAttacking) return;
        if (!PlayerManager.Instance.AllPlayersAlive())
        {
            ToggleMovement(false, true);
            return;
        }
        // If enemy is not aggro, has no target don't move
        if (!enemy.isAggro || enemy.target == null)
        {
            ToggleMovement(false);
            return;
        }
        ToggleMovement(true);
        agent.destination = enemy.target.transform.position;

        if (enemy.combat.isAttacking) return;
        float distanceToTarget = Vector3.Distance(transform.position, enemy.target.transform.position);
        if (IsFacingTarget() && distanceToTarget <= agent.stoppingDistance + attackRange)
        {
            // Don't move if close to target
            ToggleMovement(false);
            enemy.combat.StartAttack();
        }
    }

    public void OnDeath()
    {
        ToggleMovement(false);

        if (jumpRoutine != null) StopCoroutine(jumpRoutine);
    }

    public void ToggleMovement(bool on, bool playerIsDead = false)
    {
        // If on is true, don't stop the movement
        agent.isStopped = !on;
        if (playerIsDead)
        {
            enemy.body.PlayRandom("IsChasing", 4, false);
            return;
        }
        // Logic for normal zombies that keep attacking
        if (enemy.isAlive) enemy.body.PlayRandom("IsChasing", 4, on);
        if (!pauseAttack && enemy.combat.isAttacking) enemy.body.PlayRandom("IsChasing", 4, true);
        if (pauseAttack && !on) enemy.body.PlayRandom("IsChasing", 4, true);
    }

    /// <summary>
    /// Is the enemy facing the target given the threshold
    /// </summary>
    /// <returns></returns>
    public bool IsFacingTarget()
    {
        Vector3 toTarget = (enemy.target.transform.position - transform.position).normalized;
        toTarget.y = 0f;

        float dot = Vector3.Dot(transform.forward, toTarget);

        return dot > facingThreshold;
    }

    public void AlterSpeed(float multiplier)
    {
        agent.speed *= multiplier;
        enemy.body.Play("IsRunning", agent.speed > 4f);
    }
}
