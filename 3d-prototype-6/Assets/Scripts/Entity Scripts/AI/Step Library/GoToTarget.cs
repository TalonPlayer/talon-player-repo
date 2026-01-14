using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Go To Target", menuName = "AI Steps/Go To Target", order = 1)]
public class GoToTarget : Step
{
    public override void InitialExecute(Unit u, UnitWorldState s)
    {
        //
    }

    public override Status StepTick(Unit u, UnitWorldState s)
    {

        if (s.HasAimOnTarget)
        {
            u.movement.MoveTo(u.TargetPos);
            if (u.movement.HasReachedDestination) return Status.Complete;

        }
        else 
            u.movement.MoveTo(u.vision.targetLastKnownPos);


        return Status.Running;
    }

    public override bool IsComplete(UnitWorldState state)
    {
        return false;
    }

    public override void OnComplete(Unit u)
    {
        // 
    }
    public override void OnFailure(Unit u)
    {
        // Nothing
    }
}