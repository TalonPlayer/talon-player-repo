using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Watering Can", menuName = "Items/Tools/Watering Can", order = 1)]
public class WateringCan : Tool
{
    public int fill;

    public override void PrimaryUse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            if (hit.collider.CompareTag("Soil"))
            {
                Soil soil = hit.collider.GetComponent<Soil>();
                soil.Water();

            }
        }
    }

    public override void SecondaryUse()
    {
        // Refill if looking at water
    }

    public override bool Validation()
    {
        showIndicator = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            if (hit.collider.tag == "Soil")
            {
                Soil soil = hit.collider.GetComponent<Soil>();
                if (!soil.isWatered)
                {
                    HudManager.Instance.SetItemIntText(true, primaryText);
                    showIndicator = true;
                    indicatorPos = hit.collider.transform.position;
                    return true;
                }
            }
        }
        // Else if aiming at Water
        HudManager.Instance.SetItemIntText(false, "");
        return false;
    }
}
