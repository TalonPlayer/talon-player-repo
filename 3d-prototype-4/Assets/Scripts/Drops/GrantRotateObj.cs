using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Give Rotating Object", menuName = "Drop/PowerUp/Give Rotating Object", order = 1)]
public class GrantRotateObj : Powerup
{
    public RotatingObject obj;

    /// <summary>
    /// Creates a spinning object a round the player
    /// </summary>
    public override void OnPickUp(Player player)
    {
        base.OnPickUp(player);

        RotatingObject instance = Instantiate(obj);

        instance.player = player;
    }
    
}
