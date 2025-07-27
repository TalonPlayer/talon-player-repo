using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private Enemy gunner;
    void Start()
    {
        gunner = GetComponent<Enemy>();
    }
    void Update()
    {
        if (gunner.isAggro){
            
            if (GameManager.Instance.player.isAlive)
                HUDController.Instance.PlayDeathAnim(true);
                WorldController.Instance.Reset();
        }
    }
}
