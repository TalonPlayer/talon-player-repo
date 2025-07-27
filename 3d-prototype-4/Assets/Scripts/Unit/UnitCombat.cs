using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    public float attackDelay;
    public float attackTime;
    public float cooldown;
    public bool canAttack = true;
    public bool isAttacking = false;
    public int damage;
    public float attackRange = 0.5f;
    private Unit unit;
    private Coroutine attackRoutine;
    void Awake()
    {
        unit = GetComponent<Unit>();
    }
    void Start()
    {

    }

    void Update()
    {

    }
    public void OnDeath()
    {
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
        unit.movement.ToggleMovement(false);
        unit.body.PlayRandom("Attack", unit.attackCount);
        unit.body.Play("IsAttacking", true);

        yield return new WaitForSeconds(attackDelay);

        Attack();


        yield return new WaitForSeconds(attackTime);
        isAttacking = false;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
        unit.body.Play("IsAttacking", false);
    }


    private void Attack()
    {
        if (unit.target == null) return;

        Enemy e = unit.target.GetComponent<Enemy>();
        if (e && e.isAlive)
            e.OnHit(damage);
    }
}
