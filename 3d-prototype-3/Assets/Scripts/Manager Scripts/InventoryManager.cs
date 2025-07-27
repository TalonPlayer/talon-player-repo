using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public InventoryItem heldItem;
    public Image mouseFollowerImage;
    public TextMeshProUGUI countText;
    void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (heldItem != null && mouseFollowerImage != null)
        {
            mouseFollowerImage.transform.position = Input.mousePosition;
        }
    }

    public void DropSingle()
    {
        heldItem.Remove(1);

        TextFormatter();
    }

    public void TextFormatter()
    {
        if (heldItem.count > 1)
            countText.text = "" + heldItem.count;
        else
            countText.text = "";
    }

    public void PickUpItem(InventoryItem item)
    {
        heldItem = new InventoryItem(item.itemData, item.count);
        mouseFollowerImage.gameObject.SetActive(true);
        mouseFollowerImage.sprite = item.itemData.itemSprite;
        mouseFollowerImage.color = Color.white;
        TextFormatter();
    }

    public void PickUpItem(InventoryItem item, int count)
    {
        heldItem = new InventoryItem(item.itemData, count);
        mouseFollowerImage.gameObject.SetActive(true);
        mouseFollowerImage.sprite = item.itemData.itemSprite;
        mouseFollowerImage.color = Color.white;
        TextFormatter();
    }

    public void DropItem()
    {
        heldItem.itemData = null;
        mouseFollowerImage.gameObject.SetActive(false);
        mouseFollowerImage.sprite = null;
        mouseFollowerImage.color = new Color(1, 1, 1, 0); // Transparent
    }

    public InventoryItem ReplaceItem(Item item, int count)
    {
        return new InventoryItem(item, count);
    }

    public InventoryItem ReplaceItem()
    {
        return new InventoryItem(heldItem.itemData, heldItem.count);
    }
    public bool IsHoldingItem()
    {
        return heldItem.itemData != null;
    }
}
