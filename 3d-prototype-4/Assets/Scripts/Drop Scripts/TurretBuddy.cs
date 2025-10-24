using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Give Turret Buddy", menuName = "Drop/PowerUp/Give Turret Buddy", order = 1)]
public class TurretBuddy : Powerup
{
    public RotatingTurret turretPrefab;

    public override void OnPickUp(Player player)
    {
        base.OnPickUp(player);

        RotatingTurret turret = Instantiate(turretPrefab);
        turret.player = player;
        player.hand.turrets.Add(turret);
    }
}
