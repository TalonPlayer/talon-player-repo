using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Class that holds all generic goals/wants for every entity (humans and zombies)
/// </summary>
public abstract class EntityBrain : MonoBehaviour, IPointerClickHandler
{
    [Header("Vision")]
    public List<MyEntity> visibleEnemies;
    public List<MyEntity> nearbyAllies;
    public Transform target;
    public Transform facingTarget;
    public Transform closestSafeZone;
    public Transform objectiveTarget;
    public Vector3 patrolPos;

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
    [SerializeField] protected bool scoutPause = false;

    public int situation;
    public int fleeThreshold = 75;
    protected int chargeThreshold = 5;

    [Header("Social Types")]
    public Experience expLvl;
    public RoleType roleType;
    public ObjectiveType objType;
    public SocialType socialType;
    public BehaviorType behaviorType;

    protected MyEntity entity;
    protected EntityBody body;
    protected EntityMovement movement;
    protected EntityCombat combat;
    protected Coroutine scoutRoutine;

    public float distanceToTarget;

    void Awake()
    {

    }

    protected virtual void Start()
    {
        entity = GetComponent<MyEntity>();
        movement = GetComponent<EntityMovement>();
        combat = GetComponent<EntityCombat>();

        MyEntityManager.aggroTick += CheckTarget;
        MyEntityManager.brainTick += Brain;

        if (objectiveTarget) movement.MoveTo(objectiveTarget.position);
    }

    public virtual void OnDeath()
    {
        MyEntityManager.aggroTick -= CheckTarget;
        MyEntityManager.brainTick -= Brain;
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
            movement.Orbit(objectiveTarget.position, radius);
        else
            movement.Orbit(objectiveTarget.position);

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
        if (PlayerManager.Instance.cameraView != CameraView.TopDown) return;

        if (PlayerManager.Instance.controlMode == ControlMode.ActionMode)
            PlayerManager.Instance.SelectEntity(entity, Input.GetKey(KeyCode.LeftShift));
        else
            PlayerManager.Instance.SetCamera(CameraView.EntityFacing, entity);
    }
    public void CheckTarget()
    {
        // there are no more units or enemy is no longer alive
        if (visibleEnemies.Count == 0 && isAggro)
        {
            isAggro = false;
            entity.body.Play("IsAggro", false);
            return;
        }

        Transform closest = null;
        Vector3 currentPos = transform.position;
        float closestDistanceSqr = Mathf.Infinity;

        // Check to see if there are any units that are closer than the player
        foreach (MyEntity e in visibleEnemies)
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

    // Trigger enter kept lean
    void OnTriggerEnter(Collider other)
    {
        if (!other) return;

        MyEntity e = other.GetComponent<MyEntity>();
        if (!e || e == entity || !e.isAlive) return;

        bool isEnemy = isHuman ? other.CompareTag("Zombie") : other.CompareTag("Human");
        if (!isEnemy)
        {
            if (nearbyAllies != null && !nearbyAllies.Contains(e))
            {
                nearbyAllies.Add(e);
            }
            return;
        }

        if (visibleEnemies != null && !visibleEnemies.Contains(e))
        {
            visibleEnemies.Add(e);
        }

        if (!isAggro)
        {
            isAggro = true;
            if (entity)
                entity.body.Play("IsAggro", true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other) return;

        MyEntity e = other.GetComponent<MyEntity>();
        if (!e || e == entity || !e.isAlive) return;

        bool isEnemy = isHuman ? other.CompareTag("Zombie") : other.CompareTag("Human");
        if (!isEnemy)
        {
            if (nearbyAllies != null)
            {
                for (int i = nearbyAllies.Count - 1; i >= 0; i--)
                {
                    if (ReferenceEquals(nearbyAllies[i], e))
                    {
                        nearbyAllies.RemoveAt(i);
                        break;
                    }
                }
            }
            return;
        }

        if (visibleEnemies != null)
        {
            for (int i = visibleEnemies.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(visibleEnemies[i], e))
                {
                    visibleEnemies.RemoveAt(i);
                    break;
                }
            }
            // If that was the last enemy, drop aggro & anim
            if (visibleEnemies.Count == 0 && isAggro)
            {
                isAggro = false;
                facingTarget = null;
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
    Aggressive = 0,     // MyEntity will go for more risky plays
    Cautious = 1,         // MyEntity will be hesitant on going beyond safe zones
    Resilient = 2,          // MyEntity doesn't care to what happens to them
    Strategic = 3,     // MyEntity will be able to encourage anyone to assist
    Oblivious = 4,      // MyEntity doesn't know whats going on
}
