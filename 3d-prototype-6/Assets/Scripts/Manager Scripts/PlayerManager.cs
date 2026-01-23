using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public Player player;
    public WeaponObj startingWeapon;
    public Vector3 PlayerPos{ get { return player.transform.position; }}
    private Vector3 initialSpawn;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        initialSpawn = PlayerPos;

        ResetPlayer();
    }

    public void ResetPlayer()
    {
        RuntimeWeapon wpn = new RuntimeWeapon(startingWeapon);
        player.combat.Equip(wpn);

        player.Reset();

        player.Teleport(initialSpawn, Quaternion.identity);
    }
}
