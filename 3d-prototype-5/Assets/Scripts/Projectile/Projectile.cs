using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 3f;
    public string owner;
    public Rigidbody rb;
    public bool activated;
    protected Vector3 direction;
    protected Coroutine lifeRoutine;
    /// <summary>
    /// Give the projectile a direction to move towards
    /// </summary>
    /// <param name="dir"></param>
    public virtual void Launch(Vector3 dir, string name = "None")
    {
        direction = dir.normalized;
        rb.velocity = direction * speed;
        owner = name;
        activated = true;
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


    void OnCollisionEnter(Collision collision)
    {
        if (!activated) return;
        var other = collision.gameObject;
        if (other.CompareTag("Zombie"))
        {
            MyEntity e = other.GetComponent<MyEntity>();
            if (!e.isAlive) return;
            e.OnDeath();

            activated = false;
        }
        // Wall or ground is hit, stop the projectile
        else if (other.CompareTag("Wall") || other.CompareTag("Ground"))
        {
            activated = false;
        }
    }
}
