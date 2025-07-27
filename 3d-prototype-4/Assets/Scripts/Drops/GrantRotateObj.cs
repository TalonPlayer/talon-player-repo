using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Give Rotating Object", menuName = "Drop/PowerUp/Give Rotating Object", order = 1)]
public class GrantRotateObj : Powerup
{
    public RotatingObject obj;

    public override void OnPickUp()
    {
        base.OnPickUp();

        RotatingObject instance = Instantiate(obj);
    }
    
}
