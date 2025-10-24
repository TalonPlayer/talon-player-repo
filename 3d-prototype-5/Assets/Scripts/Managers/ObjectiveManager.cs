using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;
    public List<Objective> objectives;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //GroupManager.Instance.MoveToObjective(
        //    GroupManager.Instance.groups[0], objectives[0]
        //);
    }
}
