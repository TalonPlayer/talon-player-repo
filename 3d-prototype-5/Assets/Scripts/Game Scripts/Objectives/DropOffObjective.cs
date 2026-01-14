using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOffObjective : Objective
{
    public string dropOffID = "OBJ_";
    public List<HeldObjective> depositedObjectives;
    public override void Start()
    {
        base.Start();
    }


    public override void Init()
    {
        onInit?.Invoke();
    }

    /// <summary>
    /// When picked up
    /// </summary>
    public override void OnStart()
    {
        onStart?.Invoke();
    }

    /// <summary>
    /// When entity dies while carrying
    /// </summary>
    public override void OnInterrupt()
    {
        onInterrupt?.Invoke();
    }

    /// <summary>
    /// When entity brings object to goal
    /// </summary>
    public override void OnFinish()
    {
        onFinish?.Invoke();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (other.tag == "Objective")
        {
            HeldObjective o = other.GetComponent<HeldObjective>();
            if (o.objectiveID == dropOffID)
            {
                o.OnFinish();
                OnFinish();
            }
        }
    }
}
