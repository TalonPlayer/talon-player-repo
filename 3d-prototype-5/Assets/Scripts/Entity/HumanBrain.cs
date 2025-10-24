using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBrain : EntityBrain
{
    [SerializeField] protected Vector2 scoutLookInterval = new Vector2(0.8f, 2.0f);
    protected float nextScoutLookTime = 0f;
    protected Vector3 scoutLookDir = Vector3.zero;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        SafeZoneManager.safeZoneTick += ClosestSafeZone;
    }

    public override void OnDeath()
    {
        SafeZoneManager.safeZoneTick -= ClosestSafeZone;
        base.OnDeath();
    }
    public override void Brain()
    {
        if (target == null && !isAggro)
        {
            target = objectiveTarget;
            return;
        }

        movement.ToggleAiming(isAggro);
        HumanEvaluateSituation();

        if (isAggro)
        {
            HumanCombat();
        }
        HumanBehavior();
    }
    public virtual void HumanBehavior()
    {
        if (target == null)
        {
            target = objectiveTarget;
            movement.strafeSide = 0;
            movement.Orbit(target.position);
            return;
        }
        if (inDanger || isFleeing) return;

        if (inSafeZone)
        {
            if (!movement.HasReached(closestSafeZone, 1f))
            {
                movement.MoveTo(closestSafeZone.position);
            }
            else
            {
                movement.StopMovement();
            }

            return;
        }
        float distance = Vector3.Distance(transform.position, objectiveTarget.position);
        if (distance > 15f)
        {
            movement.ChangeSpeed(2);
        }
        else if (distance > 4f)
        {
            movement.ChangeSpeed(1);
        }

        // If is at objective, start defending
        if (isDefending)
        {
            // If the entity is no longer near the objective, while scouting. Orbit again
            if (!movement.HasReached(objectiveTarget, 12f) && scoutPause)
            {
                CancelScout();
                scoutPause = false;
                atObjective = false;
                movement.Orbit(objectiveTarget.position, 3f);
                isDefending = false;
            }

            // If the entity has reached the position while not scouting, stand still then orbit again soon.
            if (movement.HasReachedPosition() && !scoutPause)
            {
                movement.StopMovement();
                scoutPause = true;

                CancelScout();

                if (isAggro)
                    scoutRoutine = StartCoroutine(ScoutRoutine(Random.Range(0f, 2.5f), objectiveTarget, 3f));
                else
                    scoutRoutine = StartCoroutine(ScoutRoutine(Random.Range(1f, 8f), objectiveTarget, 3f));
            }
            else
            {
                // we are currently moving to a scout waypoint -> look around randomly
                if (!scoutPause) UpdateScoutingLook();
            }
            return;
        }

        // If they are not at objective and they know where it is, move towards it
        if (!atObjective && knowsObjective)
        {

            // If they reached the target within a certain radius, start orbiting/defending
            if (movement.HasReachedPosition(Random.Range(1f, 5f)))
            {
                atObjective = true;
                isDefending = true;
                movement.Orbit(target.position);

            }
            else // Move if not near objective
            {
                movement.ResumeMovement();

            }
            return;
        }
    }

    public virtual void HumanCombat()
    {
        if (!facingTarget || movement.moveType == MoveType.Sprinting || (!combat.hasAmmo && combat.throwableCount <= 0)) return;
        if (!combat.isReloading && IsFacingTarget() && combat.hasAmmo && !combat.isThrowing)
        {
            if (combat.currentAmmoCount > 0)
            {
                combat.RangeAttack();
            }
            else
            {
                combat.Reload();
            }
        }
        else if (IsFacingTarget() && !combat.isThrowing && combat.throwableCount <= 0)
        {
            combat.ThrowAttack();
        }

    }

    public virtual void HumanEvaluateSituation()
    {
        situation = GetSituation();
        if (!combat.hasAmmo && combat.throwableCount <= 0) 
        situation += 150;
        if (inSafeZone)
        {
            inDanger = false;
            isFleeing = false;

            if (socialType == 0) isVigilant = true;
            movement.ChangeSpeed(1);
            movement.ToggleAiming(isAggro);
            return;
        }
        // Compare situation value to fleeThreshold
        if (situation >= 15)
        {
            inDanger = false;
            isFleeing = false;
            movement.ChangeSpeed(1);
            movement.ToggleAiming(isAggro);
        }
        else
        {
            isFleeing = false;
            inDanger = false;

            movement.ChangeSpeed(0);

            movement.ToggleAiming(isAggro);
        }
    }
    public int GetSituation()
    {
        int allyCount = 1;
        int enemyCount = 0;
        float totalEnemyDistance = 0f;

        Vector3 pos = transform.position;

        // Check the number of allies
        if (nearbyAllies != null)
        {
            for (int i = 0; i < nearbyAllies.Count; i++)
            {
                var a = nearbyAllies[i];
                if (a == null || a == entity) continue;
                allyCount++;
            }
        }

        // Check the number of visible enemies and get total distance
        if (visibleEnemies != null)
        {
            for (int i = 0; i < visibleEnemies.Count; i++)
            {
                var e = visibleEnemies[i];
                if (e == null) continue;

                enemyCount++;
                totalEnemyDistance += Vector3.Distance(pos, e.transform.position);
            }
        }

        // Get a total distance -> already accumulated; also compute an average
        float avgEnemyDistance = (enemyCount > 0) ? (totalEnemyDistance / enemyCount) : float.PositiveInfinity;

        // Create an equation to get a situation Value
        // Tunables: how strongly to weight counts vs proximity
        const float dangerRadius = 3f; // closer than this is "dangerous"
        float distancePressure = 0f;    // 0..1, higher = closer enemies overall
        if (enemyCount > 0 && !float.IsInfinity(avgEnemyDistance))
            distancePressure = Mathf.Clamp01((dangerRadius - avgEnemyDistance) / dangerRadius);

        // Higher = worse (more likely to flee)

        int value = Mathf.RoundToInt(
            (enemyCount * 15f         // more enemies -> worse
            + distancePressure * 35f) // closer on average -> worse
            - allyCount * 40f        // more allies -> better (reduces score)
        );

        if (visibleEnemies.Count != 0 && value < 0)
        {
            value = 15;
        }
        return value;
    }

    protected void UpdateScoutingLook()
    {
        // only when actually moving toward a scout waypoint
        if (movement.HasReachedPosition()) return;

        movement.ToggleAiming(true); // prevent agent from auto-rotating

        if (Time.time >= nextScoutLookTime || scoutLookDir.sqrMagnitude < 1e-4f)
        {
            float yaw = Random.Range(0f, 360f);
            scoutLookDir = (Quaternion.Euler(0f, yaw, 0f) * Vector3.forward);
            nextScoutLookTime = Time.time + Random.Range(scoutLookInterval.x, scoutLookInterval.y);
        }

        movement.RotateToTarget(scoutLookDir); // your movement method that rotates the body
    }

    public void ClosestSafeZone(List<Transform> safeZones)
    {
        if (inSafeZone) return;
        Transform closest = null;
        Vector3 currentPos = transform.position;
        float closestDistanceSqr = Mathf.Infinity;

        // Check to see if there are any units that are closer than the player
        foreach (Transform zone in safeZones)
        {
            float distSqr = (zone.transform.position - currentPos).sqrMagnitude;
            if (distSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distSqr;
                closest = zone.transform;
            }
        }

        closestSafeZone = closest;
    }
}
