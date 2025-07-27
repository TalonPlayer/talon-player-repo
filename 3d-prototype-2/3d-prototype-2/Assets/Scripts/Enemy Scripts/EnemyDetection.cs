using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public Enemy enemy;

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            RaycastHit hit;

            Vector3 direction = other.transform.position - transform.position;
            Ray ray = new Ray(transform.position, direction);
            if (Physics.Raycast(ray, out hit, direction.magnitude)){
                if (hit.collider.tag == "Player"){
                    enemy.onAggro.Invoke();
                }
            }
        }
    }
}
