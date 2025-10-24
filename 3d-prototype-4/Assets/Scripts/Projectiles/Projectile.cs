using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 3f;
    public int damage;
    public int collateral;
    public int maxCollateral;
    public string owner;
    public Rigidbody rb;
    protected Vector3 direction;
    protected Coroutine lifeRoutine;
    /// <summary>
    /// Give the projectile a direction to move towards
    /// </summary>
    /// <param name="dir"></param>
    public virtual void Launch(Vector3 dir, int dmg, string name = "None")
    {
        damage = dmg;
        collateral = 0;
        direction = dir.normalized;
        rb.velocity = direction * speed;
        owner = name;
        StartCoroutine(LifeTime());
    }
    /// <summary>
    /// Projectiles have a lifespan
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifetime);

        Destroy(gameObject);
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
