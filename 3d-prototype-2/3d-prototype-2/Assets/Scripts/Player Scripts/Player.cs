using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement movement;
    public PlayerInteraction interact;
    public PlayerCombat combat;
    public PlayerAbilities abilities;
    public Animator animator;
    public CapsuleCollider cc;

    [Header("Stats")]
    public int health; // Max health is 8
    public float punchDamage = 25f;
    public float flyKickDamage = 15f;
    public float energy;
    public bool isAlive = true;

    void Awake()
    {
        // Link self to subsystems
        movement.SetPlayer(this);
        combat.SetPlayer(this);
        interact.SetPlayer(this);
        abilities.SetPlayer(this);
    }

    public void CanFight(bool canAttack)
    {
        combat.canAttack = canAttack;
    }
    public void Respawn()
    {
        EnableInput();
        HUDController.Instance.ChangeHealth(8);
        isAlive = true;
        health = 8;
    }

    public void SetPosition(Transform position)
    {
        movement.StopMovement();
        transform.position = position.position;
        movement.SetCamera(position.rotation);
    }

    public void DisableInput()
    {
        movement.StopMovement();
        movement.enabled = false;
        combat.enabled = false;
        interact.enabled = false;
    }

    public void EnableInput()
    {
        movement.enabled = true;
        combat.enabled = true;
        interact.enabled = true;
    }

    public void PlayAnim(string anim)
    {
        animator.SetTrigger(anim);
    }
    public void IntAnim(string anim, int num)
    {
        animator.SetInteger(anim, num);
    }
    public void BoolAnim(string anim, bool active)
    {
        animator.SetBool(anim, active);
    }

    public void BoolAnimFalse(string anim)
    {
        animator.SetBool(anim, false);
    }
    public void OnHit(int damage, Vector3 incomingDir, float knockBackForce)
    {
        if (isAlive)
        {
            TakeDamage(damage);
            if (isAlive)
            {
                Vector3 direction = transform.position - incomingDir;
                movement.KnockBack(direction.normalized, knockBackForce);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        HUDController.Instance.deathAnimator.Play("Damaged");
        animator.Play("Hurt Right");
        HUDController.Instance.ChangeHealth(health);
        CheckHealth();
    }

    void CheckHealth()
    {
        if (health <= 0)
        {
            isAlive = false;
            GameManager.Instance.camAnimator.SetBool("IsCinematic", true);
            PlayAnim("Dead");
            combat._audio.PlayDeathSound();
            StartCoroutine(DeathDelay());
        }
    }

    public IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(1f);
        HUDController.Instance.PlayDeathAnim(true);
        WorldController.Instance.Reset();
    }
}
