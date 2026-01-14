using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public List<Entity> entities;
    public bool canBeCleaned = false;
    public void RemoveAndEvaluate(Entity toBeRemoved)
    {
        entities.Remove(toBeRemoved);
        entities.RemoveAll(e => e == null);
    }
}
