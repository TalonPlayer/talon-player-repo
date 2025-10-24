using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSoldier : HumanBrain
{
    public Group clan;
    protected override void Start()
    {
        base.Start();
    }

    public override void OnDeath()
    {
        base.OnDeath();
    }

    public override void HumanBehavior()
    {
        if (target == null)
        {
            target = objectiveTarget;
            movement.strafeSide = 0;
            movement.Orbit(target.position);
            return;
        }

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
                movement.Orbit(objectiveTarget.position);
                isDefending = false;
            }

            // If the entity has reached the position while not scouting, stand still then orbit again soon.
            if (movement.HasReachedPosition() && !scoutPause)
            {
                movement.StopMovement();
                scoutPause = true;

                CancelScout();

                if (isAggro)
                    scoutRoutine = StartCoroutine(ScoutRoutine(Random.Range(0f, 3.5f), objectiveTarget));
                else
                    scoutRoutine = StartCoroutine(ScoutRoutine(Random.Range(1f, 8f), objectiveTarget));
            }
            return;
        }

        // If they are not at objective and they know where it is, move towards it
        if (!atObjective && knowsObjective)
        {
            
            // If they reached the target within a certain radius, start orbiting/defending
            if (movement.HasReachedPosition(Random.Range(3f, 5f)))
            {
                atObjective = true;
                isDefending = true;
                movement.Orbit(target.position, 4f);
                
            }
            else // Move if not near objective
            {
                movement.ResumeMovement();

            }
            return;
        }
    }

    public override void HumanCombat()
    {
        if (!facingTarget || movement.moveType == MoveType.Sprinting || (!combat.hasAmmo && combat.throwableCount <= 0)) return;
        if (!combat.isReloading && !combat.isThrowing)
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
        else if (!combat.isThrowing && combat.throwableCount >= 0)
        {
            combat.ThrowAttack();
        }

    }
    
}
