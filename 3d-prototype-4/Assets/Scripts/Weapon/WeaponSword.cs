using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equip Sword", menuName = "Drop/PowerUp/Equip Sword", order = 1)]
public class WeaponSword : GrantWeapon
{
    public override void OnPickUp(Player player)
    {
        base.OnPickUp(player);

        player.stats.StartRampage();
    }
}
