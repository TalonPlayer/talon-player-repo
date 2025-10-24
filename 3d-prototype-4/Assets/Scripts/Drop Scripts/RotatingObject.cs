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
    public SphereCollider sc;
    public Player player;
    protected float currentAngle = 0f;
    protected virtual void Start()
    {
        if (Random.Range(0, 2) == 0) rotateSpeed *= -1f;

        // Set initial position
        Vector3 offset = new Vector3(radius, 0f, 0f);
        transform.position = player.transform.position + offset;
        
        StartCoroutine(LifeTimeRoutine());
    }

    protected virtual void Update()
    {
        if (player == null) return;

        // Continuously rotate around player
        currentAngle += rotateSpeed * Time.deltaTime;
        float rad = currentAngle * Mathf.Deg2Rad;

        // Offset the distance from the player
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius;
        transform.position = player.transform.position + offset;
        transform.rotation = Quaternion.LookRotation(transform.position - player.transform.position);
    }

    /// <summary>
    /// After the lifespan has ended, activate other colliders so it falls on the ground
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator LifeTimeRoutine()
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
        if (sc)
        {
            sc.enabled = true;
            sc.isTrigger = false;
        }
        Vector3 direction = RandExt.RandomDirection(30f, 80f); // Upward arc between 30° and 80°
        Vector3 torque = RandExt.ApplyTorque(direction, 5f);
        rb.AddForce(direction * 3f, ForceMode.Impulse);
        rb.AddTorque(torque, ForceMode.Impulse);

        enabled = false;
    }

    /// <summary>
    /// Kill only enemies
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Enemy e = other.GetComponent<Enemy>();

            e.OnHit(9999);
        }
    }
}
