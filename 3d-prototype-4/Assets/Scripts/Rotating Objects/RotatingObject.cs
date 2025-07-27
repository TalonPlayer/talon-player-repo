using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    public float rotateSpeed = 50f;
    public float radius = 3f; // Distance from player
    public float lifeTime = 5f;
    public Rigidbody rb;
    public CapsuleCollider cc;
    public BoxCollider bc;
    private Transform player;
    private float currentAngle = 0f;

    void Start()
    {
        player = PlayerManager.Instance.player.transform;

        // Set initial position
        Vector3 offset = new Vector3(radius, 0f, 0f);
        transform.position = player.position + offset;

        StartCoroutine(LifeTimeRoutine());
    }

    void Update()
    {
        if (player == null) return;

        // Increase angle over time
        currentAngle += rotateSpeed * Time.deltaTime;

        // Convert angle to radians
        float rad = currentAngle * Mathf.Deg2Rad;

        // Calculate circular orbit position
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius;
        transform.position = player.position + offset;

        // Face outward from player
        transform.rotation = Quaternion.LookRotation(transform.position - player.position);
    }

    IEnumerator LifeTimeRoutine()
    {
        yield return new WaitForSeconds(lifeTime);
        transform.parent = EntityManager.Instance.itemFolder;

        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.isKinematic = false;

        if (bc)
        {
            bc.enabled = true;
            bc.isTrigger = false;
        }
        if (cc)
        {
            cc.enabled = true;
            cc.isTrigger = false;
        }
        Vector3 direction = RandExt.RandomDirection(30f, 80f); // Upward arc between 30° and 80°
        Vector3 torque = RandExt.ApplyTorque(direction, 5f);
        rb.AddForce(direction * 3f, ForceMode.Impulse);
        rb.AddTorque(torque, ForceMode.Impulse);

        enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Enemy e = other.GetComponent<Enemy>();

            e.OnHit(9999);
        }
    }
}
