using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Money Drop", menuName = "Drop/Money", order = 1)]
public class MoneyDrop : Drop
{
    public int scoreValue;
    public int multiplierValue;
    public override void OnPickUp()
    {
        base.OnPickUp();

        // Give the player some score and multiplier
        PlayerManager.Instance.AddScore(scoreValue);
        PlayerManager.Instance.AddMultiplier(multiplierValue);
    }
}
