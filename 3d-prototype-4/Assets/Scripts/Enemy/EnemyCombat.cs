using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public GameObject hitbox;
    public float attackDelay;
    public float attackTime;
    public bool canAttack = true;
    public bool isAttacking = false;
    private Enemy enemy;
    private Coroutine attackRoutine;
    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }
    void Start()
    {
        
    }

    void Update()
    {

    }
    public void OnDeath()
    {
        // Stop attack routine to prevent attacks after death
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }
    public void StartAttack()
    {
        attackRoutine = StartCoroutine(AttackRoutine());
    }

    public IEnumerator AttackRoutine()
    {
        canAttack = false;
        isAttacking = true;
        enemy.movement.ToggleMovement(false);
        enemy.body.PlayRandom("Attack", 3);
        yield return new WaitForSeconds(attackDelay);
        Attack();
        yield return new WaitForSeconds(attackTime);
        // Continue moving once attack is over
        enemy.movement.ToggleMovement(true);
        canAttack = true;
        isAttacking = false;
    }

    public void Attack()
    {
        if (enemy.target == null) return;
        
        // If the target is a unit, attack unit
        Unit u = enemy.target.GetComponent<Unit>();
        
        // If the target is the player, kill the player if they are not immune
        Player p = enemy.target.GetComponent<Player>();
        if (u)
            if (u.isAlive)
                u.OnHit(enemy.damage);
        if (p)
            if (PlayerManager.Instance.player.isAlive && !PlayerManager.Instance.player.isImmune)
                    PlayerManager.Instance.KillPlayer();
    }

    /*
    void OnDrawGizmos()
    {
        if (hitbox == null) return;

        Gizmos.color = new Color(1f, 0f, 0f, 0.5f); // Semi-transparent red

        Vector3 center = transform.position + transform.forward * .5f;
        Vector3 halfExtents = hitbox.transform.localScale * 0.5f;

        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, hitbox.transform.localScale);
    }
    */
}
