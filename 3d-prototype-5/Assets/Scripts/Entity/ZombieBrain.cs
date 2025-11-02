using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBrain : EntityBrain
{
    public bool isSurrounding = false;
    Coroutine surroundRoutine;

    protected override void Start()
    {
        base.Start();
    }

    public override void Brain()
    {
        if (target == null && !isAggro)
        {
            isDefending = false;
            target = objectiveTarget;
            return;
        }

        movement.ToggleAiming(isAggro);

        if (isAggro)
        {
            if (!isReady)
            {
                if (!isSurrounding)
                {
                    if (scoutRoutine != null) StopCoroutine(scoutRoutine);
                    scoutRoutine = StartCoroutine(SurroundRoutine());
                    isSurrounding = true;
                    movement.Surrounding();
                }
                SurroundBehavior();
            }
            else
            {
                isSurrounding = false;
                ZombieHostileBehavior();
            }
        }
        else
        {
            ZombiePassiveBehavior();
        }
    }

    IEnumerator SurroundRoutine()
    {
        yield return new WaitForSeconds(Random.Range(4f,6f));
        isReady = true;
        isSurrounding = false;
    }

    public void SurroundBehavior()
    {
        if (movement.HasReachedPosition(6f))
        {
            movement.Surrounding();
        }
    }

    public override void OnDeath()
    {
        if (scoutRoutine != null)
            StopCoroutine(scoutRoutine);

        base.OnDeath();
    }

    /// <summary>
    /// Run at humans
    /// </summary>
    public void ZombieHostileBehavior()
    {
        if (target == null)
        {
            target = objectiveTarget;
            movement.Orbit(objectiveTarget.position);
            return;
        }

        // If strategic or cautious, strafe first until reaching a distance.
        // When reaching distance run straight

        // If aggressive, resilient, or oblivious just run straight
        movement.MoveTo(target.position);

        ZombieEvaluateSituation();

        if (movement.HasReached(target.position, combat.attackRange))
        {
            combat.MeleeAttack();

            if (movement.HasReached(target.position, .75f))
            {
                MyEntity e = target.GetComponent<MyEntity>();
                if (e)
                    if (e.isAlive && !e.brain.inSafeZone) e.OnDeath();
            }
        }
        else
            combat.StopAttack();
    }

    /// <summary>
    /// Zombie going to objective if there are no enemies
    /// </summary>
    public void ZombiePassiveBehavior()
    {
        movement.ChangeSpeed(0);
        combat.StopAttack();

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
            if (!movement.HasReached(objectiveTarget, 6f) && scoutPause)
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
            if (movement.HasReached(objectiveTarget, 6f))
            {
                atObjective = true;
                isDefending = true;
                movement.Orbit(objectiveTarget.position, 6f);
                
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

    public void ZombieEvaluateSituation()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= 25f)
        {
            movement.ChangeSpeed(2);

            if (distance <= 15f && !isCharging)
            {
                isCharging = true;
                foreach (MyEntity e in nearbyAllies)
                {
                    if (e == null) continue;
                    if (!e.isAlive) continue;

                    e.brain.isCharging = true;
                    e.movement.agent.speed = e.movement.panicSpeed;
                }
            }

            if (isCharging)
                movement.agent.speed = movement.panicSpeed;

            movement.StrafeForLineOfSight("Zombie", Random.Range(1, 8f));
        }
        else
        {
            movement.ChangeSpeed(1);
            isCharging = false;
        }
    }
}