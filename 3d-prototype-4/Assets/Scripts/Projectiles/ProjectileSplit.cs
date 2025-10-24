using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSplit : Projectile
{
    public Projectile split;
    public ParticleSystem explosiveParticles;
    public Renderer _renderer;
    public int splitAmount;
    private float angleStep;
    public override void Launch(Vector3 dir, int dmg, string name = "None")
    {
        damage = dmg;
        collateral = 0;
        direction = dir.normalized;
        rb.velocity = direction * speed;
        angleStep = 360f / splitAmount;
        owner = name;
        StartCoroutine(LifeTime());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // Enemy hit
        {
            Enemy e = other.GetComponent<Enemy>();

            if (owner != "None")
                if (e.IsKilled(damage, owner))
                    GlobalSaveSystem.AddAchievementProgress(owner + "_kills", 1);
            e.OnHit(damage);
            collateral++;
            // Projectile can no longer pass through enemies
            if (collateral >= maxCollateral)
            {
                rb.velocity = Vector3.zero;

                Destroy(gameObject);
            }
        }

        else if (other.CompareTag("Boss"))
        {
            ISpyWorld boss = other.GetComponent<ISpyWorld>();
            boss.OnHit(damage);
            rb.velocity = Vector3.zero;
            Destroy(gameObject);
        }

        // Wall or ground is hit, stop the projectile
        else if (other.CompareTag("Wall") || other.CompareTag("Ground"))
        {
            rb.velocity = Vector3.zero;
            Destroy(gameObject);
        }
    }
}
