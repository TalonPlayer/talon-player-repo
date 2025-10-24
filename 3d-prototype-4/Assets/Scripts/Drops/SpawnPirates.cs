using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPirates : MonoBehaviour
{
    // Specifically for the Pirate Powerup
    public List<Unit> pirates;
    public Transform ship;
    public void Spawn()
    {
        StartCoroutine(SpawnRoutine());
    }
    
    /// <summary>
    /// Spawns the pirate individually instead of one at a time.
    /// </summary>
    /// <returns></returns>
    public IEnumerator SpawnRoutine()
    {
        foreach (Unit u in pirates)
        {
            yield return StartCoroutine(WaitTime(u));
        }
    }

    public IEnumerator WaitTime(Unit u)
    {
        yield return new WaitForSeconds(.075f);
        Vector3 pos = ship.position;
        Vector3 rand = RandExt.RandomDirection(0f, 360f);
        rand.y = 0f;
        pos += rand;
        EntityManager.Instance.SpawnUnit(pos, u);
    }

    // Destroy the ship
    public void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
