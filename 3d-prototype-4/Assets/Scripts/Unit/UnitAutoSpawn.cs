using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAutoSpawn : MonoBehaviour
{
    public Unit unit;
    public UIPointer pointer;
    void Start()
    {
        Unit u = EntityManager.Instance.SpawnUnit(transform, unit);
        if (pointer) pointer.target = u.transform;

    }
}
