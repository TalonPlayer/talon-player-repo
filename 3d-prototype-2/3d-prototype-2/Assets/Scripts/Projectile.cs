using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float moveSpeed;
    private int damage;
    private Vector3 direction;
    public Rigidbody rb;
    public void Fire(Vector3 direction, int damage, float movespeed)
    {
        gameObject.SetActive(true);
        this.direction = direction;
        this.damage = damage;
        this.moveSpeed = movespeed;
        StartCoroutine(DestroySelf());
    }

    void FixedUpdate()
    {
        rb.velocity = direction * moveSpeed * Time.fixedDeltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.gameObject.GetComponent<Player>();
            if (player.movement.isDashing) return;
            if (player.movement.flyKickWindow) return;
            direction.y = 0f;
            player.OnHit(damage, direction, 500f);
        }
        Destroy(gameObject);
    }

    public IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
