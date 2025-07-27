using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySummonerCombat : EnemyRangeCombat
{
    [Header("Summoner")]
    public GameObject minionPrefab;
    public int summonCount;
    public float summonRadius;
    public float summonChance;
    public float summonDelay;
    public float summonTime;
    private bool firstSummon = true;
    private List<Enemy> minions = new List<Enemy>();
    public override void CheckDistance()
    {
        if (enemy.distance <= attackRange && enemy.movement.isFacing)
        {
            if (canAttack)
            {
                RaycastHit hit;

                Vector3 direction = enemy.target.position - transform.position;
                Ray ray = new Ray(transform.position, direction);
                if (Physics.Raycast(ray, out hit, direction.magnitude))
                {
                    if (hit.collider.tag == "Player")
                    {
                        if (Random.Range(0, 100) < summonChance || firstSummon)
                        {
                            attackRoutine = StartCoroutine(SummonRoutine());
                            firstSummon = false;
                        }
                            
                        else
                            attackRoutine = StartCoroutine(FireRoutine());
                    }
                }
            }
        }
    }
    public override void OnDeath()
    {
        base.OnDeath();

        foreach (Enemy minion in minions)
        {
            if (minion.isAlive)
            {
                minion.onDeath?.Invoke();
            }
        }
    }
    public void Summon()
    {
        Vector3 center = transform.position;
        for (int i = 0; i < summonCount; i++)
        {
            // Get angle in radians around the summoner
            float angle = i * (360f / summonCount) * Mathf.Deg2Rad;

            // Calculate position in a circle
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * summonRadius;
            Vector3 spawnPos = center + offset;

            // Snap to closest point on NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPos, out hit, 10f, NavMesh.AllAreas))
            {
                GameObject obj = Instantiate(minionPrefab, hit.position, enemy.head.rotation, EnemyManager.Instance.transform);
                Enemy minion = obj.GetComponent<Enemy>();

                minions.Add(minion);
            }
        }

        Invoke(nameof(AggroMinions), 1f);
    }
    public void AggroMinions()
    {
        foreach (Enemy minion in minions)
        {
            if (minion.isAlive && !minion.isAggro)
                minion.onAggro?.Invoke();
        }
    }
    public IEnumerator SummonRoutine()
    {
        canAttack = false;
        enemy.movement.StopMovement();
        enemy.body.animator.Play("Summon Attack");
        yield return new WaitForSeconds(summonDelay);
        if (!enemy.isKnocked || noInterruption && enemy.isAlive)
        {
            isAttacking = true;
            Summon();
        }
        yield return new WaitForSeconds(summonTime);
        isAttacking = false;
        enemy.movement.ChasePlayer();
        yield return new WaitForSeconds(fireCooldown);
        canAttack = true;
    }
    
    public override IEnumerator FireRoutine(){
        canAttack = false;
        enemy.movement.StopMovement();
        enemy.body.animator.Play("Attack");
        yield return new WaitForSeconds(fireDelay);
        if (!enemy.isKnocked)
        {
            isAttacking = true;
            Shoot();
        }
        yield return new WaitForSeconds(fireTime);
        isAttacking = false;
        enemy.movement.ChasePlayer();
        yield return new WaitForSeconds(fireCooldown);
        canAttack = true;
    }
}
