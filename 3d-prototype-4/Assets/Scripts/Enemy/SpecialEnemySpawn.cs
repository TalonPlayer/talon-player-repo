using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEnemySpawn : MonoBehaviour
{
    public Enemy enemyPrefab; // The enemy
    public float minInterval;
    public float maxInterval;
    public bool immediateSpawn = false;

    [Header("Zombie Stats")]
    public int minCount;
    public int maxCount;
    public int health;
    public int minSpeed;
    public int maxSpeed;
    Coroutine spawnRoutine;
    void Start()
    {
        if (immediateSpawn)
            ImmediateSpawn();
        else
            spawnRoutine = StartCoroutine(SpawnRoutine(Random.Range(minInterval, maxInterval)));
    }

    /// <summary>
    /// Immediately spawn the enemy
    /// </summary>
    public void ImmediateSpawn()
    {
        spawnRoutine = StartCoroutine(SpawnRoutine(1f));
    }

    IEnumerator SpawnRoutine(float time)
    {
        yield return new WaitForSeconds(time);

        for (int i = 0; i < Random.Range(minCount, maxCount); i++)
            EntityManager.Instance.SpawnEnemy(transform, enemyPrefab, health, minSpeed, maxSpeed, false);

        spawnRoutine = StartCoroutine(SpawnRoutine(Random.Range(minInterval, maxInterval)));
    }

    /// <summary>
    /// Stop Spawning
    /// </summary>
    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
        }
    }

    /// <summary>
    /// Resume Spawning
    /// </summary>
    public void ResumeSpawning()
    {
        spawnRoutine = StartCoroutine(SpawnRoutine(Random.Range(minInterval, maxInterval)));
    }
}
