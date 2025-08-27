using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Extra Gimmick", menuName = "Drop/PowerUp/Extra Gimmicks", order = 1)]
public class Extra : Powerup
{
    public ExtraDrop drop;

    public override void OnPickUp()
    {
        base.OnPickUp();

        // Extra drop gimmicks so that way I dont have to create lots of scripts
        switch (drop)
        {
            case ExtraDrop.Sleep:
                Sleep();
                break;
            case ExtraDrop.Speed:
                Speed();
                break;
            case ExtraDrop.Shield:
                Shield();
                break;
        }
    }

    /// <summary>
    /// Kill all enemies spawned
    /// </summary>
    public void Sleep()
    {
        foreach (Enemy e in EntityManager.Instance.enemies)
            e.OnHit(9999);
    }

    /// <summary>
    /// Increase speed of current on screen but also permanently increase player movespeed by percentage
    /// </summary>
    public void Speed()
    {
        foreach (Enemy e in EntityManager.Instance.enemies)
            e.movement.AlterSpeed(1.5f);
        PlayerManager.Instance.player.movement.AlterSpeed(.05f);
    }

    /// <summary>
    /// Give the player a shield
    /// </summary>
    public void Shield()
    {
        PlayerManager.Instance.GrantShield();
    }

    public enum ExtraDrop
    {
        Sleep,
        Speed,
        Shield
    }
}
