using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance;
    public delegate void OnHit(); public static event OnHit onHit;
    public delegate void OnDeath(); public static event OnDeath onDeath;
    public delegate void BrainTick(); public static event BrainTick brainTick;
    public List<Unit> enemies = new List<Unit>();
    public List<Unit> allies = new List<Unit>();
    public List<Entity> entities = new List<Entity>();
    public Transform enemyFolder;
    public Transform allyFolder;
    public Transform weaponDropFolder;
    void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        onHit?.Invoke(); onHit = null;

        onDeath?.Invoke(); onDeath = null;

        brainTick?.Invoke();
    }

    public static void AddEnemy(Unit u)
    {
        Instance.enemies.Add(u);
    }

    public static void AddAlly(Unit u)
    {
        Instance.allies.Add(u);
    }
}
