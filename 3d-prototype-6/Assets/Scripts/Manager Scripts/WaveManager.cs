using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;
    public int maxOnScreen = 14;
    public int EnemyCount { get { return enemies.Count; } }
    public int nextWaveTimer = 30;
    public List<WaveInfo> wave;
    private WaveInfo currentWave;
    public int waveIndex = -1;
    public int spawnIndex = 0;
    public bool intermission = false;
    public bool debugMode = false;
    [SerializeField] private List<Transform> spawnpoints = new List<Transform>();
    private List<Unit> enemies = new List<Unit>();
    private Coroutine spawningRoutine, intermissionRoutine;
    public delegate void SpawnProximity(); public static event SpawnProximity onProximity;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (debugMode) 
        { 
            intermission = true;
            return;
        }
        
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        onProximity?.Invoke();
    }

    public void Reset()
    {
        if (spawningRoutine != null) StopCoroutine(spawningRoutine);
        if (intermissionRoutine != null) StopCoroutine(intermissionRoutine);

        enemies.Clear();
        spawnIndex = 0;
        waveIndex = -1;
        NextWave();
    }

    public void NextWave()
    {
        waveIndex++;
        waveIndex = Mathf.Min(waveIndex, wave.Count - 1);
        currentWave = wave[waveIndex];
        StartWave(currentWave);
    }

    public void StartWave(WaveInfo wave)
    {
        intermission = false;
        spawnIndex = 0;

        spawningRoutine = StartCoroutine(SpawningRoutine(wave.spawnableUnits));
    }
    private IEnumerator SpawningRoutine(UnitInfo[] units)
    {
        while (spawnIndex < units.Length)
        {
            yield return Random.Range(0f, .25f);

            if (EnemyCount < maxOnScreen)
            {
                Vector3 pos = new Vector3();
                Quaternion rot = new Quaternion();

                if (spawnpoints.Count == 0)
                {
                    pos = Vector3.zero;
                    rot = Quaternion.identity;
                }
                else
                {
                    Transform rand = Helper.RandomElement(spawnpoints);

                    pos = rand.position;
                    rot = rand.rotation;
                }


                UnitInfo u = units[spawnIndex];
                Unit unit = Instantiate(u.unit, pos, rot);

                unit.brain.spawnPlan = PlanLibrary.GetPlans(unit.ai.AILevel, BehaviorMode.Spawn);
                unit.brain.searchPlan = PlanLibrary.GetPlans(unit.ai.AILevel, BehaviorMode.Search);
                unit.brain.combatPlan = PlanLibrary.GetPlans(unit.ai.AILevel, BehaviorMode.Combat);
                unit.brain.retreatPlan = PlanLibrary.GetPlans(unit.ai.AILevel, BehaviorMode.Retreat);

                if (u.startingWeapon != null) unit.combat.startingWeapon = u.startingWeapon;


                unit.Init();

                unit.death.AppendEvent(() => CheckEnemyCount(unit));
                enemies.Add(unit);
                spawnIndex++;
                HUDManager.UpdateWaveInfo($"");
            }

        }
    }
    private IEnumerator Intermission(int time)
    {
        intermission = true;
        for (int i = time; i > 0; i--)
        {
            HUDManager.UpdateWaveInfo($"Time Remaining: {i} | Press 6 to skip");
            yield return new WaitForSeconds(1);
        }
        NextWave();
    }

    public void CheckEnemyCount(Unit unit)
    {
        enemies.Remove(unit);
        PlayerManager.Instance.player.AddPoints(unit.points);
        if (EnemyCount <= 5) HUDManager.UpdateWaveInfo($"Enemy Count: {EnemyCount}");
        if (EnemyCount <= 0)
        {
            if (intermissionRoutine != null) StopCoroutine(intermissionRoutine);
            intermissionRoutine = StartCoroutine(Intermission(nextWaveTimer));
            return;
        }
    }

    public void SkipIntermission()
    {
        if (intermission)
        {
            if (intermissionRoutine != null) StopCoroutine(intermissionRoutine);
            intermissionRoutine = StartCoroutine(Intermission(5));

            intermission = false;
        }
    }

    public static void AddSpawn(Transform spawn)
    {
        Instance.spawnpoints.Add(spawn);
    }
    public static void RemoveSpawn(Transform spawn)
    {
        Instance.spawnpoints.Remove(spawn);
    }
}

// The unit with the given starting weapon
[System.Serializable]
public struct UnitInfo
{
    public Unit unit;
    public WeaponObj startingWeapon;
}

[System.Serializable]
public struct WaveInfo
{
    public UnitInfo[] spawnableUnits;
}

