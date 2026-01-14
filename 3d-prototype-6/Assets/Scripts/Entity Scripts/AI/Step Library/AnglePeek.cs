using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Angle Peek", menuName = "AI Steps/Angle Peek", order = 1)]
public class AnglePeek : Step
{
    public float strafeDistance;
    public override void InitialExecute(Unit u, UnitWorldState s)
    {
        u.movement.AnglePeek(strafeDistance, u.TargetPos);
    }

    public override Status StepTick(Unit u, UnitWorldState s)
    {
        if (IsComplete(s)) return Status.Complete;
        return Status.Running;
    }

    public override bool IsComplete(UnitWorldState state)
    {
        return state.HasClearShot;
    }

    public override void OnComplete(Unit u)
    {
        u.ai.orientation = FacingMode.FaceTarget;
    }

    public override void OnFailure(Unit u)
    {
        u.ai.orientation = FacingMode.FaceMoveDirection;
    }
}