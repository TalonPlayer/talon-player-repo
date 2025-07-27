using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Entity
{
    public Transform target;
    public bool isAggro;
    public bool isSpawning;
    private CapsuleCollider cc;
    public Rigidbody rb;
    public LayerMask deathLayer;
    public ParticleSystem bloodParticles;
    [HideInInspector] public UnitMovement movement;
    [HideInInspector] public UnitCombat combat;
    [HideInInspector] public UnitBody body;
    [Header("Animation Counts")]
    public int spawnCount;
    public int attackCount;
    public int deathCount;
    void Awake()
    {
        movement = GetComponent<UnitMovement>();
        combat = GetComponent<UnitCombat>();
        body = GetComponent<UnitBody>();
        cc = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        EntityManager.healthTick += CheckHealth;
    }

    public void Spawn(float spawnTime)
    {
        body.PlayRandom("IsSpawning", spawnCount, true);
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
        EntityManager.Instance.units.Remove(this);
        EntityManager.aggroTick -= CheckTarget;

        EntityManager.healthTick -= CheckHealth;
        body.PlayRandom("IsDead", deathCount, true);
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
        if (!isAlive) return;
        float closestDistanceSqr = Mathf.Infinity;
        Transform closest = null;
        Vector3 currentPos = transform.position;

        foreach (Enemy e in EntityManager.Instance.enemies)
        {
            if (e == null || !e.isAlive) continue;

            float distSqr = (e.transform.position - currentPos).sqrMagnitude;
            if (distSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distSqr;
                closest = e.transform;
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
