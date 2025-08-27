using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPointer : MonoBehaviour
{
    public Image pointer;
    public Transform target;
    public bool isTargeting = false;
    void Update()
    {
        if (isTargeting)
        {
            // Pointer will attempt to hover above the objective
            float minX = pointer.GetPixelAdjustedRect().width / 2;
            float maxX = Screen.width - minX;

            float minY = pointer.GetPixelAdjustedRect().height / 2;
            float maxY = Screen.height - minY;

            Vector3 pos = Camera.main.WorldToScreenPoint(target.position);
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            pointer.transform.position = pos;
        }

    }

}
