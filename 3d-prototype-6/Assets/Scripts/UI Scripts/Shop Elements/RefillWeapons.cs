using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefillWeapons : ShopElement, IDuplicateItem
{
    private bool isFull = true;
    protected override void Update()
    {
        if (!isFull) base.Update();
        else
        {
            DuplicateItem();

            CheckAmmo();
        }
    }

    protected override void OnPay()
    {
        base.OnPay();

        player.combat.RefillAmmo();
    }

    private void CheckAmmo()
    {
        foreach (Weapon w in player.combat.weaponSlots)
        {
            if (!w) continue;
            if (!w.IsFull)
            {
                isFull = false;
                return;
            }
        }

        isFull = true;
    }

    public void DuplicateItem()
    {
        isAvailable = false;
        button.interactable = true;
        Color clr = ShopHUDManager.GetTextColor(2);
        costText.color = clr;
        itemText.color = clr;
        itemText.text = "(Full) " + name;

        button.interactable = false;
    }
}
