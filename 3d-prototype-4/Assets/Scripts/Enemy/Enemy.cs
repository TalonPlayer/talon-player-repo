using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : Entity
{
    public Transform target; // The Target

    // Boolean Checks
    public bool isAggro;
    public bool isSpawning;

    // Components
    private CapsuleCollider cc;
    public Rigidbody rb;
    public LayerMask deathLayer;
    public ParticleSystem bloodParticles;
    public int killValue = 100;
    public int deathSoundChance = 4; // 1 is guaranteed, 4 means 33% chance
    public bool contributeToCount = true; // Contributes to the zombie count
    public bool immortal = false; // Is killable?
    public bool isTargeted = false;
    [Header("Sounds")]
    public List<AudioClip> deathSounds;
    public List<AudioClip> damageSounds;
    public List<AudioClip> ambientSounds;
    public AudioClip skullSpawn;
    public AudioSource audioSource;
    public AudioSource damageAudio;
    [Header("Explosive Type")]
    public ParticleSystem explosiveParticles;
    public float explosionRadius = 3f;
    public int explosionDamage = 65;
    [Header("Child Components")]
    [HideInInspector] public EnemyMovement movement;
    [HideInInspector] public EnemyCombat combat;
    [HideInInspector] public EnemyBody body;
    Player killedBy;
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
        // Check the enemy's health at the health tick
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

        if (Random.Range(0, 11) == 0)
            PlayZombieNoise(false);

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

    /// <summary>
    /// Plays a random zombie noise
    /// </summary>
    /// <param name="isDeath">Plays Death Noise</param>
    public void PlayZombieNoise(bool isDeath)
    {
        audioSource.pitch = Random.Range(.65f, 1.6f);
        if (isDeath)
            audioSource.PlayOneShot(RandExt.RandomElement(deathSounds));
        else
            audioSource.PlayOneShot(RandExt.RandomElement(ambientSounds));
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
            OnDeath();
        }
    }

    /// <summary>
    /// What happens when the enemy dies
    /// </summary>
    public void OnDeath()
    {
        if (immortal) return;
        isAlive = false;
        if (Random.Range(0, deathSoundChance) == 0)
            PlayZombieNoise(true);
        // Don't allow enemy to check their target
        EntityManager.aggroTick -= CheckTarget;

        EntityManager.Instance.enemies.Remove(this);

        // Clean up scene
        EntityManager.Instance.RecycleRagdolls();
        EntityManager.Instance.RecycleDrops();

        if (killedBy != null)
        {
            // Reward player score
            killedBy.stats.AddScore(killValue);
            killedBy.info.kills++;
        }


        // Don't check health since already dead    
        EntityManager.healthTick -= CheckHealth;

        // Attempt to spawn a skull
        EntityManager.Instance.SpawnSkull(transform.position);

        // Update the count of zombies if it contributes
        if (contributeToCount)
            WorldManager.Instance.UpdateCount();

        if (_name == "Entity")
            GlobalSaveSystem.AddAchievementProgress("entity_kills", 1);


        // Death animation
        body.PlayRandom("IsDead", 8, true);
        body.Play("RandomFloat", Random.Range(.8f, 1.5f));
        isAggro = false;

        // Ignore some layers when dying
        cc.excludeLayers += deathLayer;
        cc.enabled = false;
        rb.useGravity = false;
        gameObject.layer = 1 << 2;

        onDeath?.Invoke();
    }

    /// <summary>
    /// Enemy Explodes and damages within the radius
    /// </summary>
    public void Explode()
    {
        explosiveParticles.Play();

        // Get the radius
        float r2 = explosionRadius * explosionRadius;

        // Enemy and Unit Layers
        int layerMask = (1 << 8) | (1 << 15) | (1 << 7);

        // Detect what Enemy and Units are within the radius
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);

        // Damage the Colliders if they are an enemy or unit
        foreach (var h in hits)
        {
            if (h == null) continue;

            Player p = h.GetComponent<Player>();
            if (!p.stats.isImmune && p.isAlive)
                p.KillPlayer();

            // Try Enemy
            Enemy e = h.GetComponentInParent<Enemy>();
            if (e != null && e != this && e.isAlive)
            {
                e.OnHit(explosionDamage);
                if (e.health < explosionDamage)
                    GlobalSaveSystem.AddAchievementProgress("electric_kills", 1);
                continue;
            }

            // Try Unit
            Unit u = h.GetComponentInParent<Unit>();
            if (u != null && u.isAlive)
            {
                u.OnHit(explosionDamage);
                continue;
            }
        }
    }

    /// <summary>
    /// Delay the destroy so that death animations and sounds can play
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
        damageAudio.PlayOneShot(RandExt.RandomElement(damageSounds));

        // Subscribe the damage taken so that all damage can happen on the same tick
        EntityManager.damageTick += () =>
        {
            health -= damage;
        };
    }

    /// <summary>
    /// Checks if the enemy will die to the damage
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public bool IsKilled(int damage, string owner)
    {
        Debug.Log(owner);
        if (health <= damage)
        {
            killedBy = PlayerManager.Instance.players.Find(p => p._name == owner);
            Debug.Log(killedBy._name);
            return true;
        }
        return false;
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
        Transform closest = null;
        Vector3 currentPos = transform.position;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (Player p in PlayerManager.Instance.players)
        {
            if (p == null || !p.isAlive) continue;

            float distSqr = (p.transform.position - currentPos).sqrMagnitude;
            if (distSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distSqr;
                closest = p.transform;
            }
        }

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
