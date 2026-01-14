using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeathEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent onDeath;
    public void OnDeath()
    {
        onDeath?.Invoke();
    }
}
