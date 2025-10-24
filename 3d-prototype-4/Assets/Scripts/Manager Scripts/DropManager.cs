using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedDrop // A class to hold Drop Instance information that a list can't hold
{
    public Drop drop;
    public string name;
    public float weight;
    public int activeCount = 0;
    public WeightedDrop(Drop _drop, float _weight)
    {
        drop = _drop;
        weight = _weight;
        activeCount = 0;
    }
}

public class DropManager : MonoBehaviour
{
    public static DropManager Instance;
    [Header("Drops")]
    public List<Drop> dropPool;
    public List<GameObject> dropLocations;
    public Transform dropFolder;
    public Transform collectLocation;
    public DropObject dropPrefab;
    public Animator dropAreaAnimator;
    public int batchAmount = 3;

    [Header("Numbers")]
    public float spawnIntervalMin = 2; // Minimum time between spawning batches
    public float spawnIntervalMax = 10; // Maximum time
    public float spawnInBetweenTime = .15f; // In between spawning each drop
    public int spawnMin = 8; // Min number of drops to be spawned in a batch
    public int spawnMax = 14; // Max

    private Coroutine spawnRoutine;
    private float totalWeight;
    private List<WeightedDrop> weightedDrops = new List<WeightedDrop>();
    private Dictionary<Drop, WeightedDrop> dropToWeight = new Dictionary<Drop, WeightedDrop>();
    private bool powerupSpawned;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        batchAmount += PlayerManager.Instance.numOfPlayers;
        StartRound();
    }

    public void StartRound()
    {
        GetWeights();
        DelayBurst(Random.Range(2, 5));
        Invoke(nameof(StartSpawnRoutine), Random.Range(2, 5));
    }
    void GetWeights()
    {
        weightedDrops.Clear();
        dropToWeight.Clear();
        totalWeight = 0;

        // Add raw weights
        foreach (Drop d in dropPool)
        {
            float weight = 1f / d.rarity;
            totalWeight += weight;
            WeightedDrop wd = new WeightedDrop(d, weight);
            wd.name = d._name;
            weightedDrops.Add(wd);
            dropToWeight[d] = wd;
        }

        // Normalize weights only ONCE
        foreach (WeightedDrop w in weightedDrops)
        {
            w.weight /= totalWeight;

            float weight = w.weight;
            weight = Mathf.Round(weight * 10000f) / 100f;

            // Debug.Log(w.name + ": " + weight + "%");
        }
    }

    /// <summary>
    /// A Batch Spawn that happens at the start of a level
    /// </summary>
    public void StartSpawnRoutine()
    {
        int batch = Random.Range(1, batchAmount);
        float waitTime = Random.Range(spawnIntervalMin, spawnIntervalMax);
        spawnRoutine = StartCoroutine(SpawnRoutine(waitTime, batch, false, true));
    }

    /// <summary>
    /// Stops the spawn of drops
    /// </summary>
    public void StopSpawn()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    /// <summary>
    /// Stops the current drop spawns and automatically spawn a burst
    /// </summary>
    public void Burst()
    {
        StopSpawn();
        spawnRoutine = StartCoroutine(SpawnRoutine(1f, Random.Range(2, 5), true, false));
    }

    /// <summary>
    /// Delays the burst
    /// </summary>
    /// <param name="time"></param>
    public void DelayBurst(float time)
    {
        Invoke(nameof(Burst), time);
    }

    /// <summary>
    /// The loop for drops to spawn during a level
    /// </summary>
    /// <param name="time">How long until the next Batch</param>
    /// <param name="batch">How many times a batch of drops will be spawned</param>
    /// <param name="fast">The drop locations are more random</param>
    /// <param name="repeat">Determines if the routine loops with these parameters</param>
    /// <returns></returns>
    IEnumerator SpawnRoutine(float time, int batch, bool fast, bool repeat)
    {
        if (fast) dropAreaAnimator.speed = Random.Range(8f, 12f);
        else dropAreaAnimator.speed = Random.Range(.8f, 1.6f);

        yield return new WaitForSeconds(time);

        yield return StartCoroutine(SpawnBatch(batch));
        if (repeat)
        {
            float waitTime = Random.Range(spawnIntervalMin, spawnIntervalMax);
            batch = Random.Range(1, dropLocations.Count + 1);
            spawnRoutine = StartCoroutine(SpawnRoutine(waitTime, batch, fast, true));
        }
        else
        {
            spawnRoutine = null;
        }
    }

    /// <summary>
    /// Spawns the number of batches
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    IEnumerator SpawnBatch(int num)
    {
        dropAreaAnimator.speed = 0f;

        for (int i = 0; i < num; i++)
        {
            // Pick a random drop location
            Transform dropZone = dropLocations[Random.Range(0, dropLocations.Count)].transform;

            // Number of drops in this batch
            int dropCount = Random.Range(spawnMin, spawnMax + 1);
            for (int j = 0; j < dropCount; j++)
            {
                SpawnDrop(dropZone);
                yield return new WaitForSeconds(spawnInBetweenTime);
            }
            powerupSpawned = false;
        }
        dropAreaAnimator.speed = 1f;
    }

    /// <summary>
    /// Spawns a random drop
    /// </summary>
    /// <param name="location"></param>
    public void SpawnDrop(Transform location)
    {
        if (weightedDrops.Count == 0) return;
        bool loop = true;
        do
        {
            float rand = Random.Range(0f, 1f);
            float totalChance = 0f;

            foreach (WeightedDrop w in weightedDrops)
            {
                float adjustedWeight = w.weight;
                totalChance += adjustedWeight;

                // If the random number is within the range of the total chance
                // then that drop is being spawned
                if (rand <= totalChance)
                {
                    if (w.drop is Powerup && !powerupSpawned)
                    {
                        powerupSpawned = true;
                        loop = false;
                    }
                    else if (w.drop is Powerup && powerupSpawned)
                        continue;
                        
                    SpawnDropObject(w.drop, location);
                    loop = false;
                    return;
                }
            }
        } while (loop);

    }

    /// <summary>
    /// Assigns a Drop to a Drop Object
    /// </summary>
    /// <param name="drop"></param>
    /// <param name="location"></param>
    public void SpawnDropObject(Drop drop, Transform location)
    {
        Vector3 pos = RandExt.RandomPosition(location);
        DropObject obj = Instantiate(
            dropPrefab,
            pos,
            Quaternion.identity,
            dropFolder);

        obj.drop = drop;

        // Places the drop in a random position so that the axis of spinning is random
        Vector3 randDirection = RandExt.RandomDirection(0f, 360f);
        randDirection *= .15f;
        randDirection += obj.transform.position;

        Instantiate(drop.model, randDirection, Quaternion.identity, obj.modelObj.transform);
        Instantiate(drop.sparklePrefab, obj.transform.position, Quaternion.identity, obj.transform);

        if (dropToWeight.TryGetValue(drop, out WeightedDrop wd))
        {
            wd.activeCount++;
        }
    }

    /// <summary>
    /// Spawns a specific drop at a location
    /// </summary>
    /// <param name="drop"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public DropObject SpawnDropObject(Drop drop, Vector3 pos)
    {
        DropObject obj = Instantiate(
            dropPrefab,
            pos,
            Quaternion.identity,
            dropFolder);

        obj.drop = drop;

        Instantiate(drop.model, pos, Quaternion.identity, obj.modelObj.transform);
        Instantiate(drop.sparklePrefab, obj.transform.position, Quaternion.identity, obj.transform);

        if (dropToWeight.TryGetValue(drop, out WeightedDrop wd))
        {
            wd.activeCount++;
        }

        return obj;
    }

    /// <summary>
    /// Check if the drop has already been spawned in the scene
    /// </summary>
    /// <param name="drop"></param>
    public void CheckDrop(Drop drop)
    {
        if (dropToWeight.TryGetValue(drop, out WeightedDrop wd))
        {
            wd.activeCount--;
        }
    }

    /// <summary>
    /// Clear all of the drops on the scene
    /// </summary>
    public void CleanUp()
    {
        foreach (Transform child in dropFolder)
            Destroy(child.gameObject);
    }
}
