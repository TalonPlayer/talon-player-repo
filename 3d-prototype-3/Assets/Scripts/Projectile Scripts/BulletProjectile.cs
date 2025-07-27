using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : Projectile
{
    public override void Fire(Vector3 _direction, float _moveSpeed, int _damage)
    {
        direction = _direction;
        moveSpeed = _moveSpeed;
        damage = _damage;
    }

    public override void Move()
    {
        rb.velocity = direction * moveSpeed * Time.fixedDeltaTime;
    }
}
