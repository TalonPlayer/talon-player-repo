using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantHarvest : MonoBehaviour
{
    public Item item;
    public int amount;

    public void Harvest()
    {
        PlayerManager.Instance.player.inventory.RecieveItem(item, amount);
    }
}
