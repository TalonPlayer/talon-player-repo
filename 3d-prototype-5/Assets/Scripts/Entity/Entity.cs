using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class that contains generic info about the entity
/// </summary>
public class Entity : MonoBehaviour
{
    [Header("Info")]
    public string entityName;
    public string entityID;
    public Group group;
    public List<Entity> friends;
    public bool isAlive = true;

    [Header("Stats")]
    public float threatLevel = 50;
    public float speed;
    public float stamina = 100;

    [Header("Components")]
    public EntityBrain brain;
    public EntityMovement movement;
    public EntityBody body;
    public EntityCombat combat;
    public Rigidbody rb;
    public CapsuleCollider cc;
    public Outline outline;
    public UnityEvent onDeath;
    void Awake()
    {


    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        EntityManager.Instance.entities.Add(this);
    }

    public void Init()
    {
        body = GetComponentInChildren<EntityBody>();
        outline = GetComponent<Outline>();

        brain = GetComponent<EntityBrain>();
        movement = GetComponent<EntityMovement>();
        combat = GetComponent<EntityCombat>();
    }

    public bool IsFriend(Entity entity)
    {
        return friends.Contains(entity);
    }

    public void OnDeath()
    {
        EntityManager.Instance.ScheduleRespawn(entityName);

        EntityManager.finishTick += () =>
        {
            EntityManager.Instance.entities.Remove(this);
            EntityManager.Instance.RecycleRagdolls();
            EntityManager.Instance.RecycleItems();
            isAlive = false;
            cc.enabled = false;
            movement.agent.enabled = false;

            foreach (Entity e in EntityManager.Instance.entities)
            {
                if (e.brain.nearbyAllies.Contains(this)) e.brain.nearbyAllies.Remove(this);
                if (e.brain.visibleEnemies.Contains(this)) e.brain.visibleEnemies.Remove(this);

                if (e.brain.target == transform) e.brain.target = null;
                if (e.brain.facingTarget == transform) e.brain.facingTarget = null;
            }

            if (group != null)
            {
                group.members.Remove(this);
            }

            brain.OnDeath();
            movement.OnDeath();
            body.OnDeath();
            DelayDestroy(.25f);
        };
    }

    public void DelayDestroy(float time)
    {
        Invoke(nameof(DestroySelf), time);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    void Respawn()
    {
        
    }

}

public enum SpeedType
{
    Fast = 0,
    Average = 1,
    Slow = 2
}

public enum StaminaType
{
    LongLasting = 0,
    Average = 1,
    EasilyWinded = 2
}