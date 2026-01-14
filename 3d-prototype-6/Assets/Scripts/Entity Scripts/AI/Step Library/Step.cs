using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Step : ScriptableObject
{
    public IntentType IntentType;
    public MoveUrgency MoveUrgency;
    public FailureEffect FailureEffect;
    public bool DelayInit;
    public float MinTimeLimit;
    public float MaxTimeLimit;
    public float MinInitWaitTime;
    public float MaxInitWaitTime;
    public abstract void InitialExecute(Unit u, UnitWorldState s);
    public abstract Status StepTick(Unit u, UnitWorldState s);
    public virtual Status UnPausableTick(Unit u, UnitWorldState s)
    {
        return Status.Running;
    }
    public abstract bool IsComplete(UnitWorldState state);
    public abstract void OnComplete(Unit u);
    public abstract void OnFailure(Unit u);
}

[System.Serializable]
public class RuntimeStep
{
    public string Name;
    public float TimeLimit;
    public float DelayInit;
    public bool EndOfPlan;
    private bool isActive;
    public Coroutine Timer;
    private Unit main;
    private Step step;
    private UnitWorldState state;
    public RuntimeStep(Step s, Unit u, UnitWorldState us)
    {
        main = u;
        step = s;
        state = us;
        TimeLimit = UnityEngine.Random.Range(s.MinTimeLimit, s.MaxTimeLimit);
        Name = s.name;

        if (s.DelayInit)
            DelayInit = UnityEngine.Random.Range(s.MinInitWaitTime, s.MaxInitWaitTime);
        else DelayInit = 0;
    }
    public IntentType Intent { get { return step.IntentType; } }
    public MoveUrgency Movement { get { return step.MoveUrgency; } }
    public FailureEffect FailureEffect { get { return step.FailureEffect; } }

    public void Init()
    {
        step.InitialExecute(main, state);
        isActive = true;
    }

    public Status Tick()
    {
        if (isActive) return step.StepTick(main, state);
        else return step.UnPausableTick(main, state);
    }

    public void OnComplete()
    {
        step.OnComplete(main);
    }

    public void OnFailure()
    {
        step.OnFailure(main);
    }
}

public enum Status
{
    Running,
    Complete,
    Failure,
}

public enum FailureEffect
{
    StepBack,
    Repeat,
    Skip,
    Retreat,
    ResetPlan,
    FullReset,
}

public enum IntentType
{
    Seek,
    Pursue,
    Engage,
    Reposition,
    Defend,
    Anchor,
    Reset
}
public enum MoveUrgency
{
    None,
    Cautious,
    Sprint,
    Crouch,
}


// Step Ideas
/*
    Anchor - Stand still and shoot
    DodgeCover - Defensive Override's way to take cover
    CoverPeek - Peek from cover
    SearchLastKnown - Go to last known target position. Once reached look around
    (I have to change how LOS works for this to work)
    StrafePressure - Move either left or right slightly.
    Fallback to Allies
    AnchorLane - Go to Anchor point and hold for a bit.
    UnstuckNudge - Small movement to get the unit unstuck
*/
