using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantDetection : MonoBehaviour
{
    public Plant plant;
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Robot")
        {
            plant.RobotDetected(other.gameObject);
        }
    }
}
