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
    public bool isRanged = false;
    public ParticleSystem muzzleFlash;
    public Projectile projectilePrefab;
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

        if (!isRanged)
        {
            unit.body.PlayRandom("Attack", unit.attackCount);
            unit.body.Play("IsAttacking", true);
        }
        yield return new WaitForSeconds(attackDelay);

        if (isRanged)
            Shoot();
        else
            Attack();
        yield return new WaitForSeconds(attackTime);
        isAttacking = false;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
        unit.body.Play("IsAttacking", false);
    }

    private void Shoot()
    {
        RaycastHit hit;
        Vector3 pos = muzzleFlash.transform.parent.position;
        LayerMask layer = (1 << 13) | (1 << 6) | (1 << 8);
        Debug.DrawRay(pos, transform.forward * attackRange, Color.cyan, 2f);
        if (Physics.Raycast(pos, transform.forward, out hit, attackRange, layer))
        {
            if (!hit.collider.CompareTag("Enemy")) return;
            Projectile proj = Instantiate(projectilePrefab,
            muzzleFlash.transform.parent.position,
            Quaternion.LookRotation(transform.forward),
            PlayerManager.Instance.bulletFolder);

            proj.Launch(transform.forward, damage, unit._name);
            muzzleFlash.Play();

            unit.body.PlayRandom("Attack", unit.attackCount);
            unit.body.Play("IsAttacking", true);
        }
    }
    private void Attack()
    {
        if (unit.target == null) return;

        Enemy e = unit.target.GetComponent<Enemy>();
        if (e && e.isAlive)
        {
            //if (e.IsKilled(damage, unit.owner._name))
                //GlobalSaveSystem.AddAchievementProgress(unit.owner._name + "_kills", 1);
            e.OnHit(damage);
        }

    }
}
