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
    public Transform enemyFolder;
    public Transform allyFolder;
    public Transform weaponDropFolder;
    public Transform ragdollFolder;
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
        u.transform.parent = Instance.enemyFolder;
        Instance.enemies.Add(u);
    }

    public static void AddAlly(Unit u)
    {
        u.transform.parent = Instance.allyFolder;
        Instance.allies.Add(u);
    }

    public void CleanUp()
    {
        Transform[] folders = { enemyFolder, allyFolder, weaponDropFolder, ragdollFolder };

        foreach(Transform f in folders)
        {
            Transform[] children = Helper.GetChildrenArray(f);

            foreach(Transform c in children)
                Destroy(c.gameObject);
        }

        enemies.Clear();
        allies.Clear();

        brainTick = null;
    }

}
