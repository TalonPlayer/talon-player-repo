using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFollowPlayer : MonoBehaviour
{
    // An object that follows the player but maintans y position
    private Transform player;
    void Start()
    {
        player = PlayerManager.Instance.player.transform;
    }

    void Update()
    {
        Vector3 pos = player.position;
        pos.y = transform.position.y;

        transform.position = pos;
    }
}
