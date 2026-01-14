using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Maintain LOS", menuName = "AI Steps/Maintain LOS", order = 1)]
public class MaintainLOS : Step
{
    public float strafeDistance;
    public override void InitialExecute(Unit u, UnitWorldState s)
    {
        
    }

    public override Status StepTick(Unit u, UnitWorldState s)
    {

        if (!s.HasClearShot)
        {
            u.movement.AnglePeek(strafeDistance, u.TargetPos);
        }

        if (s.TargetAcquired)
            if (!s.TargetAlive) return Status.Failure;

        return Status.Running;
    }

    public override bool IsComplete(UnitWorldState state)
    {
        return false;
    }

    public override void OnComplete(Unit u)
    {
        
    }
    public override void OnFailure(Unit u)
    {
        // Nothing
    }
}