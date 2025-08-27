using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    public Tower towerPrefab;
    public Transform folder;
    public List<Transform> spawns;
    public List<Tower> towers;
    public int towerCount;
    void Awake()
    {
        SpawnTowers();
    }

    /// <summary>
    /// Spawns the towers
    /// </summary>
    public void SpawnTowers()
    {
        for (int i = 0; i < towerCount; i++)
        {
            SpawnTower(spawns[Random.Range(0, spawns.Count)]);
        }
    }

    /// <summary>
    /// Spawn a tower in a random position at a transform
    /// </summary>
    /// <param name="location"></param>
    void SpawnTower(Transform location)
    {
        Vector3 pos = RandExt.RandomPosition(location);
        pos = SnapToGround(pos);

        Tower t = Instantiate(towerPrefab, pos, Quaternion.identity, folder);
        t.Init();

        towers.Add(t);
    }

    /// <summary>
    /// Despawn all the towers
    /// </summary>
    public void DespawnTowers()
    {
        foreach (Tower t in towers)
            t.Despawn();

        towers.Clear();
    }

    /// <summary>
    /// Snap the tower to the floor
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector3 SnapToGround(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos, Vector3.down, out hit, 3.5f, 1 << 6)) // Layer 6 = Ground
        {
            Vector3 groundedPos = pos;
            groundedPos.y = hit.point.y;
            return groundedPos;
        }

        return pos;
    }

}
