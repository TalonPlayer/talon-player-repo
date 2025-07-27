using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public GameObject containerPrefab;
    private GameObject containerObj;
    private List<Cell> cells = new List<Cell>();
    public bool isOpened = false;
    void Start()
    {
        containerObj = Instantiate(containerPrefab, HudManager.Instance.inventoryScreen.transform);
        FormContainer();
    }

    void Update()
    {
        if (isOpened)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Close();
            }
        }
    }

    public void Open()
    {
        isOpened = true;
        PlayerManager.Instance.player.enabled = false;
        PlayerManager.Instance.player.inventory.OnOpenInventory();
        HudManager.Instance.inventoryScreen.SetActive(true);
        HudManager.Instance.SetInteractText(true, "[E] to Close");
        containerObj.SetActive(true);
        SetContainer();
    }

    public void Close()
    {
        HudManager.Instance.inventoryScreen.SetActive(false);
        PlayerManager.Instance.player.enabled = true;
        PlayerManager.Instance.player.inventory.SetUpInventory();
        containerObj.SetActive(false);
        InventoryManager.Instance.mouseFollowerImage.gameObject.SetActive(false);

    }

    public void FormContainer()
    {
        for (int i = 0; i < containerObj.transform.childCount; i++)
        {
            cells.Add(containerObj.transform.GetChild(i).GetComponent<Cell>());
        }

    }

    public void SetContainer()
    {
        foreach (Cell cell in cells)
        {
            cell.SetCell();
        }
    }

}
