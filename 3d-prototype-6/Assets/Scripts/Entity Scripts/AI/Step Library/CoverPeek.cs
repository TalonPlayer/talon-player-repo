
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cover Peek", menuName = "AI Steps/Cover Peek", order = 1)]
public class CoverPeek : Step
{
    public override void InitialExecute(Unit u, UnitWorldState s)
    {
        u.movement.MoveToCoverPeek();
    }

    public override Status StepTick(Unit u, UnitWorldState s)
    {
        if (s.CoverBlown)
        {
            u.movement.UnCover();
            return Status.Failure;
        } 
        return Status.Running;
    }

    public override Status UnPausableTick(Unit u, UnitWorldState s)
    {
        if (s.CoverBlown)
        {
            u.movement.UnCover();
            return Status.Failure;
        } 
        return Status.Running;
    }

    public override bool IsComplete(UnitWorldState state)
    {
        return true;
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