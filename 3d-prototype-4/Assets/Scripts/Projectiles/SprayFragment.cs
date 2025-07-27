using System.Collections;
using UnityEngine;

public class SprayFragment : Projectile
{
    public int damage;
    public override void Launch(Vector3 dir)
    {
        collateral = 0;
        direction = dir.normalized;
        rb.velocity = direction * speed;
        StartCoroutine(LifeTime());
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy e = other.GetComponent<Enemy>();
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
