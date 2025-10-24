
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Preset Entity", menuName = "ScriptObjs/Preset Entity", order = 1)]
public class EntityPreset : ScriptableObject
{
    [Header("Info")]
    public string _name = "No Name";
    public string ID;
    public Experience experience;
    public int threatLevel = 50;
    public bool hasClan;
    public string clanName = "No Clan";
    public Sprite clanLogo;
    public EntityBody body;
    public List<string> friendIDS;
    public bool exclusiveWeapon = false;
    public string weaponID = "NERF_001";

    [Header("Social Types")]
    public RoleType roleType;
    public ObjectiveType objType;
    public SocialType socialType;
    public BehaviorType behaviorType;

    [Header("Stats")]
    public float speed;

    [Header("Outfits")]
    public List<Color> primaryColors;
    public List<Color> secondaryColors;
    public Texture shirtTexture;
    
    [Header("Outfit ID")]
    public string faceID;
    public string hatID;
    public string glovesID;
    public string shoesID;
    public string beltID;
    
}
