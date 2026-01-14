using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityObj
{
    public string entityName;
    public string entityID;
    public List<string> friendIDS;
    public string clanName;
    public Sprite clanLogo;
    public bool active;
    public EntityBody body;
    public bool isPreset;
    public bool hasClan;

    public Experience experience;
    public int threatLevel;
    public bool exclusiveWeapon = false;
    public string weaponID;

    public float speed;

    public RoleType role;
    public ObjectiveType mission;
    public SocialType social;
    public BehaviorType behavior;

    public List<Color> primaryColors = new List<Color>();
    public List<Color> secondaryColors = new List<Color>();
    public Texture shirtTexture;
    public string faceID;
    public string hatID;
    public string glovesID;
    public string shoesID;
    public string beltID;
    public bool respawning = false;
    public List<Entity> spawnAwayFrom;
    public Vector3 deathPos;
    public EntityObj() { }

    public EntityObj(string name)
    {
        active = false;
        entityName = name;
    }

    public EntityObj(EntityPreset preset)
    {
        isPreset = true;
        active = false;
        hasClan = preset.hasClan;
        entityName = preset._name;
        entityID = preset.ID;
        friendIDS = preset.friendIDS;
        clanName = preset.clanName;
        clanLogo = preset.clanLogo;
        body = preset.body;

        experience = preset.experience;
        threatLevel = preset.threatLevel;

        weaponID = preset.weaponID;
        exclusiveWeapon = preset.exclusiveWeapon;

        speed = preset.speed;

        role = preset.roleType;
        mission = preset.objType;
        social = preset.socialType;
        behavior = preset.behaviorType;

        primaryColors = preset.primaryColors;
        secondaryColors = preset.secondaryColors;
        shirtTexture = preset.shirtTexture;

        faceID = preset.faceID;
        hatID = preset.hatID;
        glovesID = preset.glovesID;
        shoesID = preset.shoesID;
        beltID = preset.beltID;
    }

    public Vector3 GetRespawnPoint()
    {
        Vector3 enemyCohesion = Cohesion();

        Vector3 away = (deathPos - enemyCohesion).normalized;
        away.y = deathPos.y + .25f;
        int layers = (1 << 11) | (1 << 12);
        if (Physics.Raycast(deathPos, away, out RaycastHit hit, 20f, layers, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.tag == "Wall")
            {
                hit.point -= away * 2f;

                Vector3 perp = Vector3.Cross(hit.point, Vector3.up);
                perp *= 2f;
                perp.y = .25f;
                float left = Vector3.Distance(-perp, enemyCohesion);
                float right = Vector3.Distance(perp, enemyCohesion);

                hit.point = left > right ? perp : -perp;
                if (left > right)
                    hit.point = perp;
                else
                {
                    hit.point = -perp;
                    perp = -perp;
                }
                if (Physics.Raycast(hit.point, perp, out hit, 7f, layers, QueryTriggerInteraction.Ignore))
                    respawning = false;
            }
                
            return hit.point;
        }
        away.y = .25f;
        away *= 20f;
        return away + deathPos;

    }

    public Vector3 Cohesion()
    {
        var enemies = spawnAwayFrom;
        if (enemies == null || enemies.Count == 0)
            return deathPos;

        Vector3 center = deathPos;
        int count = 0;

        for (int i = 0; i < enemies.Count; i++)
        {
            var e = enemies[i];
            if (e == null) continue;
            center += e.transform.position;
            count++;
        }

        return (count > 0) ? center / count : deathPos;
    }
}
