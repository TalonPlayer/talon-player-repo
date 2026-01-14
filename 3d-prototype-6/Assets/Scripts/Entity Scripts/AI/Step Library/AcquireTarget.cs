using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Acquire Target", menuName = "AI Steps/Acquire Target", order = 1)]
public class AcquireTarget : Step
{
    public override void InitialExecute(Unit u, UnitWorldState s)
    {
        u.movement.Orbit(8f, PlayerManager.Instance.PlayerPos);
        u.vision.targetLastKnownPos = u.movement.agent.destination;
    }

    public override Status StepTick(Unit u, UnitWorldState s)
    {
        if (IsComplete(s)) return Status.Complete;
        else return Status.Running;
    }

    public override bool IsComplete(UnitWorldState state)
    {
        return state.HasTargets;
    }

    public override void OnComplete(Unit u)
    {
        u.target = u.vision.possibleTargets[0];
        Vector3 pos = u.TargetPos;
        pos += Helper.RandomVectorInRadius(2f);
        pos.y += u.TargetPos.y;
        u.vision.targetLastKnownPos = pos;
    }
    public override void OnFailure(Unit u)
    {
        // Nothing
    }
}