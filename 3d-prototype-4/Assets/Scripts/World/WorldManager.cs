using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;
    public string worldName;
    public int worldIndex;
    public UnityEvent onComplete;
    public UnityEvent onReset;
    public UnityEvent onFinish;
    public List<Transform> spawnPoints;
    public List<World> worlds;
    public List<NextLevelZone> nextLevelZones;
    public GameObject portal;
    public Transform worldCenter;
    public World currentWorld;
    public float spawnInterval = .5f;
    public float waveCheckTime = 5f;
    public int maxOnScreen = 24; // maximum on screen zombies
    private Transform currentSpawn;
    public int levelIndex = 0;
    private bool allZombiesSpawned = false;
    private bool isLastLevel;

    [Header("Counters")]
    public int waveCount; // the number of zombies to be spawned at 1 spawn point
    public int currentCount; // the current number of zombies
    public int totalCount; // the total number of zombies
    private Coroutine waveRoutine;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        currentWorld = worlds[levelIndex];
        InitWorld();
    }

    /// <summary>
    /// Initialize the world at the worldIndex
    /// </summary>
    public void InitWorld()
    {
        PlayerManager.Instance.player.info.level = levelIndex + 1;
        PlayerManager.Instance.player.info.worldIndex = worldIndex;
        PlayerManager.Instance.player.info.world = worldName;
        EntityManager.Instance.GetStats(
            currentWorld.health,
            currentWorld.minSpeed,
            currentWorld.maxSpeed
        );
        // Total number of zombies in this world
        totalCount = currentWorld.zombieCount;
        SetWave();
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);

        waveRoutine = StartCoroutine(WaveCheckRoutine(5f));
        allZombiesSpawned = false;

        if (currentWorld.isLast) isLastLevel = true;
    }

    /// <summary>
    /// Set the current wave to spawn at a single spawn point.
    /// </summary>
    public void SetWave()
    {
        // Get a random number of zombies to spawn here
        waveCount = GetWaveCount();

        // Get the lowest number between the total zombies and the wave count
        // If the total count is lower, that means this is the last wave
        waveCount = Mathf.Min(waveCount, totalCount);

        // Random Spawn
        currentSpawn = RandSpawn();
    }

    /// <summary>
    /// Get a random number between the wave's minimum and maximum.
    /// </summary>
    /// <returns></returns>
    public int GetWaveCount()
    {
        return Random.Range(currentWorld.minWaveCount, currentWorld.maxWaveCount);
    }

    /// <summary>
    /// Returns a random spawnpoint
    /// </summary>
    /// <returns></returns>
    public Transform RandSpawn()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

    /// <summary>
    /// Updates the count of zombies
    /// </summary>
    public void UpdateCount()
    {
        // The total count of zombies decreases by 1
        totalCount--;

        // The current count of zombies decreases by 1 (Used to determine how many should be spawned within a wave)
        currentCount--;
        
        if (totalCount <= 0)
        {
            if (isLastLevel)
            {
                HudManager.Instance.AdvanceLevel(worldName, levelIndex + 1);
                onFinish?.Invoke();

                Invoke(nameof(ActivatePortal), 3f);
                PlayerManager.Instance.bufferedUnits.Clear();
                foreach (Unit u in EntityManager.Instance.units)
                    PlayerManager.Instance.bufferedUnits.Add(u._name);
                foreach (Unit u in EntityManager.Instance.units)
                    u.OnHit(9999);
            }
            else
            {
                HudManager.Instance.AdvanceLevel(worldName, levelIndex + 1);
                allZombiesSpawned = true;
                onComplete?.Invoke();
            }
        }

        EntityManager.Instance.RecycleRagdolls();
        EntityManager.Instance.RecycleDrops();
    }

    public void ActivatePortal()
    {
        portal.SetActive(true);
    }

    public IEnumerator WaveCheckRoutine(float timer)
    {
        // Wait before checking the zombie count
        yield return new WaitForSeconds(timer);

        if (allZombiesSpawned)
            yield break;

        while (currentCount != maxOnScreen)
        {
            if (totalCount <= 0 || totalCount <= currentCount) break;

            currentCount++;
            // The number of zombies within this wave
            waveCount--;
            // If all the zombies within this wave equals 0, set a new wave
            if (waveCount <= 0)
            {
                SetWave();
            }
            EntityManager.Instance.SpawnEnemy(currentSpawn);
            yield return new WaitForSeconds(spawnInterval);
        }

        // Only restart if not done spawning
        if (!allZombiesSpawned)
            waveRoutine = StartCoroutine(WaveCheckRoutine(waveCheckTime));
    }


    public void Advance()
    {
        HudManager.Instance.ToggleBlackScreen(true);
        levelIndex++;
        currentWorld = worlds[levelIndex];
        InitWorld();
        onReset?.Invoke();
        HudManager.Instance.AdvanceLevel("hide", 0);
        HudManager.Instance.DisablePointers();

    }

    public void OpenRandomDoor()
    {
        nextLevelZones = RandExt.ShuffleList(nextLevelZones);
        int num = Random.Range(1, Mathf.Min(4, nextLevelZones.Count));
        for (int i = 0; i < num; i++)
        {
            NextLevelZone zone = nextLevelZones[i];
            zone.OpenDoor();
            HudManager.Instance.AssignPointers(i, zone.door);
        }
    }
}
