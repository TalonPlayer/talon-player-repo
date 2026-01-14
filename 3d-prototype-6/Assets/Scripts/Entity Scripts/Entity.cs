using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public string _name;
    public string audioID;
    public int health;
    protected int _maxHealth;
    public bool isAlive = true;
    [Header("Entity Components")]
    public DeathEvent death;
    protected virtual void Awake()
    {
        name = _name;
    }
    protected virtual void Start()
    {
        _maxHealth = health;
    }

    public virtual void OnHit(int damage, Entity attacker)
    {
        EntityManager.onHit += () =>
        {
            health -= damage;
            CheckHealth();
        }; 
    }
    protected void CheckHealth()
    {
        if (isAlive && health <= 0)
        {
            EntityManager.onDeath += OnDeath;
        }
    }
    public virtual void OnDeath()
    {
        isAlive = false;
        death.OnDeath();
    }
}
