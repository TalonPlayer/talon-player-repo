using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropShadow : MonoBehaviour
{
    public Transform player;
    public float offset = .02f;
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(player.transform.position, Vector3.down, out hit, 200f, Layer.Ground))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + offset, transform.position.z);
        }
    }
}
