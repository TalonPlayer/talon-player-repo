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

        // If is at objective, start defending
        if (isDefending)
        {
            // If the entity has reached the position while not scouting, stand still then orbit again soon.
            if (movement.HasReachedPosition() && !scoutPause)
            {
                movement.StopMovement();
                scoutPause = true;

                CancelScout();

                scoutRoutine = StartCoroutine(ScoutRoutine(Random.Range(5f, 10f), objectiveTarget));
            }

            // If the entity is no longer near the objective, while scouting. Orbit again
            if (!movement.HasReached(objectiveTarget, baseOrbitRadius) && scoutPause)
            {
                CancelScout();
                scoutPause = false;
                atObjective = false;
                movement.Orbit(objectiveTarget.position);
                isDefending = false;
            }
            return;
        }

        // If they are not at objective and they know where it is, move towards it
        if (!atObjective && knowsObjective)
        {
            
            // If they reached the target within a certain radius, start orbiting/defending
            if (movement.HasReached(objectiveTarget, orbitRadius))
            {
                atObjective = true;
                isDefending = true;
                movement.Orbit(objectiveTarget.position, baseOrbitRadius);
                
            }
            else // Move if not near objective
            {
                if (movement.targetPos != objectiveTarget.transform.position)
                    movement.targetPos = objectiveTarget.transform.position;

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
            else if (combat.hasAmmo)
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
