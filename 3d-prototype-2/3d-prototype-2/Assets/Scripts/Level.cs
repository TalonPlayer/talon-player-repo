using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Level : MonoBehaviour
{
    [Header("Level Info")]
    public Transform spawnpoint;
    public Transform enemyObj;
    public Transform enemyWaypointsObj;
    public List<Spawner> spawns;
    public List<GameObject> uniqueObjects;
    public UnityEvent onInit;
    public UnityEvent onComplete;
    public UnityEvent onReset;
    public bool hasEnemies;
    public bool cutscene;
    public bool completed;
    public bool isConnected = true;
    public bool finishInit = false;

    /// <summary>
    /// Initialize the Level
    /// </summary>
    public void Init()
    {
        onInit?.Invoke();
        if (!isConnected) WorldController.Instance.Respawn();
        if (cutscene)
        {
            GameManager.Instance.player.CanFight(false);
        }
        else
        {
            GameManager.Instance.player.CanFight(true);
        }

        if (hasEnemies)
        {
            UnpackSpawners();
            WorldController.Instance.currentEnemies = SpawnEnemies();
        }

        finishInit = true;
    }

    public void Destroy()
    {
        foreach (GameObject obj in uniqueObjects)
        {
            obj.SetActive(false);
        }
    }

    /// <summary>
    /// Spawns the enemies
    /// </summary>
    public List<Enemy> SpawnEnemies()
    {
        List<Enemy> currentEnemies = new List<Enemy>();
        if (completed) return currentEnemies;

        foreach (Spawner s in spawns)
        {
            GameObject obj = Instantiate(
                s.enemyPrefab,
                s.transform.position,
                s.transform.rotation,
                WorldController.Instance.transform);
            Enemy e = obj.GetComponent<Enemy>();
            e.aggroGroup = s.group;
            e.movement.Init(s.stationery, s.waypoints);
            currentEnemies.Add(e);
        }

        return currentEnemies;
    }

    public void UnpackSpawners()
    {
        spawns.Clear();
        for (int i = 0; i < enemyObj.childCount; i++)
        {
            Spawner s = enemyObj.GetChild(i).GetComponent<Spawner>();
            for (int j = 0; j < s.transform.childCount; j++)
            {
                Waypoint w = s.transform.GetChild(j).GetComponent<Waypoint>();
                s.waypoints.Add(w);
            }
            spawns.Add(s);
        }
    }

    public void Complete()
    {
        completed = true;
        onComplete?.Invoke();
    }
}
