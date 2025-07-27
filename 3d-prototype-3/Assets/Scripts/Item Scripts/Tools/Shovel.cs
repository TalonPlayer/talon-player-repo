using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shovel", menuName = "Items/Tools/Shovel", order = 1)]
public class Shovel : Tool
{
    public GameObject soilPrefab;
    public float soilRadius = 1f;
    public override void PrimaryUse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            if (hit.collider.tag == "Dirt")
            {
                GameObject obj = Instantiate(soilPrefab, hit.point, Quaternion.identity);
                Soil s = obj.GetComponent<Soil>();

                BotanyManager.Instance.soil.Add(s);
            }
        }
    }
    public override void SecondaryUse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            if (hit.collider.tag == "Soil")
            {
                Soil s = hit.collider.GetComponent<Soil>();
                BotanyManager.Instance.soil.Remove(s);
                Destroy(hit.collider.gameObject);
            }
        }
    }

    public override bool Validation()
    {
        showIndicator = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            if (hit.collider.tag == "Dirt")
            {
                HudManager.Instance.SetItemIntText(true, primaryText);

                Collider[] overlaps = Physics.OverlapSphere(hit.point, soilRadius);
                foreach (Collider col in overlaps)
                {
                    if (col.CompareTag("Soil"))
                    {
                        return false;
                    }
                }
                showIndicator = true;
                indicatorPos = hit.point;
                return true;
            }
            else if (hit.collider.tag == "Soil")
            {
                indicatorPos = hit.collider.transform.position;
                HudManager.Instance.SetItemIntText(true, secondaryText);
                return true;
            }
        }
        HudManager.Instance.SetItemIntText(false, "");
        return false;
    }
}
