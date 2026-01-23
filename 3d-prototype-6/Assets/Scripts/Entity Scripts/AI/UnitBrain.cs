using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Decide what to do with info from vision and combat
/// </summary>
public class UnitBrain : MonoBehaviour
{
    public Unit main;
    public UnitWorldState state;
    public BehaviorMode behaviorMode;
    public float reactionTime = 0.2f;
    private float _reactionTimer;
    public Step[] spawnPlan;    // Plan used when the unit is spawned, leave empty if theres nothing special
    public Step[] searchPlan;   // Plan used to search for enemies
    public Step[] combatPlan;   // Plan used when in combat, this should be loopable
    public Step[] retreatPlan;  // Plan used for when the unit needs to retreat
    public bool logPlans;
    public TextMeshPro debugPlanText;
    private Step[] currentPlan;
    private RuntimeStep currentStep;
    private bool _timedOut;
    private int _stepIndex = -1;
    private int _timedOutFailures;
    // Caches required references on spawn.
    void Awake()
    {
        main = GetComponent<Unit>();
    }

    // Initializes action set + goals once.
    void Start()
    {
        EntityManager.brainTick += BrainTick;
        state = new UnitWorldState();
    }

    public void Init()
    {
        FullReset();

        debugPlanText.gameObject.SetActive(logPlans);
    }

    private void FullReset()
    {
        if (spawnPlan != null && spawnPlan.Length > 0)
            SwitchPlan(BehaviorMode.Spawn);
        else
            SwitchPlan(BehaviorMode.Search);
    }

    public void OnDeath()
    {
        EntityManager.brainTick -= BrainTick;
    }

    public void BrainTick()
    {
        _reactionTimer -= Time.deltaTime;
        if (_reactionTimer > 0f)
            return;

        _reactionTimer = Random.Range(Time.deltaTime, reactionTime);
        state = main.Tick(state);

        StepTick();

        main.ai.Tick();
    }


    /// <summary>
    /// Go to next step and create a runtime step
    /// </summary>
    public void NextStep()
    {
        if (currentStep != null)
        {
            StopCoroutine(currentStep.Timer);
        }
        _timedOut = false;

        _stepIndex++;

        currentStep = new RuntimeStep(currentPlan[_stepIndex], main, state);

        currentStep.EndOfPlan = _stepIndex == currentPlan.Length - 1;
        if (currentStep.DelayInit > 0)
            currentStep.Timer = StartCoroutine(DelayStep(currentStep.DelayInit));
        else
        {
            currentStep.Timer = StartCoroutine(StepTimer(currentStep.TimeLimit));
            currentStep.Init();
        }

        main.ai.intent = currentStep.Intent;
        main.ai.movementType = currentStep.Movement;

        if (logPlans)
        {
            if (currentStep.EndOfPlan)
                debugPlanText.text = $"Step {_stepIndex} (End): {currentStep.Name}";
            else
                debugPlanText.text = $"Step {_stepIndex}: {currentStep.Name}";
        }
    }

    public void StepTick()
    {
        if (currentStep == null) return;
        Status status = currentStep.Tick();

        if (_timedOut) status = Status.Failure;
        if (status == Status.Running) return;

        ClosestTarget();


        if (status == Status.Complete) // If the status is complete, call OnComplete.
        {
            _timedOutFailures = 0;
            currentStep.OnComplete();
        }
        else if (status == Status.Failure) // If the status is Failure, call Replan.
        {
            _timedOutFailures++;
            currentStep.OnFailure();
            Replan();
            return;
        }

        // If we are not at the end of the plan, go to the next step
        if (!currentStep.EndOfPlan)
        {
            NextStep();
        }
        else // If we are at the end of the plan, resume the default plan if we were on a fallback plan.
        {
            switch (behaviorMode)
            {
                case BehaviorMode.Spawn:    // When the initial plan finishes, go to search
                    SwitchPlan(BehaviorMode.Search);
                    break;
                case BehaviorMode.Search:   // Once target is found, go to combat
                    SwitchPlan(BehaviorMode.Combat);
                    break;
                case BehaviorMode.Combat:   // Loop once target is found, otherwise swap to search mode
                    SwitchPlan(BehaviorMode.Combat);
                    break;
                case BehaviorMode.Retreat:  // Once retreat is complete, go back to searching.
                    SwitchPlan(BehaviorMode.Search);
                    break;
            }
        }
    }


    public void SwitchPlan(BehaviorMode mode)
    {
        behaviorMode = mode;
        List<Step> newPlan = new List<Step>();

        switch (mode)
        {
            case BehaviorMode.Spawn:
                newPlan = spawnPlan.ToList();
                break;
            case BehaviorMode.Search:
                newPlan = searchPlan.ToList();
                break;
            case BehaviorMode.Combat:
                newPlan = combatPlan.ToList();
                break;
            case BehaviorMode.Retreat:
                newPlan = retreatPlan.ToList();
                break;
        }
        currentPlan = (Step[]) newPlan.ToArray().Clone();
        _stepIndex = -1;
        NextStep();
    }

    private void Replan()
    {
        if (_timedOutFailures >= 3)
        {
            FullReset();
            return;
        }
        switch (currentStep.FailureEffect)
        {
            case FailureEffect.StepBack:
                _stepIndex -= 2;
                _stepIndex = Mathf.Max(_stepIndex, -1);
                NextStep();
                return;
            case FailureEffect.Repeat:
                _stepIndex -= 1;
                NextStep();
                return;
            case FailureEffect.Skip:
                NextStep();
                return;
            case FailureEffect.Retreat:
                SwitchPlan(BehaviorMode.Retreat);
                return;
            case FailureEffect.ResetPlan:
                _stepIndex = -1;
                NextStep();
                return;
            case FailureEffect.FullReset:
                FullReset();
                return;
        }
    }

    IEnumerator StepTimer(float timeLimit)
    {
        yield return new WaitForSeconds(timeLimit);

        // Step is timed out
        _timedOut = true;
    }

    IEnumerator DelayStep(float timer)
    {
        yield return new WaitForSeconds(timer);
        currentStep.Timer = StartCoroutine(StepTimer(currentStep.TimeLimit));
        currentStep.Init();
    }

    public void ClosestTarget()
    {
        float closestDist = Mathf.Infinity;
        Entity closestTarget = null;

        foreach (Entity t in main.vision.possibleTargets)
        {
            float dist = Vector3.Distance(transform.position, t.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestTarget = t;
            }
        }

        if (closestTarget != null)
        {
            main.target = closestTarget;
            state.TargetAcquired = true;
        }
        else
        {
            state.TargetAcquired = false;
        }

    }

}

public enum BehaviorMode
{
    Spawn = 0,
    Search = 1,
    Combat = 2,
    Retreat = 3
}