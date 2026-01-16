using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public Player player;
    public WeaponObj startingWeapon;
    public Vector3 PlayerPos{ get { return player.transform.position; }}
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        RuntimeWeapon wpn = new RuntimeWeapon(startingWeapon);
        player.combat.Equip(wpn);
    }

}
