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

        switch (drop)
        {
            case ExtraDrop.Sleep:
                Sleep();
                break;
        }
    }

    public void Sleep()
    {
        foreach (Enemy e in EntityManager.Instance.enemies)
        {
            e.OnHit(9999);
        }
    }


    public enum ExtraDrop
    {
        Sleep,
    }
}
