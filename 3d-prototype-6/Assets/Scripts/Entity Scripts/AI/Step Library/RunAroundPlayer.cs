using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Run Around Player", menuName = "AI Steps/Run Around Player", order = 1)]
public class RunAroundPlayer : Step
{
    public override void InitialExecute(Unit u, UnitWorldState s)
    {
        u.movement.Orbit(25f, PlayerManager.Instance.PlayerPos);
    }

    public override Status StepTick(Unit u, UnitWorldState s)
    {
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