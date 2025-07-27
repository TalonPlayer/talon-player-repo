using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Food", menuName = "Items/Consumable/Food", order = 1)]
public class Food : Item
{
    public int sellingPrice;
    public int healAmount;
    public override void PrimaryUse()
    {

    }

    public override void SecondaryUse(){}

    public override bool Validation()
    {
        HudManager.Instance.SetItemIntText(true, primaryText);

        return true;
    }
}
