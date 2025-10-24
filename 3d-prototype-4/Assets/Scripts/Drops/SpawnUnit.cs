using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Equip Unit", menuName = "Drop/PowerUp/Spawn Unit", order = 1)]
public class SpawnUnit : Powerup
{
    public Unit unit;
    public int spawns = 1;
    public override void OnPickUp(Player player)
    {
        base.OnPickUp(player);

        // Spawns the unit nearby the player
        for (int i = 0; i < spawns; i++)
        {
            Vector3 pos = player.transform.position;
            pos += RandExt.RandomDirection(0f, 360f);
            pos.y += 1f;
            Unit u = EntityManager.Instance.SpawnUnitReturn(pos, unit);
            u.owner = player;
        }
    }
}
