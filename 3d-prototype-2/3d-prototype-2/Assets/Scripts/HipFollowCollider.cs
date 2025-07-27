using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HipFollowCollider : MonoBehaviour
{
    public Transform enemy;

    void Update() {
        transform.position = enemy.transform.position;
    }
}
