using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBarrier : MonoBehaviour
{
    // Kills entities that get touch it
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

            PlayerManager.Instance.TeleportPlayer(WorldManager.Instance.worldCenter);


            Invoke(nameof(KillPlayer), 1f);

        }

        else if (other.tag == "Enemy")
        {
            Enemy e = other.GetComponent<Enemy>();
            int count = WorldManager.Instance.currentCount;
            count--;
            count = Mathf.Max(count, 0);
            WorldManager.Instance.currentCount = count;
            e.contributeToCount = false;
            e.OnHit(9999);
            PlayerManager.Instance.player.info.kills--;
        }
        else if (other.tag == "Unit")
        {
            Unit u = other.GetComponent<Unit>();

            u.OnHit(9999);
        }
    }

    void KillPlayer()
    {
        PlayerManager.Instance.KillPlayer();
    }
}
