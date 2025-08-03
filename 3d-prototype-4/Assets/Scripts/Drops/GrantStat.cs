using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Give Stat", menuName = "Drop/PowerUp/GiveStat", order = 1)]
public class GrantStat : Powerup
{
    public Type type;
    public override void OnPickUp()
    {
        base.OnPickUp();

        // Gives the player the type of stat
        switch (type)
        {
            case Type.life:
                PlayerManager.Instance.GrantLife();
                break;
            case Type.dash:
                PlayerManager.Instance.GrantDash();
                break;
            case Type.nuke:
                PlayerManager.Instance.GrantNuke();
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
