using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Give Weapon", menuName = "Drop/PowerUp/GiveWeapon", order = 1)]
public class GrantWeapon : Powerup
{
    public Weapon weapon;
    public override void OnPickUp()
    {
        base.OnPickUp();

        if (weapon._name == PlayerManager.Instance.player.hand.hand._name)
        {
            Weapon w = WeaponLibrary.Instance.Upgrade(PlayerManager.Instance.player.hand.hand);
            PlayerManager.Instance.player.hand.Equip(w);
        }
        else
            PlayerManager.Instance.player.hand.Equip(weapon);

    }
}
