using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public Item itemData;      // Reference to ScriptableObject
    public int count = 1;      // Stack count

    public InventoryItem(Item data, int startingCount = 1)
    {
        itemData = data;
        count = startingCount;
    }

    public void Add(int amount) => count += amount;
    public void Remove(int amount) => count -= amount;
}

