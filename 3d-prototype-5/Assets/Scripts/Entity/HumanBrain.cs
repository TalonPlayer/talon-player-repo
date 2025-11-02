using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBrain : EntityBrain
{
    [SerializeField] protected Vector2 scoutLookInterval = new Vector2(0.8f, 2.0f);
    protected float nextScoutLookTime = 0f;
    protected Vector3 scoutLookDir = Vector3.zero;

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

        // Removed dependency on inDanger / isFleeing flags

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
            if (!movement.HasReached(objectiveTarget, 12f) && scoutPause)
            {
                CancelScout();
                scoutPause = false;
                atObjective = false;
                movement.Orbit(objectiveTarget.position, 3f);
                isDefending = false;
            }

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
                if (!scoutPause) UpdateScoutingLook();
            }
            return;
        }

        if (!atObjective && knowsObjective)
        {
            if (movement.HasReachedPosition(Random.Range(1f, 5f)))
            {
                atObjective = true;
                isDefending = true;
                movement.Orbit(target.position);
            }
            else
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

    protected void UpdateScoutingLook()
    {
        if (movement.HasReachedPosition()) return;

        movement.ToggleAiming(true);

        if (Time.time >= nextScoutLookTime || scoutLookDir.sqrMagnitude < 1e-4f)
        {
            float yaw = Random.Range(0f, 360f);
            scoutLookDir = (Quaternion.Euler(0f, yaw, 0f) * Vector3.forward);
            nextScoutLookTime = Time.time + Random.Range(scoutLookInterval.x, scoutLookInterval.y);
        }

        movement.RotateToTarget(scoutLookDir);
    }

    public void ClosestSafeZone(List<Transform> safeZones)
    {
        if (inSafeZone) return;
        Transform closest = null;
        Vector3 currentPos = transform.position;
        float closestDistanceSqr = Mathf.Infinity;

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
