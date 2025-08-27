using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool knocksOver;
    public bool harmful = true;
    public bool rotates = false;
    public float rotateSpeed = 0f;
    public int damage;
    void Start()
    {

    }

    void Update()
    {
        if (rotates)
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);


    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Unit")
        {
            Runner r = other.GetComponent<Runner>();

            if (harmful)
                r.OnHit(damage);
            if (knocksOver)
                r.KnockOver(2f);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Unit")
        {
            Runner r = other.GetComponent<Runner>();
            if (knocksOver)
                r.KnockOver(2f);
        }
    }
}
