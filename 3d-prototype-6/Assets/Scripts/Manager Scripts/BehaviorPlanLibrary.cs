using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanLibrary : MonoBehaviour
{
    public static PlanLibrary Instance;
    [SerializeField] private PlanPack[] _spawnPlans;
    [SerializeField] private PlanPack[] _searchPlans;
    [SerializeField] private PlanPack[] _combatPlans;
    [SerializeField] private PlanPack[] _retreatPlans;
    private PlanPack[][] planPacks;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        planPacks = new PlanPack[][]{ _spawnPlans, _searchPlans, _combatPlans, _retreatPlans };
    }

    public static Step[] GetPlans(int aiLevel, BehaviorMode planType)
    {
        PlanPack[] planPack = Instance.planPacks[(int) planType];

        planPack = Array.FindAll(planPack, p => aiLevel >= p.requiredAILevel); 

        return Helper.RandomElement(planPack).plan;
    }
}

[System.Serializable]
public class PlanPack
{
    public BehaviorMode planType;
    public Step[] plan;
    [Tooltip("The Level an AI needs to be in order to have access to this plan")]
    public int requiredAILevel;
}


