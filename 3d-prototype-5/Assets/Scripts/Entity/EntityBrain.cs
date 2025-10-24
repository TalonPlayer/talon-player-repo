using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Class that holds all generic goals/wants for every entity (humans and zombies)
/// </summary>
public abstract class EntityBrain : MonoBehaviour, IPointerClickHandler
{
    [Header("Vision")]
    public List<Entity> visibleEnemies;
    public List<Entity> nearbyAllies;
    public Transform target;
    public Transform facingTarget;
    public Transform closestSafeZone;
    public Transform objectiveTarget;

    [Header("World States")]
    public bool isHuman;                // Is a human
    public bool isLeader;
    public bool hasGroup;               // Has a group of people nearby
    public bool hasFriends;             // Has friends
    public bool inTempGroup;            // Is temporarily joining group?
    public bool isAggro;                // Sees a threat
    public bool atObjective;            // Is at Objective
    public bool knowsObjective;         // Knows where objective is
    public bool isDefending;
    public bool isFleeing;
    public bool inDanger;
    public bool isVigilant;
    public bool isReady;
    public bool isCharging;
    public bool inSafeZone;
    public bool inFormation;
    public bool isWandering;
    protected bool scoutPause = false;

    public int situation;
    public int fleeThreshold = 75;
    protected int chargeThreshold = 5;

    // Vision
    float visionDistance = 25f;
    int steps = 18;
    float maxOffset = 45f;

    [Header("Social Types")]
    public Experience expLvl;
    public RoleType roleType;
    public ObjectiveType objType;
    public SocialType socialType;
    public BehaviorType behaviorType;

    protected Entity entity;
    protected EntityBody body;
    protected EntityMovement movement;
    protected EntityCombat combat;
    protected Coroutine scoutRoutine;
    void Awake()
    {

    }
    protected virtual void Start()
    {
        entity = GetComponent<Entity>();
        movement = GetComponent<EntityMovement>();
        combat = GetComponent<EntityCombat>();
        EntityManager.visionTick += VisionCheck;
        EntityManager.aggroTick += CheckTarget;
        EntityManager.brainTick += Brain;

        if (objectiveTarget) movement.MoveTo(objectiveTarget.position);
    }

    public virtual void OnDeath()
    {
        EntityManager.visionTick -= VisionCheck;
        EntityManager.aggroTick -= CheckTarget;
        EntityManager.brainTick -= Brain;
    }

    public abstract void Brain();

    #region Scouting Behavior
    public void CancelScout()
    {
        if (scoutRoutine != null) StopCoroutine(scoutRoutine);
    }
    protected IEnumerator ScoutRoutine(float time, Transform target, float radius = 0f)
    {
        yield return new WaitForSeconds(time);

        if (radius > 0f)
            movement.Orbit(target.position, radius);
        else
            movement.Orbit(target.position);

        movement.randomDirection = Helper.RandomVectorInRadius(Random.Range(0f, 5f));
        movement.ToggleAiming(true);
        scoutPause = false;
    }

    #endregion

