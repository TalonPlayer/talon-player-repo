using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance;
    public List<Enemy> enemies;
    public List<Unit> units;
    public List<GameObject> enemyPrefabs;
    public Transform enemyFolder;
    public Transform unitFolder;
    public Transform ragdollFolder;
    public Transform itemFolder;
    public Transform skullFolder;
    public PhysicalDrop skullPrefab;
    public float skullChance = 90;
    public float spawnTime = 3f;
    public delegate void DamageTick();
    public static event DamageTick damageTick;
    public delegate void CheckHealth();
    public static event CheckHealth healthTick;
    public delegate void AggroClosest();
    public static event AggroClosest aggroTick;
    private int health;
    private int minSpeed;
    private int maxSpeed;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {

    }

    void Update()
    {
        if (damageTick != null)
        {
            damageTick();
            damageTick = null;

            if (healthTick != null)
                healthTick();
        }

        if (aggroTick != null)
        {
            aggroTick();
        }
    }

    public void GetStats(int _health, int _minSpeed, int _maxSpeed)
    {
        health = _health;
        minSpeed = _minSpeed;
        maxSpeed = _maxSpeed;
    }

    /// <summary>
    /// Spawns the enemy pool
    /// </summary>
    public void SpawnEnemy(Transform spawn)
    {
        Vector3 pos = RandExt.RandomPosition(spawn);
        pos.y += 2f;
        pos = SnapToGround(pos);
        GameObject obj = Instantiate(
            enemyPrefabs[Random.Range(0, enemyPrefabs.Count)],
            pos,
            spawn.rotation,
            enemyFolder
            );

        Enemy e = obj.GetComponent<Enemy>();
        e.Spawn(spawnTime);
        e.Init(health, minSpeed, maxSpeed);
        enemies.Add(e);
    }

    public void SpawnUnit(Vector3 spawn, Unit unit)
    {
        spawn = SnapToGround(spawn);
        Unit u = Instantiate(
            unit,
            spawn,
            Quaternion.identity,
            unitFolder
            );

        u.Spawn(spawnTime);
        units.Add(u);
    }

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

    public void RecycleRagdolls()
    {
        if (ragdollFolder.childCount >= 65)
        {
            List<Transform> delete = new List<Transform>();
            for (int i = 0; i < Random.Range(1, 5); i++)
            {
                delete.Add(ragdollFolder.GetChild(i));
            }

            foreach (Transform obj in delete)
            {
                Destroy(obj.gameObject);
            }
        }
    }

    public void RecycleDrops()
    {
        if (itemFolder.childCount >= 65)
        {
            List<Transform> delete = new List<Transform>();
            for (int i = 0; i < Random.Range(1, 5); i++)
            {
                delete.Add(itemFolder.GetChild(i));
            }

            foreach (Transform obj in delete)
            {
                Destroy(obj.gameObject);
            }
        }
    }

    public void CleanUp()
    {
        foreach (Transform child in itemFolder)
            Destroy(child.gameObject);
        foreach (Transform child in ragdollFolder)
            Destroy(child.gameObject);
        foreach (Transform child in skullFolder)
            Destroy(child.gameObject);
    }
    public void UnitSnapToPlayer()
    {
        Vector3 playerPos = PlayerManager.Instance.player.transform.position;

        int unitCount = units.Count;
        if (unitCount == 0) return;

        float angleStep = 360f / unitCount;
        float radius = 1f;

        for (int i = 0; i < unitCount; i++)
        {
            Unit unit = units[i];
            NavMeshAgent agent = unit.movement.agent;
            unit.movement.enabled = false;
            // Disable NavMeshAgent and manually teleport the whole root GameObject
            if (agent.enabled) agent.enabled = false;

            float angleRad = angleStep * i * Mathf.Deg2Rad;
            float x = Mathf.Cos(angleRad) * radius;
            float z = Mathf.Sin(angleRad) * radius;

            Vector3 offset = new Vector3(x, 0f, z);
            Vector3 targetPos = SnapToGround(playerPos + offset);

            unit.transform.position = targetPos;
            unit.transform.rotation = Quaternion.LookRotation(playerPos - targetPos); // face center
        }

        // Delay re-enabling NavMeshAgents one frame
        StartCoroutine(ReEnableAgentsNextFrame());
    }

    private IEnumerator ReEnableAgentsNextFrame()
    {
        yield return new WaitForSeconds(.5f); // wait one frame

        foreach (Unit unit in units)
        {
            unit.movement.enabled = true;
            NavMeshAgent agent = unit.movement.agent;

            agent.enabled = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }
    }



    public void SpawnSkull(Vector3 pos)
    {
        float rand = Random.Range(0f, 100f);

        if (rand > skullChance) return;

        PhysicalDrop skull = Instantiate(skullPrefab, pos, Quaternion.identity, skullFolder);
        skull.LaunchUp();
    }

    public void ClearAggro()
    {
        aggroTick = null;
    }
}
