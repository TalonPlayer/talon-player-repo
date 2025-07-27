using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Entity : MonoBehaviour
{
    [Header("Stats")]
    public string _name;
    public int health;
    public int damage;
    public bool isAlive;
    public UnityEvent onDeath;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
