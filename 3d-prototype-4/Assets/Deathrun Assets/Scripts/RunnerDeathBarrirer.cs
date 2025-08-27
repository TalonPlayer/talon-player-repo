using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerDeathBarrirer : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Unit")
        {
            Runner r = other.GetComponent<Runner>();
            r.isAlive = false;
            r.OnDeath();
        }
    }
}
