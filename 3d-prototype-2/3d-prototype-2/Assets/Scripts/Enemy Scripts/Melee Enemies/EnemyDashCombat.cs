using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDashCombat : EnemyMeleeCombat
{
    [Header("Dashing")]
    public bool dashWindow;
    public float dashForce;
    private Vector3 direction;
    private Vector3 startingPos;
    protected override IEnumerator DelayAttack()
    {
        canAttack = false;
        isAttacking = true;
        enemy.body.animator.Play("Attack");
        enemy.movement.StopMovement();



        yield return new WaitForSeconds(attackDelay); // small delay for wind-up

        if (!enemy.isKnocked || noInterruption && enemy.isAlive)
        {
            Dash();
        }

        yield return new WaitForSeconds(attackTime);
        enemy.movement.ChasePlayer();
        isAttacking = false;

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        hitBox.gameObject.SetActive(false);
    }

    public void Dash()
    {
        
        dashWindow = true;
        enemy.rb.useGravity = false;
        enemy.rb.isKinematic = false; // Important!
        enemy.rb.velocity = Vector3.zero;

        enemy.movement.navMeshAgent.enabled = false;

        direction = enemy.target.position - transform.position;
        float magnitude = direction.magnitude;
        enemy.cc.includeLayers = new LayerMask();
        enemy.cc.excludeLayers += 6;
        Vector3 dashVector = direction.normalized * dashForce * magnitude;
        enemy.rb.AddForce(dashVector, ForceMode.Impulse);
        enemy.rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);

        Invoke(nameof(ResetDash), .15f);
        Invoke(nameof(ResetDashWindow), .75f);
    }

    public void ResetDash()
    {
        enemy.rb.useGravity = true;
        enemy.cc.includeLayers = 6;
        enemy.cc.excludeLayers -= 6;
    }

    public void ResetDashWindow()
    {
        dashWindow = false;

    }
    
    void OnCollisionEnter(Collision other)
    {
        if (dashWindow)
        {
            if (other.collider.tag == "Player")
            {
                ResetDashWindow();
                enemy.KnockBack(-direction, 4);
                Player player = other.gameObject.GetComponent<Player>();
                player.OnHit(damage, direction, attackForce);
            }
        }
    }
}
