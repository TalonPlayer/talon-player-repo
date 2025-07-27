using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLibrary : MonoBehaviour
{
    public static WeaponLibrary Instance;
    public List<Weapon> weapons = new List<Weapon>();
    void Awake()
    {
        Instance = this;
    }

    public Weapon Upgrade(Weapon weapon)
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

        return weapon;
    }


    public Weapon Downgrade(Weapon weapon)
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

        return weapons[0];
    }
}
