using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeCombat : EnemyCombat
{
    public LayerMask playerLayer;
    public Transform hitBox;

    [Header("Melee")]
    public float attackDelay = .5f;
    public float attackForce = 50f;
    public float attackRange = 2.5f;
    public float attackTime;
    public float attackCooldown = 1f;
    public int damage = 1;
    public override void CheckDistance()
    {
        if (enemy.distance <= attackRange && enemy.movement.isFacing)
        {
            if (canAttack)
            {
                attackRoutine = StartCoroutine(DelayAttack());
            }
            else
            {
                enemy.body.BoolAnim("IsWalking", true);
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
        hitBox.gameObject.SetActive(false);
    }
    protected virtual IEnumerator DelayAttack()
    {
        canAttack = false;
        isAttacking = true;
        enemy.body.IntAnim("AttackNum", Random.Range(0, enemy.body.atkAnims));
        enemy.body.animator.Play("Attack");
        enemy.movement.StopMovement();

        yield return new WaitForSeconds(attackDelay); // small delay for wind-up

        if (!enemy.isKnocked || noInterruption && enemy.isAlive)
        {

            hitBox.gameObject.SetActive(true);

            Vector3 center = enemy.head.position + enemy.head.forward * 2f;
            Collider[] hits = Physics.OverlapBox(
                center,
                hitBox.localScale,
                enemy.head.rotation,
                playerLayer
            );

            foreach (Collider coll in hits)
            {
                if (coll.CompareTag("Player"))
                {
                    Player player = coll.GetComponent<Player>();
                    player.OnHit(damage, transform.position, attackForce);
                }
            }
        }

        yield return new WaitForSeconds(attackTime);
        enemy.movement.ChasePlayer();
        isAttacking = false;

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        hitBox.gameObject.SetActive(false);
    }
}
