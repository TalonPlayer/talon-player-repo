using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Give Weapon", menuName = "Drop/PowerUp/GiveWeapon", order = 1)]
public class GrantWeapon : Powerup
{
    public Weapon weapon;
    public override void OnPickUp(Player player)
    {
        base.OnPickUp(player);
        Weapon newWeapon = WeaponLibrary.Instance.ReplaceWeapon(weapon);
        // Gives the player a weapon, if the player is currently holding the same weapon, upgrade it
        if (newWeapon._name == player.hand.hand._name)
        {
            Weapon w = WeaponLibrary.Instance.Upgrade(player, player.hand.hand);
            player.hand.Equip(w);
        }
        else
            player.hand.Equip(newWeapon);

    }
}
