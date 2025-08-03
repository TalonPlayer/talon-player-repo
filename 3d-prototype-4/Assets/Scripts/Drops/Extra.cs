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
    /// 50/50 chance to give speed buff to enemies or player
    /// </summary>
    public void Speed()
    {
        int rand = Random.Range(0, 3);
        if (rand == 0)
            foreach (Enemy e in EntityManager.Instance.enemies)
                e.movement.AlterSpeed(2f);
        else
            PlayerManager.Instance.player.movement.SpeedBoost(2f, 25f);
    }

    public enum ExtraDrop
    {
        Sleep,
        Speed,
    }
}
