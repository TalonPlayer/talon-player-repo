using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Wanderers run to safe zones when they feel endangered
public class HumanWanderer : HumanBrain
{
    public Group friendGroup;
    protected override void Start()
    {
        base.Start();
        SafeZoneManager.safeZoneTick += ClosestSafeZone;
        objectiveTarget = Helper.RandomElement(MyEntityManager.Instance.wanderPoints);
        movement.Orbit(objectiveTarget.position);

        if (isLeader)
        {
            friendGroup.AssignLineFormation();
            inFormation = true;
        }
    }

    public override void Brain()
    {
        if (target == null && !isAggro)
        {
            target = objectiveTarget;
            return;
        }

        if (isAggro && isVigilant)
        {
            HumanCombat();
        }
        HumanBehavior();
    }

    public override void OnDeath()
    {
        SafeZoneManager.safeZoneTick -= ClosestSafeZone;
        base.OnDeath();
    }

    public override void HumanBehavior()
    {
        if (target == null)
        {
            movement.strafeSide = 0;
            movement.randomDirection = Helper.RandomVectorInRadius(6f);
            objectiveTarget = Helper.RandomElement(MyEntityManager.Instance.wanderPoints);
            movement.Orbit(objectiveTarget.position);
        }
        if (inSafeZone) // Set a timer for them to go back out
        {
            if (!movement.HasReached(closestSafeZone, 4f))
            {
                movement.MoveTo(closestSafeZone.position);
            }
            else
            {
                movement.StopMovement();
            }

            return;
        }

        if (inDanger || isFleeing) return;

        ObjectiveBehavior();

        if (isWandering)
        {
            if (isLeader)
            {
                if (inFormation) friendGroup.AssignLineFormation();
                if (movement.HasReachedPosition(Random.Range(1f, 5f)))
                {
                    objectiveTarget = Helper.RandomElement(MyEntityManager.Instance.wanderPoints);
                    movement.Orbit(objectiveTarget.position);
                }
                else // Move if not near objective
                    movement.ResumeMovement();

            }
            else if (!isLeader && !inFormation)
            {
                if (movement.HasReachedPosition(Random.Range(1f, 5f)))
                {
                    objectiveTarget = Helper.RandomElement(MyEntityManager.Instance.wanderPoints);
                    movement.Orbit(objectiveTarget.position);
                }
                else // Move if not near objective
                {
                    movement.ResumeMovement();
                }
            }
        }
    }

    public override void HumanCombat()
    {
        if (!facingTarget) return;


        if (objectiveTarget && !inFormation)

        if (!combat.isReloading)
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

    }

    public void ObjectiveBehavior()
    {
        // Logic if doing objective
        if (!atObjective && knowsObjective && !inFormation)
        {
            // If they reached the target within a certain radius, start orbiting/defending
            if (movement.HasReachedPosition(Random.Range(1f, 5f)))
            {
                atObjective = true;
                isDefending = true;
                movement.Orbit(objectiveTarget.position);

            }
            else // Move if not near objective
            {
                movement.ResumeMovement();

            }
            return;
        }
    }
}
