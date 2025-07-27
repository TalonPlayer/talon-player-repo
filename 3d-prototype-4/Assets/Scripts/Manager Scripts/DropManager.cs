using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedDrop
{
    public Drop drop;
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
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
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
            weightedDrops.Add(wd);
            dropToWeight[d] = wd;
        }

        // Normalize weights only ONCE
        foreach (WeightedDrop w in weightedDrops)
        {
            w.weight /= totalWeight;
        }
    }


    public void StartSpawnRoutine()
    {
        int batch = Random.Range(1, dropLocations.Count + 1);
        float waitTime = Random.Range(spawnIntervalMin, spawnIntervalMax);
        spawnRoutine = StartCoroutine(SpawnRoutine(waitTime, batch, false, true));
    }

    public void StopSpawn()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    public void Burst()
    {
        StopSpawn();
        spawnRoutine = StartCoroutine(SpawnRoutine(1f, Random.Range(2, 5), true, false));
    }

    public void DelayBurst(float time)
    {
        Invoke(nameof(Burst), time);
    }

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
        }
        dropAreaAnimator.speed = 1f;
    }


    public void SpawnDrop(Transform location)
    {
        if (weightedDrops.Count == 0) return;

        float rand = Random.Range(0f, 1f);
        float totalChance = 0f;

        foreach (WeightedDrop w in weightedDrops)
        {
            float adjustedWeight = w.weight;

            // If the drop already exists, then reduce the chances of it spawning
            if (w.drop is Powerup && w.activeCount > 0)
                adjustedWeight *= 0.05f; // reduce weight if that powerup is already active

            totalChance += adjustedWeight;

            // If the random number is within the range of the total chance
            // then that drop is being spawned
            if (rand <= totalChance)
            {
                SpawnDropObject(w.drop, location);
                return;
            }
        }
    }

    public void SpawnDropObject(Drop drop, Transform location)
    {
        Vector3 pos = RandExt.RandomPosition(location);
        DropObject obj = Instantiate(
            dropPrefab,
            pos,
            Quaternion.identity,
            dropFolder);

        obj.drop = drop;
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

    public void CheckDrop(Drop drop)
    {
        if (dropToWeight.TryGetValue(drop, out WeightedDrop wd))
        {
            wd.activeCount--;
        }
    }

    public void CleanUp()
    {
        foreach (Transform child in dropFolder)
            Destroy(child.gameObject);
    }
}
