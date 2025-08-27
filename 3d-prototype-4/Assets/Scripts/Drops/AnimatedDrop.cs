using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Animated Drop", menuName = "Drop/PowerUp/Animated Drop", order = 1)]
public class AnimatedDrop : Powerup
{
    public Animator animator;
    public override void OnPickUp()
    {
        base.OnPickUp();

        // This drop plays an animation after it gets picked up
        Animator a = Instantiate(animator, PlayerManager.Instance.player.transform.position, PlayerManager.Instance.player.transform.rotation);
        a.SetTrigger("Play");
    }
}
