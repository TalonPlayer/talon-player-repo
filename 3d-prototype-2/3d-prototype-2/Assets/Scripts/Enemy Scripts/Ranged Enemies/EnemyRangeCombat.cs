using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangeCombat : EnemyCombat
{
    [Header("Ranged")]
    public Transform firePoint;
    public float fireDelay;
    public float fireCooldown;
    public float fireTime;
    public float bulletSpeed = 1000f;
    public float attackRange;
    public float maxAimOffset; // 0 means 100% accuracy
    public float inBetweenTime = .25f;
    public int numOfShots;
    public int damage = 1;
    public Projectile projectilePrefab;
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
                        attackRoutine = StartCoroutine(BurstFireRoutine());
                    }
                }
            }
        }
    }

    public override void OnDeath()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }
    public void Shoot()
    {
        Vector3 direction = GetDirection(enemy.target.position, firePoint.position);
        Vector3 offset =
            enemy.head.right * Random.Range(-maxAimOffset, maxAimOffset) +
            enemy.head.up * Random.Range(-maxAimOffset, maxAimOffset);     
        Vector3 offsetDirection = (direction + offset).normalized;
        Quaternion rotation = Quaternion.LookRotation(offsetDirection) * Quaternion.Euler(-90f, 0f, 0f);
        Projectile bullet = Instantiate(projectilePrefab, firePoint.position, rotation);
        bullet.Fire(offsetDirection, damage, bulletSpeed);
    }
    public virtual IEnumerator FireRoutine()
    {
        enemy.Play("Attack");
        yield return new WaitForSeconds(fireDelay);

        if ((!enemy.isKnocked || noInterruption) && enemy.isAlive)
        {
            Shoot();
        }
    }

    public IEnumerator BurstFireRoutine()
    {
        canAttack = false;
        isAttacking = true;

        for (int i = 0; i < numOfShots; i++)
        {
            yield return StartCoroutine(FireRoutine());

            if (i < numOfShots - 1)
                yield return new WaitForSeconds(inBetweenTime); // delay only between shots
        }

        yield return new WaitForSeconds(fireTime); // delay after burst
        isAttacking = false;
        enemy.Play("Reload");
        
        yield return new WaitForSeconds(fireCooldown); // cooldown until next burst
        canAttack = true;
        
    }
}
