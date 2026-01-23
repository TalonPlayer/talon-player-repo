using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BodyPart: MonoBehaviour
{
    [HideInInspector] public Entity main;
    [HideInInspector] public float damageMult;
    private Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }

    public void Hit(Vector3 direction, float force)
    {
        rb.AddForce(direction * force, ForceMode.Impulse);
    }
}
