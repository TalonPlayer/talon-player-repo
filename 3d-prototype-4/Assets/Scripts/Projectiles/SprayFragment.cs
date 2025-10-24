using System.Collections;
using UnityEngine;

public class SprayFragment : Projectile
{
    public override void Launch(Vector3 dir, int dmg, string name = "None")
    {
        damage = dmg;
        collateral = 0;
        direction = dir.normalized;
        rb.velocity = direction * speed;
        owner = name;
        StartCoroutine(LifeTime());
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy e = other.GetComponent<Enemy>();
            if (owner != "None")
                if (e.IsKilled(damage, owner))
                    GlobalSaveSystem.AddAchievementProgress(owner + "_kills", 1);
            e.OnHit(damage);
            collateral--;
            if (collateral <= 0)
            {
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    public override IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifetime);

        Destroy(gameObject);
    }
}
