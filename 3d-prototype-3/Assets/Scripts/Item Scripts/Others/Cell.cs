using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Cell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public InventoryItem heldItem;
    public Image spaceImg;
    public Image borderImg;
    public TextMeshProUGUI countText;
    public Color highlightedColor;
    public Color defaultColor;
    public void OnPointerClick(PointerEventData eventData)
    {
        // If holding an item, try to place it
        if (InventoryManager.Instance.IsHoldingItem())
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (heldItem.itemData == null) // Empty cell
                {
                    heldItem = InventoryManager.Instance.ReplaceItem();
                    SetCell();
                    InventoryManager.Instance.DropItem();
                }
                else if (heldItem.itemData == InventoryManager.Instance.heldItem.itemData)
                {
                    heldItem.Add(InventoryManager.Instance.heldItem.count);
                    SetCell();
                    InventoryManager.Instance.DropItem();
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (heldItem.itemData == null) // Empty cell
                {
                    heldItem = InventoryManager.Instance.ReplaceItem();
                    InventoryManager.Instance.DropSingle();
                    heldItem.count = 1;
                    SetCell();
                }
                else if (heldItem.itemData == InventoryManager.Instance.heldItem.itemData)
                {
                    InventoryManager.Instance.DropSingle();
                    heldItem.Add(1);
                    SetCell();
                }

                if (InventoryManager.Instance.heldItem.count == 0)
                {
                    InventoryManager.Instance.DropItem();
                }
            }
        }
        else
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (heldItem.itemData != null)
                {
                    InventoryManager.Instance.PickUpItem(heldItem);
                    heldItem.itemData = null;
                    SetCell();
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (heldItem.itemData != null)
                {
                    int half = heldItem.count - (heldItem.count / 2);
                    InventoryManager.Instance.PickUpItem(heldItem, half);
                    heldItem.count = heldItem.count - half;
                    SetCell();
                }
            }

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        borderImg.color = highlightedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        borderImg.color = defaultColor;
    }

    public void SetCell()
    {
        borderImg.color = defaultColor;

        if (heldItem.itemData)
        {
            spaceImg.sprite = heldItem.itemData.itemSprite;
            spaceImg.color = new Color(255, 255, 255, 1);
            if (heldItem.count > 1)
                countText.text = "" + heldItem.count;
            else
                countText.text = "";
        }
        else
        {
            spaceImg.sprite = null;
            spaceImg.color = new Color(255, 255, 255, 0);
            countText.text = "";
        }
    }

    public void TextFormatter()
    {
        if (heldItem.count > 1)
            countText.text = "" + heldItem.count;
        else
            countText.text = "";
    }
}
