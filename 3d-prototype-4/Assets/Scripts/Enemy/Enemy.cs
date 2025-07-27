using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Enemy : Entity
{
    public Transform target;
    public bool isAggro;
    public bool isSpawning;
    private CapsuleCollider cc;
    public Rigidbody rb;
    public LayerMask deathLayer;
    public ParticleSystem bloodParticles;
    [HideInInspector] public EnemyMovement movement;
    [HideInInspector] public EnemyCombat combat;
    [HideInInspector] public EnemyBody body;
    void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        combat = GetComponent<EnemyCombat>();
        body = GetComponent<EnemyBody>();
        cc = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        EntityManager.healthTick += CheckHealth;
    }

    public void Spawn(float spawnTime)
    {
        body.PlayRandom("IsSpawning", 1, true);
        cc.excludeLayers += 1 << 8;
        isSpawning = true;
        Invoke(nameof(EndSpawn), spawnTime);
    }

    public void EndSpawn()
    {
        isSpawning = false;
        body.Play("IsSpawning", false);
        EntityManager.aggroTick += CheckTarget;
    }

    public void Init(int _health, int _min, int _max)
    {
        health = _health;
        movement.Init(_min, _max);
    }

    void Update()
    {
    }
    public void CheckHealth()
    {
        if (health <= 0.0f)
        {
            isAlive = false;
            OnDeath();
        }
    }

    public void OnDeath()
    {
        EntityManager.aggroTick -= CheckTarget;
        EntityManager.Instance.enemies.Remove(this);

        PlayerManager.Instance.AddScore(100);
        EntityManager.healthTick -= CheckHealth;
        EntityManager.Instance.SpawnSkull(transform.position);
        WorldManager.Instance.UpdateCount();
        body.PlayRandom("IsDead", 8, true);
        body.Play("RandomFloat", Random.Range(.8f, 1.5f));
        isAggro = false;

        cc.excludeLayers += deathLayer;

        onDeath?.Invoke();
    }

    public void DestroySelf(float time)
    {
        Invoke(nameof(DestroySelf), time);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void OnHit(int damage)
    {
        if (bloodParticles != null)
        {
            bloodParticles.Play();
        }

        EntityManager.damageTick += () =>
        {
            health -= damage;
        };
    }

    public void AggroUnit(Transform _target)
    {
        if (!isAlive) return;
        target = _target;
        isAggro = true;
        body.Play("IsSpawning", false);
        body.Play("IsChasing", true);
        body.Play("RandomFloat", Random.Range(0f, 1f));

    }

    public void CheckTarget()
    {
        if (target == null) isAggro = false;
        if (EntityManager.Instance.units.Count == 0 && isAggro || !isAlive) return;

        
        Transform closest = PlayerManager.Instance.player.transform;
        Vector3 currentPos = transform.position;
        float closestDistanceSqr = (closest.position - currentPos).sqrMagnitude;

        foreach (Unit u in EntityManager.Instance.units)
        {
            if (u == null) continue;

            float distSqr = (u.transform.position - currentPos).sqrMagnitude;
            if (distSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distSqr;
                closest = u.transform;
            }
        }


        if (closest != null)
        {
            if (target != closest)
            {
                target = closest;
                AggroUnit(target);
            }
        }
    }
}
