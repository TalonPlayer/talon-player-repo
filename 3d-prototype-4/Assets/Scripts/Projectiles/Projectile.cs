using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 3f;
    public int collateral;
    public Rigidbody rb;
    protected Vector3 direction;
    protected PlayerHand owner;
    protected Coroutine lifeRoutine;
    public virtual void Launch(Vector3 dir)
    {
        collateral = 0;
        direction = dir.normalized;
        rb.velocity = direction * speed;
        StartCoroutine(LifeTime());
    }

    public void SetOwner(PlayerHand ownerHand)
    {
        owner = ownerHand;
    }

    public virtual IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifetime);

        owner.ReturnToPool(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy e = other.GetComponent<Enemy>();
            e.OnHit(owner.hand.damage);
            collateral++;
            if (collateral >= owner.hand.collateral)
            {
                rb.velocity = Vector3.zero;

                if (owner)
                    owner.ReturnToPool(this);
            }
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Ground"))
        {
            rb.velocity = Vector3.zero;
            owner.ReturnToPool(this);
        }
    }
}
