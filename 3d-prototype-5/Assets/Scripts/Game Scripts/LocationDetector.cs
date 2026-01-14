using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocationDetector : MonoBehaviour
{
    public TextMeshProUGUI locationText;
    public TextMeshProUGUI progressText;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Location")
        {
            locationText.text = "Location: " + other.name;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Location")
        {
            locationText.text = "Nearby: " + other.name;
        }
    }
}
