using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityHUDManager : MonoBehaviour
{
    public static EntityHUDManager Instance;
    [Header("Character Info")]
    public Image clanLogo;
    public GameObject clanObj;
    public TextMeshProUGUI characterName, experience, clanRole, status, action, weaponName, weaponInfo, threatLevel;
    [Header("Generic Info")]
    public List<Image> badges;
    public Image playstyle;
    public Image social;
    public Image behavior;

    [Header("Stat Info")]
    public FriendBox boxPrefab;
    public Transform friendListContent;
    public List<FriendBox> friendBoxes;
    public Image ammoBar, magBar, throwBar, stamBar, spdBar;
    public TextMeshProUGUI ammoStat, magStat, throwStat, stamStat, spdStat;
    private List<Sprite> badgeSprites;
    private List<Sprite> playStyleSprites;
    private List<Sprite> socialSprites;
    private List<Sprite> behaviorSprites;
    public Sprite humanSprite;
    public Sprite zombieSprite;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        badgeSprites = Resources.LoadAll<Sprite>("Sprites/Badges").ToList();
        playStyleSprites = Resources.LoadAll<Sprite>("Sprites/Personalities/Playstyle").ToList();
        socialSprites = Resources.LoadAll<Sprite>("Sprites/Personalities/Social").ToList();
        behaviorSprites = Resources.LoadAll<Sprite>("Sprites/Personalities/Behavior").ToList();

    }

    public void SetHud(Entity entity)
    {

        // Name
        characterName.text = entity.entityName;

        // Experience
        switch ((int) entity.brain.expLvl)
        {
            case 0:
                experience.text = "Beginner";
                break;
            case 1:
                experience.text = "Intermediate";
                break;
            case 2:
                experience.text = "Advanced";
                break;
            case 3:
                experience.text = "Expert";
                break;
            case 4:
                experience.text = "Master";
                break;
        }

        // Clan + Role

        switch ((int)entity.brain.roleType)
        {
            case 0:
            clanRole.text = "Leader";
                break;
            case 1:
            clanRole.text = "Follower";
                break;
            case 2:
            clanRole.text = "Wanderer";
                break;
            case 3:
            clanRole.text = "Sociable";
                break;
            case 4:
            clanRole.text = "LoneWolf";
                break;
            case 5:
            clanRole.text = "Opportunist";
                break;
        }

        // Clan Logo
        if (entity.brain.hasGroup)
        {
            clanObj.SetActive(true);
            clanRole.text += " | " + entity.group.groupName;

            if (entity.group.groupImage)
                clanLogo.sprite = entity.group.groupImage;
        }
        else
        {
            clanObj.SetActive(false);
        }
        // Status

        // Action

        // Weapon Name

        WeaponModel weapon = entity.combat.weapon;
        weaponName.text = weapon.weaponName;

        // Weapon Info
        switch (weapon.weaponType)
        {
            case WeaponType.Pistol:
                weaponInfo.text = "Pistol\n";
                break;
            case WeaponType.Rifle:
                weaponInfo.text = "Rifle\n";

                break;
            case WeaponType.Sniper:
                weaponInfo.text = "Sniper\n";

                break;
            case WeaponType.Minigun:
                weaponInfo.text = "Minigun\n";
                break;
        }
        switch (weapon.loadingType)
        {
            case LoadType.Single:
                weaponInfo.text += "Single-Shot\n";
                break;
            case LoadType.Pump:
                weaponInfo.text += "Pump-Action\n";

                break;
            case LoadType.Semi:
                weaponInfo.text += "Semi-Auto\n";

                break;
            case LoadType.Auto:
                weaponInfo.text += "Full-Auto\n";

                break;
        }

        weaponInfo.text += "Fire Rate: " + weapon.fireRate + "\n";
        weaponInfo.text += "Ammo: " + weapon.maxAmmoCount + "\n";

        // Threat Level
        threatLevel.text = entity.threatLevel.ToString();

        // Badge Sprites

        // Personality Sprites

        // Friend List

        foreach (FriendBox box in friendBoxes)
            Destroy(box.gameObject);
        friendBoxes.Clear();

        if (entity.brain.hasFriends)
        {
            foreach (Entity friend in entity.friends)
            {
                FriendBox box = Instantiate(boxPrefab, friendListContent);
                box.aliveStatus.sprite = friend.brain.isHuman ? humanSprite : zombieSprite;

                box.friendName.text = friend.entityName;
            }

        }

        // Ammo
        ammoBar.fillAmount = (float) entity.combat.currentAmmoCount / entity.combat.maxAmmoCount;
        ammoStat.text = entity.combat.currentAmmoCount + " / " + entity.combat.maxAmmoCount;

        // Mags
        magBar.fillAmount = (float )entity.combat.currentMags / entity.combat.maxMagCount;
        magStat.text = entity.combat.currentMags + " / " + entity.combat.maxMagCount;

        // Throwables
        throwBar.fillAmount = (float) entity.combat.throwableCount / entity.combat.maxThrowableCount;
        throwStat.text = entity.combat.throwableCount + " / " + entity.combat.maxThrowableCount;

        // Stamina
        stamBar.fillAmount = entity.stamina / 100f;
        stamStat.text = entity.stamina + " / 100";

        // Speed
        spdBar.fillAmount = entity.speed / 100;
        spdStat.text = entity.speed + " / 100";
    }
}
