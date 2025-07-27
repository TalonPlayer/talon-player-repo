using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Zone : MonoBehaviour
{
    public UnityEvent onEnter;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            onEnter?.Invoke();
        }
    }
}
