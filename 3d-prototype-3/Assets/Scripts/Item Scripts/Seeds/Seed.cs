using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Seed Packet", menuName = "Items/Seeds/Seed Packet", order = 1)]
public class Seed : Item
{
    public GameObject seedlingPrefab;
    public override void PrimaryUse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            if (hit.collider.CompareTag("Soil"))
            {
                Soil soil = hit.collider.GetComponent<Soil>();
                Vector3 pos = hit.collider.transform.position;
                pos.y += .2f;

                GameObject obj = Instantiate(seedlingPrefab, pos, Quaternion.identity, soil.transform);
                Seedling seedling = obj.GetComponent<Seedling>();
                soil.PlantSeed(seedling);
            }
        }
    }

    public override void SecondaryUse(){}
    public override bool Validation()
    {
        showIndicator = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            if (hit.collider.tag == "Soil")
            {
                HudManager.Instance.SetItemIntText(true, primaryText);
                Soil soil = hit.collider.GetComponent<Soil>();
                if (soil.hasPlant) return false;

                showIndicator = true;
                indicatorPos = hit.collider.transform.position;
                indicatorPos.y += .05f;
                return true;
            }
        }
        HudManager.Instance.SetItemIntText(false, "");
        return false;
    }
}
