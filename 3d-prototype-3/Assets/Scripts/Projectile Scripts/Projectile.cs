using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public Rigidbody rb;
    protected float moveSpeed;
    protected Vector3 direction;
    protected int damage;
    public abstract void Fire(Vector3 _direction, float _moveSpeed, int _damage);
    public abstract void Move();
    void FixedUpdate()
    {
        Move();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.gameObject.GetComponent<Player>();
            Debug.Log("Player was shot");
        }
        else if (other.tag == "Plant")
        {
            Plant plant = other.gameObject.GetComponent<Plant>();
            Debug.Log("Plant was shot");
        }
        else if (other.tag == "Robot")
        {
            Robot robot = other.gameObject.GetComponent<Robot>();
            robot.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
