using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLibrary : MonoBehaviour
{
    public static WeaponLibrary Instance;
    public List<Weapon> weapons = new List<Weapon>();
    public List<Weapon> goldWeapons = new List<Weapon>();
    public List<string> weaponNames;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // GetGoldWeapons();
    }

    public void GetGoldWeapons()
    {
        foreach (string name in weaponNames)
        {
            if (GlobalSaveSystem.IsUnlocked("Gold " + name))
            {
                ReplaceWeapon(name);
            }
        }
    }

    public void ReplaceWeapon(string weaponName)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            Weapon current = weapons[i];

            // Check if names match
            if (current._name != weaponName) continue;

            // Try to find the gold weapon with the same name and next tier
            Weapon gold = goldWeapons.Find(g =>
                g._name == current._name && g.id == current.id);

            foreach (Player player in PlayerManager.Instance.players)
            {
                Weapon def = player.hand.defaultWeapon;

                if (def.id == gold.id)
                {
                    WorldManager.Instance.ChangeDefaultWeapon(player, gold);
                }
            }
            if (gold != null)
            {
                weapons[i] = gold; // replace with gold version
                Debug.Log($"Replaced {current.id} with {gold.id}");
            }
        }
    }

    public Weapon ReplaceWeapon(Weapon weapon)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            Weapon current = weapons[i];

            // Check if both name and id match
            if (current._name == weapon._name && current.id == weapon.id)
            {
                return current;
            }
        }

        // Nothing found
        return null;
    }

    public Weapon Upgrade(Player player, Weapon weapon)
    {
        if (weapon == null || string.IsNullOrEmpty(weapon.id))
            return null;

        // Parse current weapon ID number
        string prefix = "ID_";
        string numberPart = weapon.id.Replace(prefix, "");

        if (!int.TryParse(numberPart, out int currentID))
            return null;

        int nextID = currentID + 1;
        string nextIDString = $"{prefix}{nextID:D3}"; // Keeps leading zeros (e.g., ID_011)

        // Search for the weapon with the next ID
        foreach (Weapon w in weapons)
        {
            if (w.id == nextIDString)
            {
                return w;
            }
        }

        if (weapon._name == "Sword") player.stats.StartRampage();

        return weapon;
    }


    public Weapon Downgrade(Player player, Weapon weapon)
    {
        if (weapon == null || string.IsNullOrEmpty(weapon.id))
            return null;

        string prefix = "ID_";
        string numberPart = weapon.id.Replace(prefix, "");

        if (!int.TryParse(numberPart, out int currentID))
            return null;

        int prevID = currentID - 1;
        if (prevID < 0) return null;

        string prevIDString = $"{prefix}{prevID:D3}";

        foreach (Weapon w in weapons)
        {
            if (w.id == prevIDString)
            {
                return w;
            }
        }

        return player.hand.defaultWeapon;
    }
}
