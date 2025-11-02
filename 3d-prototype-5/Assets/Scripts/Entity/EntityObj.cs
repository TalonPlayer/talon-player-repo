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
}
