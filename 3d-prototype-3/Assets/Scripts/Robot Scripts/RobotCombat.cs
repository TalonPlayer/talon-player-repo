using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotCombat : MonoBehaviour
{
    public float attackRange;
    public float attackDelay;
    public float attackTime;
    public float attackCooldown;
    public int damage;
    public bool isAttacking;
    public bool canAttack;
    private float distance;
    public Coroutine attackRoutine;
    private Robot robot;
    void Start()
    {
        robot = GetComponent<Robot>();
    }

    void Update()
    {
        if (robot.target == null) return;

        distance = Vector3.Distance(robot.target.transform.position, transform.position);

        if (distance <= attackRange && canAttack)
        {
            attackRoutine = StartCoroutine(Attack());
        }
    }
    public void OnDeath()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }

    public IEnumerator Attack()
    {
        canAttack = false;
        isAttacking = true;
        yield return new WaitForSeconds(attackDelay);
        Plant plant = robot.target.GetComponent<Plant>();
        Player player = robot.target.GetComponent<Player>();

        if (plant)
            plant.TakeDamage(damage);
        else if (player)
            Debug.Log("Attacked Player");
            
        yield return new WaitForSeconds(attackTime);
        isAttacking = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
