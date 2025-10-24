using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Create Object", menuName = "Drop/PowerUp/Drop Creates Object", order = 1)]
public class CreateObjectDrop : Powerup
{
    public GameObject obj;

    /// <summary>
    /// Creates an object in the universal gameplay scene
    /// </summary>
    public override void OnPickUp(Player player)
    {
        base.OnPickUp(player);

        GameObject gameObj = Instantiate(obj, player.transform.position, Quaternion.identity);

        RotatingObject rotatingObject = gameObj.GetComponent<RotatingObject>();

        if (rotatingObject) rotatingObject.player = player;
    }
    
}