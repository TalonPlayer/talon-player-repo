using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishGameDemo : MonoBehaviour
{
    // This class ends the game if the player reaches the end of the demo
    public void EndGame()
    {
        foreach (Player player in PlayerManager.Instance.players)
        {
            player.movement.enabled = false;
            player.hand.enabled = false;
        }
        
        PlayerManager.Instance.EndGame();
    }
}
