using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishGameDemo : MonoBehaviour
{
    // This class ends the game if the player reaches the end of the demo
    public Animator animator;
    public void EndGame()
    {
        PlayerManager.Instance.player.movement.enabled = false;
        PlayerManager.Instance.player.hand.enabled = false;
        animator.SetTrigger("Play");
        PlayerManager.Instance.EndGame();
    }
}
