using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantCombat : MonoBehaviour
{
    [Header("Combat Info")]
    public bool isAttacking = false;
    public bool noInterruption = false;
    public bool canAttack = true;
    public Transform firePoint;

    [Header("Combat Stats")]
    public float fireDelay;
    public float fireCooldown;
    public float fireTime;
    public float bulletSpeed = 1000f;
    public float attackRange;
    public float maxAimOffset = 0; // 0 means 100% accuracy
    public float inBetweenTime = .25f;
    public int numOfShots = 1;
    public int damage = 15;
    public Projectile projectilePrefab;
    public Coroutine attackRoutine;
    private Plant plant;
    void Start()
    {
        plant = GetComponent<Plant>();
    }

    void Update()
    {
        if (!plant.isAggro) return;

        if (plant.body.isFacing && !isAttacking)
        {
            attackRoutine = StartCoroutine(BurstFireRoutine());
        }
    }

    public void Aggro()
    {
        PlantManager.checkTarget += CheckTarget;
    }

    public void CheckTarget()
    {
        if (!plant.target.isAlive)
        {
            PlantManager.checkTarget -= CheckTarget;
            plant.Pacify();
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
    public void Shoot()
    {
        if (plant.target == null) return;
        Vector3 direction = plant.target.transform.position - firePoint.position;
        Vector3 offset =
            plant.body.body.right * Random.Range(-maxAimOffset, maxAimOffset) +
            plant.body.body.up * Random.Range(-maxAimOffset, maxAimOffset);     
        Vector3 offsetDirection = (direction + offset).normalized;
        Quaternion rotation = Quaternion.LookRotation(offsetDirection) * Quaternion.Euler(-90f, 0f, 0f);
        Projectile bullet = Instantiate(projectilePrefab, firePoint.position, rotation);
        bullet.Fire(
            offsetDirection,
            bulletSpeed,
            damage
            );
    }
    public IEnumerator FireRoutine()
    {
        yield return new WaitForSeconds(fireDelay);
        Shoot();
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
        
        yield return new WaitForSeconds(fireCooldown); // cooldown until next burst
        canAttack = true;
        
    }
}
