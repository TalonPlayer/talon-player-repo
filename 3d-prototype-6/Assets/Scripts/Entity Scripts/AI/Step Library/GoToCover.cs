using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Go To Cover", menuName = "AI Steps/Go To Cover", order = 1)]
public class GoToCover : Step
{
    public override void InitialExecute(Unit u, UnitWorldState s)
    {
        if (s.HasCoverNearby)
            u.movement.MoveToCover();
    }

    public override Status StepTick(Unit u, UnitWorldState s)
    {
        if (u.movement.coverPoint == null )
        {
            u.movement.MoveToCover();
            return Status.Running;
        }

        if (!s.HasCoverNearby)
            return Status.Failure;
            
        if (u.movement.HasReachedDestination)
        {
            u.movement.InCover();
            return Status.Complete;
        }

        return Status.Running;
    }

    public override bool IsComplete(UnitWorldState state)
    {
        return state.InCover;
    }

    public override void OnComplete(Unit u)
    {
        
    }

    public override void OnFailure(Unit u)
    {
        // Nothing
    }
}