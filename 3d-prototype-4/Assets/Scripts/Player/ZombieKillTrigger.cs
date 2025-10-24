using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieKillTrigger : MonoBehaviour
{
    
    /// <summary>
    /// Kill only enemies
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Enemy e = other.GetComponent<Enemy>();

            e.OnHit(9999);
        }
    }
}
