using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropText : MonoBehaviour
{
    // This script is used to destroy the text that tells you what powerup picked up
    public void DestroyText()
    {
        Destroy(transform.parent.gameObject);
    }
}