    protected bool IsFacingTarget()
    {
        if (target == null) return false;

        Vector3 toTarget = (target.transform.position - transform.position).normalized;
        toTarget.y = 0f;

        float dot = Vector3.Dot(transform.forward, toTarget);

        return dot > .0001f; // You can adjust the threshold (1 = perfect alignment)
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.cameraView == CameraView.EntityFacing) return;
        GameManager.Instance.SetCamera(CameraView.EntityFacing, entity);
    }
    public void VisionCheck()
    {
        Vector3 origin = transform.position;
        origin.y += 1f;

        float halfFOV = maxOffset; // degrees
        float distance = visionDistance;

        int count = Mathf.Max(1, steps);
        if (isFleeing && inDanger)
        {
            count /= 2;
            maxOffset /= 2f;
            distance /= 5f;
        }
        foreach (Entity a in nearbyAllies)
        {
            foreach (Entity e in a.brain.visibleEnemies)
            {
                if (!visibleEnemies.Contains(e))
                {
                    visibleEnemies.Add(e);
                    isAggro = true;
                    entity.body.Play("IsAggro", true);
                }
            }
        }
        List<Entity> temp = visibleEnemies.FindAll(e => e == null);

        foreach (Entity e in temp)
        {
            visibleEnemies.Remove(e);
        }

        for (int i = 0; i <= count; i++)
        {
            float t = (float)i / count;
            float yaw = Mathf.Lerp(-halfFOV, halfFOV, t); // -FOV..+FOV

            // Rotate around this entity's local up axis so it follows its Y rotation
            Quaternion yawRot = Quaternion.AngleAxis(yaw, transform.up);
            Vector3 dir = (yawRot * transform.forward).normalized;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, distance))
            {
                if (isHuman)
                {
                    Entity e = hit.collider.GetComponent<Entity>();
                    if (!e) return;

                    if (hit.collider.CompareTag("Zombie") && !visibleEnemies.Contains(e))
                    {
                        visibleEnemies.Add(e);
                        isAggro = true;
                    }
                    else if (hit.collider.CompareTag("Human") && !nearbyAllies.Contains(e))
                    {
                        nearbyAllies.Add(e);
                    }
                    entity.body.Play("IsAggro", true);

                }
                else if (!isHuman)
                {
                    Entity e = hit.collider.GetComponent<Entity>();
                    if (!e) return;

                    if (hit.collider.CompareTag("Human") && !visibleEnemies.Contains(e))
                    {
                        visibleEnemies.Add(e);
                        isAggro = true;
                    }
                    else if (hit.collider.CompareTag("Zombie") && !nearbyAllies.Contains(e))
                    {
                        nearbyAllies.Add(e);
                    }

                    entity.body.Play("IsAggro", true);
                }
            }
        }
    }

    public void CheckTarget()
    {
        List<Entity> availableEnemies = visibleEnemies.FindAll(e => !e.brain.inSafeZone);
        // there are no more units or enemy is no longer alive
        if (availableEnemies.Count == 0 && isAggro)
        {
            isAggro = false;
            entity.body.Play("IsAggro", false);
            return;
        }

        Transform closest = null;
        Vector3 currentPos = transform.position;
        float closestDistanceSqr = Mathf.Infinity;

        // Check to see if there are any units that are closer than the player
        foreach (Entity e in availableEnemies)
        {
            if (e == null || e.brain.inSafeZone) continue;

            float distSqr = (e.transform.position - currentPos).sqrMagnitude;
            if (distSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distSqr;
                closest = e.transform;
            }
        }

        // Target the new unit or player
        if (closest != null)
        {
            if (facingTarget != closest)
            {
                facingTarget = closest;

                if (isAggro && !isHuman)
                {
                    target = closest;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isHuman)
        {
            Entity e = other.GetComponent<Entity>();
            if (!e) return;

            if (other.CompareTag("Zombie"))
            {
                if (visibleEnemies.Contains(e))
                    visibleEnemies.Remove(e);
            }

            if (other.CompareTag("Human"))
            {
                if (nearbyAllies.Contains(e))
                    nearbyAllies.Remove(e);
            }
        }
        else
        {
            Entity e = other.GetComponent<Entity>();
            if (!e) return;
            if (other.CompareTag("Human"))
            {
                if (visibleEnemies.Contains(e))
                    visibleEnemies.Remove(e);
            }

            if (other.CompareTag("Zombie"))
            {
                if (nearbyAllies.Contains(e))
                    nearbyAllies.Remove(e);
            }
        }

    }
}

public enum Experience
{
    Beginner = 0,
    Intermediate = 1,
    Advanced = 2,
    Expert = 3,
    Master = 4
}

public enum RoleType
{
    Leader = 0,     // Evaluates current situation to player.
    Follower = 1,   // Follows the leader and attempts to keep everyone in group alive
    Wanderer = 2,   // Wanders around the map and temporarily joins a group
    Sociable = 3,   // Prioritizes friends over anything else
    LoneWolf = 4,   // Refuses to join other groups and do their own thing
    Opportunist = 5 // Joins teams who are performing the best
}

public enum ObjectiveType
{
    ObjectiveDriven = 0,    // Focuses on objective
    SocialSeeker = 1,       // Focuses on interactions
    Explorer = 2,           // Focuses on exploring
    SelfPresevation = 3     // Focuses on themselves
}

public enum SocialType
{
    Introverted = 0,        // Rarely interacts
    Extroverted = 1,        // Interacts a lot
    Hybrid = 2              // Middle
}

public enum BehaviorType
{
    Aggressive = 0,     // Entity will go for more risky plays
    Cautious = 1,         // Entity will be hesitant on going beyond safe zones
    Resilient = 2,          // Entity doesn't care to what happens to them
    Strategic = 3,     // Entity will be able to encourage anyone to assist
    Oblivious = 4,      // Entity doesn't know whats going on
}



