using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InWorldTextBox : MonoBehaviour
{
    // Text that appears when you get close to an object
    private Transform textObj;
    void Start()
    {
        textObj = transform.GetChild(0);
    }
    void OnTriggerEnter(Collider other)
    {
        textObj.gameObject.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        Destroy(gameObject);
    }
}
