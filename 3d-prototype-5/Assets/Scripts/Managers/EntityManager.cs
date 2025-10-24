using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance;
    public int humans = 10;
    public int zombies = 10;
    public Transform ragdollFolder;
    public Transform itemFolder;
    public Transform projectileFolder;
    public Transform wanderFolder;

    [Header("Lists")]
    public List<Entity> entities;       // Active Entities
    public List<EntityObj> entityInfos;  // Entity Info
    public List<Transform> wanderPoints;
    public EntityPreset[] presetEntities;
    public List<string> names;
    public Transform humanFolder;
    public Transform zombieFolder;

    [Header("Prefabs")]
    public Entity humanSoldier;
    public Entity humanWanderer;
    public Entity zombiePrefab;

    [Header("Entity Pool")]
    public EntityBody defaultBody;
    public EntityPool pool;
    public Transform humanSpawn;
    public Transform zombieSpawn;
    public List<Coroutine> scheduledRespawn = new List<Coroutine>();
    // Delegates

    public delegate void OnMove();
    public static event OnMove moveTick;
    public delegate void OnFinish();
    public static event OnFinish finishTick;
    public delegate void OnVision();
    public static event OnVision visionTick;
    public delegate void AggroClosest();
    public static event AggroClosest aggroTick;
    public delegate void BrainTick();
    public static event BrainTick brainTick;
    private List<Outfit> faceList, hatList, gloveList, shoeList, beltList;
    void Awake()
    {
        Instance = this;

        wanderPoints = Helper.GetChildren(wanderFolder);

        presetEntities = Resources.LoadAll<EntityPreset>("Scriptable Objects/Entity Presets");

        StartCoroutine(VisionRoutine());
    }
    public void SillyHalloween()
    {
        EntityPreset sillyHalloween = presetEntities.ToList().Find(e => e.ID == "GAAZE");

        for (int i = 0; i < 15; i++)
        {
            EntityObj obj = new EntityObj(sillyHalloween);

            obj.entityID = "GAAZE_" + i;

            entityInfos.Add(obj);
        }
    }
    void PresetOutfits()
    {
        faceList = CostumeManager.Instance.faceList.FindAll(o => !o.presetExclusive);
        hatList = CostumeManager.Instance.hatList.FindAll(o => !o.presetExclusive);
        gloveList = CostumeManager.Instance.gloveList.FindAll(o => !o.presetExclusive);
        shoeList = CostumeManager.Instance.shoeList.FindAll(o => !o.presetExclusive);
        beltList = CostumeManager.Instance.beltList.FindAll(o => !o.presetExclusive);

    }
    void Start()
    {
        PresetOutfits();
        //AddPresetPool();
        //SillyHalloween();
        CreateEntityPool();

        foreach (EntityObj info in entityInfos)
        {
            SpawnHuman(info);
        }


        for (int i = 0; i < zombies; i++)
        {
            string n = Helper.RandomElement(pool.firstNames);
            n += " " + Helper.RandomElement(pool.lastNameInitials);
            EntityObj obj = new EntityObj(n);

            obj.threatLevel = Random.Range(25, 75);

            obj.weaponID = Helper.RandomElement(pool.weapons).weaponID;

            obj.speed = Random.Range(0, 100f);
            obj.experience = (Experience)Random.Range(0, 5);
            obj.role = RoleType.Follower;
            // obj.role = (RoleType)Random.Range(0, 6);
            obj.mission = (ObjectiveType)Random.Range(0, 4);
            obj.social = (SocialType)Random.Range(0, 3);
            obj.behavior = (BehaviorType)Random.Range(0, 5);
            obj.body = defaultBody;

            for (int j = 0; j < obj.body.clothes.Count; j++)
            {
                obj.primaryColors.Add(Helper.RandomElement(pool.primaryColors));
                obj.secondaryColors.Add(Helper.RandomElement(pool.secondaryColors));
            }
            obj.shirtTexture = Helper.RandomElement(pool.shirtTextures);

            obj.faceID = Helper.RandomElement(faceList).ID;
            obj.hatID = Helper.RandomElement(hatList).ID;
            obj.glovesID = Helper.RandomElement(gloveList).ID;
            obj.shoesID = Helper.RandomElement(shoeList).ID;
            obj.beltID = Helper.RandomElement(beltList).ID;

            entityInfos.Add(obj);
            SpawnZombie(obj);
        }

    }

    void Update()
    {

        aggroTick?.Invoke();

        brainTick?.Invoke();
        moveTick?.Invoke();

        finishTick?.Invoke();
        finishTick = null;
    }

    IEnumerator VisionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(.25f);
            visionTick?.Invoke();
        }
    }
    /// <summary>
    /// Create Preset Entities
    /// </summary>
    public void AddPresetPool()
    {
        foreach (EntityPreset preset in presetEntities)
            entityInfos.Add(new EntityObj(preset));
    }
    public void CreateEntityPool()
    {
        // Change 15 for GameManager.cs Entity Pool Num - Preset Count
        for (int i = 0; i < humans; i++)
        {
            string n = Helper.RandomElement(pool.firstNames);
            n += " " + Helper.RandomElement(pool.lastNameInitials);
            EntityObj obj = new EntityObj(n);

            obj.threatLevel = Random.Range(25, 75);

            obj.weaponID = Helper.RandomElement(pool.weapons).weaponID;

            obj.speed = Random.Range(0, 100f);
            obj.experience = (Experience)Random.Range(0, 5);
            obj.role = RoleType.Follower;
            // obj.role = (RoleType)Random.Range(0, 6);
            obj.mission = (ObjectiveType)Random.Range(0, 4);
            obj.social = (SocialType)Random.Range(0, 3);
            obj.behavior = (BehaviorType)Random.Range(0, 5);
            obj.body = defaultBody;

            for (int j = 0; j < obj.body.clothes.Count; j++)
            {
                obj.primaryColors.Add(Helper.RandomElement(pool.primaryColors));
                obj.secondaryColors.Add(Helper.RandomElement(pool.secondaryColors));
            }
            obj.shirtTexture = Helper.RandomElement(pool.shirtTextures);

            obj.faceID = Helper.RandomElement(faceList).ID;
            obj.hatID = Helper.RandomElement(hatList).ID;
            obj.glovesID = Helper.RandomElement(gloveList).ID;
            obj.shoesID = Helper.RandomElement(shoeList).ID;
            obj.beltID = Helper.RandomElement(beltList).ID;

            entityInfos.Add(obj);
        }
    }

    public void ScheduleRespawn(string _name)
    {
        if (scheduledRespawn.Count != 0)
            scheduledRespawn = scheduledRespawn.FindAll(r => r != null);

        EntityObj info = entityInfos.Find(i => i.entityName == _name);
        scheduledRespawn.Add(StartCoroutine(RespawnRoutine(info, 15f)));
    }

    IEnumerator RespawnRoutine(EntityObj info, float respawnTime)
    {
        yield return new WaitForSeconds(respawnTime);

        SpawnZombie(info);
    }



    public void SpawnHuman(EntityObj info)
    {
        Vector3 pos = Helper.RandomPosition(humanSpawn);
        pos = SnapToGround(pos);
        Vector3 rot = Helper.RandomVectorInRadius(1f) + transform.position;
        Entity e;
        switch (info.role)
        {
            case RoleType.Follower:
                e = Instantiate(humanSoldier, pos, Quaternion.LookRotation(rot), humanFolder);
                break;
            default:
                e = Instantiate(humanSoldier, pos, Quaternion.LookRotation(rot), humanFolder);
                break;
        }


        if (info.hasClan)
        {
            e.brain.hasGroup = info.hasClan;

            if (info.clanLogo)
                GroupManager.Instance.AssignToGroup(info.clanName, e, info.clanLogo);
            else
                GroupManager.Instance.AssignToGroup(info.clanName, e);

        }
        e.name = info.entityName;
        e.brain.isHuman = true;

        e.speed = info.speed;
        e.brain.expLvl = info.experience;

        e.brain.roleType = info.role;

        e.brain.objType = info.mission;
        e.brain.socialType = info.social;
        e.brain.behaviorType = info.behavior;


        BuildEntity(e, info);

        e.Init();

        e.brain.objectiveTarget = ObjectiveManager.Instance.objectives[0].transform;

        e.entityName = info.entityName;
        e.entityID = info.entityID;

        WeaponModel weapon;

        if (info.exclusiveWeapon)
            weapon = pool.exclusiveWeapons.Find(w => w.weaponID == info.weaponID);
        else
            weapon = pool.weapons.Find(w => w.weaponID == info.weaponID);

        e.combat.Equip(weapon);
        e.brain.knowsObjective = true;

        if (e.outline) e.outline.Init();
    }

    public void BuildEntity(Entity entity, EntityObj info)
    {
        EntityBody body = Instantiate(info.body, entity.transform);

        entity.body = body;
        entity.combat.body = body;

        body.SetClothes(info);
        CostumeManager.Instance.Dress(entity, info);

    }


    public void SpawnZombie(EntityObj info)
    {
        Vector3 pos = Helper.RandomPosition(zombieSpawn);
        pos = SnapToGround(pos);
        Vector3 rot = Helper.RandomVectorInRadius(1f) + transform.position;
        Entity e = Instantiate(zombiePrefab, pos, Quaternion.LookRotation(rot), zombieFolder);
        BuildEntity(e, info);
        e.Init();
        e.brain.objectiveTarget = ObjectiveManager.Instance.objectives[0].transform;
        e.name = info.entityName;
        e.entityName = info.entityName;
        e.entityID = info.entityID;

        WeaponModel weapon = pool.weapons.Find(w => w.weaponID == info.weaponID);

        e.combat.SetAnimatorController(weapon.zombieAnimator);

        e.brain.knowsObjective = true;
    }


    public void GetFriends(Entity ent, EntityPreset obj)
    {
        ent.brain.hasFriends = true;
        foreach (string id in obj.friendIDS)
        {
            ent.friends.Add(entities.Find(e => e.entityID == id));
        }
    }

    public void RecycleRagdolls()
    {
        if (ragdollFolder.childCount >= 20)
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

    public void RecycleItems()
    {
        if (itemFolder.childCount >= 20)
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

    public Vector3 SnapToGround(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos, Vector3.down, out hit, 3.5f, 1 << 11)) // Layer 11 = Ground
        {
            Vector3 groundedPos = pos;
            groundedPos.y = hit.point.y;
            return groundedPos;
        }

        return pos;
    }
}
