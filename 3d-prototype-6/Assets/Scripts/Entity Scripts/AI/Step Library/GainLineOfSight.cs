using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gain LOS", menuName = "AI Steps/Gain LOS", order = 1)]
public class GainLOS : Step
{
    public override void InitialExecute(Unit u, UnitWorldState s)
    {
        u.movement.MoveTo(u.TargetPos);
    }

    public override Status StepTick(Unit u, UnitWorldState s)
    {
        u.movement.MoveTo(u.TargetPos);

        if (IsComplete(s)) return Status.Complete;
        else return Status.Running;
    }

    public override bool IsComplete(UnitWorldState state)
    {
        return state.HasClearShot;
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