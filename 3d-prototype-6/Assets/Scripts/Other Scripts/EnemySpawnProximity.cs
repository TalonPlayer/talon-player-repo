using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to check if an enemies can spawn in this location. 
/// If there player is nearby, this becomes inactive.
/// </summary>
public class EnemySpawnProximity : MonoBehaviour
{
    private bool isNotSeen = true;
    private int layers;
    void Start()
    {
        layers = Layer.Ground | Layer.Wall;
        WaveManager.onProximity += ViewedByPlayer;
    }
    private void ViewedByPlayer()
    {
        Vector3 playerPos = PlayerManager.Instance.PlayerPos;
        if (Physics.Linecast(playerPos, transform.position, layers, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawLine(playerPos, transform.position, Color.red, .15f);

            if (isNotSeen)
            {
                WaveManager.AddSpawn(transform);
                isNotSeen = false;
            }
        }
        else
        {
            Debug.DrawLine(playerPos, transform.position, Color.green, .15f);

            if (!isNotSeen)
            {
                WaveManager.RemoveSpawn(transform);
                isNotSeen = true;
            }
        }
    }
}
