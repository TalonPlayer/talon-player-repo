using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public Player player;
    public int weaponSlots = 2;
    public WeaponObj startingWeapon;
    public Vector3 PlayerPos{ get { return player.transform.position; }}
    private Vector3 initialSpawn;
    public delegate void OnWeaponChange(); public static event OnWeaponChange onWeaponChange;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        initialSpawn = PlayerPos;

        Invoke(nameof(ResetPlayer), .5f);
    }

    public void ResetPlayer()
    {
        player.Reset();

        player.combat.Init(weaponSlots);

        RuntimeWeapon wpn = new RuntimeWeapon(startingWeapon);
        player.combat.Equip(wpn);

        
        player.Teleport(initialSpawn, Quaternion.identity);
    }

    public void WeaponSlotsChanged()
    {
        onWeaponChange?.Invoke();
    }
}
