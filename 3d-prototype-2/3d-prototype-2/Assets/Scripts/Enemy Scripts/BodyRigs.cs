using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.UI;
public class BodyRigs : MonoBehaviour
{
    [Header("Rigs")]
    public List<MultiAimConstraint> constraints;
    public RigBuilder rig;
    public Rig headRig;
    void Start()
    {

    }

    public void ActivateRigs()
    {
        foreach (MultiAimConstraint contraint in constraints)
        {
            AddTargetToAimConstraint(contraint, GameManager.Instance.playerHead.transform, 1f);
        }
    }

    void AddTargetToAimConstraint(MultiAimConstraint constraint, Transform newTarget, float weight)
    {
        if (!constraint) return;

        // Create a WeightedTransform
        WeightedTransform weighted = new WeightedTransform
        {
            transform = newTarget,
            weight = weight
        };

        // Create a new WeightedTransformArray with 1 entry
        WeightedTransformArray newArray = new WeightedTransformArray();
        newArray.Add(weighted);

        // Apply to constraint
        constraint.data.sourceObjects = newArray;

        // Set weight of the constraint to activate it
        constraint.weight = 1f;

        rig.Build();
    }

    public void DisableHeadRigs()
    {
        foreach (MultiAimConstraint constraint in constraints)
        {
            constraint.weight = 0f;
        }
        headRig.weight = 0f;
        rig.Build();
    }
}
