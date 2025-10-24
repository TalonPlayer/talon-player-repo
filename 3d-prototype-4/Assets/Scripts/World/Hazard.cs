using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            if (!player.stats.isImmune)
                player.KillPlayer();
        }
        else if (other.tag == "Enemy")
        {
            Enemy e = other.GetComponent<Enemy>();
            e.OnHit(9999);
        }
        else if (other.tag == "Unit")
        {
            Unit u = other.GetComponent<Unit>();
            u.OnHit(9999);
        }
    }
}
