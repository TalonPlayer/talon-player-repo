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
    private Coroutine teleportRoutine;
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
    public bool WillDie(int damage)
    {
        return health - damage <= 0;
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

    /// <summary>
    /// Teleport the entity to a position and influence rotation. Calls a coroutine to make sure it happens.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rot">Put in Quaternion.Identity if you don't want rotation</param>
    public void Teleport(Vector3 pos, Quaternion rot)
    {
        if (teleportRoutine != null) StopCoroutine(teleportRoutine);
        teleportRoutine = StartCoroutine(TPRoutine(pos, rot));
    }

    IEnumerator TPRoutine(Vector3 p, Quaternion r)
    {
        float t = 0f;
        float maxTime = .25f;
        bool unsuccessful = true;
        do
        {
            t += Time.deltaTime / maxTime;
            if (t >= 1f) break;

            transform.position = p;
            transform.rotation = r;

            bool posMatch = transform.position.Equals(p);
            bool rotMatch = transform.rotation.Equals(r);

            unsuccessful = !posMatch && !rotMatch;
            yield return null;
        } while(unsuccessful);
    }

}
