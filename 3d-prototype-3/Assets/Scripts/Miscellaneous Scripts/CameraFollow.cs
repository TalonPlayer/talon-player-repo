using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerHead;
    void Update()
    {
        transform.position = playerHead.transform.position;
        playerHead.rotation = transform.rotation;
    }
}
