using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Anchor Here", menuName = "AI Steps/Anchor Here", order = 1)]
public class AnchorHere : Step
{
    public override void InitialExecute(Unit u, UnitWorldState s)
    {
        u.movement.ToggleMovement(false);
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
        u.movement.ToggleMovement(true);
    }
    public override void OnFailure(Unit u)
    {
        u.movement.ToggleMovement(true);
    }
}