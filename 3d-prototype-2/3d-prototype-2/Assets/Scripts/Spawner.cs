using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int group;
    public bool stationery = false;
    public List<Waypoint> waypoints;
}
