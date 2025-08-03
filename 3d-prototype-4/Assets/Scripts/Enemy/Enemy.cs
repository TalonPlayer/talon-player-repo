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

    /// <summary>
    /// Spawn the enemy for the given time
    /// </summary>
    /// <param name="spawnTime"></param>
    public void Spawn(float spawnTime)
    {
        // Play the animation of spawning and ignore other enemy colliders
        body.PlayRandom("IsSpawning", 1, true);
        cc.excludeLayers += 1 << 8;
        isSpawning = true;
        Invoke(nameof(EndSpawn), spawnTime);
    }

    /// <summary>
    /// When the spawn ends
    /// </summary>
    public void EndSpawn()
    {
        isSpawning = false;
        body.Play("IsSpawning", false);
        EntityManager.aggroTick += CheckTarget;
    }

    /// <summary>
    /// Initiate the enemy health and speed
    /// </summary>
    /// <param name="_health"></param>
    /// <param name="_min"></param>
    /// <param name="_max"></param>
    public void Init(int _health, int _min, int _max)
    {
        health = _health;
        movement.Init(_min, _max);
    }

    void Update()
    {
    }
    
    /// <summary>
    /// Check to see if the enemy is alive
    /// </summary>
    public void CheckHealth()
    {
        if (health <= 0.0f)
        {
            isAlive = false;
            OnDeath();
        }
    }

    /// <summary>
    /// What happens when the enemy dies
    /// </summary>
    public void OnDeath()
    {
        // Don't allow enemy to check their target
        EntityManager.aggroTick -= CheckTarget;
        EntityManager.Instance.enemies.Remove(this);

        // Reward player score
        PlayerManager.Instance.AddScore(100);

        // Don't check health since already dead
        EntityManager.healthTick -= CheckHealth;

        // Attempt to spawn a skull
        EntityManager.Instance.SpawnSkull(transform.position);

        // Changet the count of zombies
        WorldManager.Instance.UpdateCount();

        // Death animation
        body.PlayRandom("IsDead", 8, true);
        body.Play("RandomFloat", Random.Range(.8f, 1.5f));
        isAggro = false;

        // Ignore some layers when dying
        cc.excludeLayers += deathLayer;

        onDeath?.Invoke();
    }

    /// <summary>
    /// Delay the destroy
    /// </summary>
    /// <param name="time"></param>
    public void DestroySelf(float time)
    {
        Invoke(nameof(DestroySelf), time);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// When the enemy gets hit
    /// </summary>
    /// <param name="damage"></param>
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

    /// <summary>
    /// Aggro to a specific target
    /// </summary>
    /// <param name="_target"></param>
    public void AggroUnit(Transform _target)
    {
        if (!isAlive) return;
        target = _target;
        isAggro = true;
        body.Play("IsSpawning", false);
        body.Play("IsChasing", true);
        body.Play("RandomFloat", Random.Range(0f, 1f));

    }

    /// <summary>
    /// Check to see if the target is still alive
    /// </summary>
    public void CheckTarget()
    {
        // Target was destroyed so not aggro
        if (target == null) isAggro = false;

        // there are no more units or enemy is no longer alive
        if (EntityManager.Instance.units.Count == 0 && isAggro || !isAlive) return;

        // Default closest is the player
        Transform closest = PlayerManager.Instance.player.transform;
        Vector3 currentPos = transform.position;
        float closestDistanceSqr = (closest.position - currentPos).sqrMagnitude;

        // Check to see if there are any units that are closer than the player
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

        // Target the new unit or player
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
