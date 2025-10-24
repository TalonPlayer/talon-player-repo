using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Give Stat", menuName = "Drop/PowerUp/GiveStat", order = 1)]
public class GrantStat : Powerup
{
    public Type type;
    public override void OnPickUp(Player player)
    {
        base.OnPickUp(player);

        // Gives the player the type of stat
        switch (type)
        {
            case Type.life:
                player.stats.GrantLife();
                break;
            case Type.dash:
                player.stats.GrantDash();
                break;
            case Type.nuke:
                player.stats.GrantNuke();
                break;
        }
    }
    public enum Type
    {
        life,
        dash,
        nuke
    }
}
