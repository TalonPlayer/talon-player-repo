using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Human"))
        {
            Entity e = other.GetComponent<Entity>();

            e.brain.closestSafeZone = transform;
            e.brain.inSafeZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Human"))
        {
            Entity e = other.GetComponent<Entity>();

            e.brain.inSafeZone = false;
        }
    }
}
