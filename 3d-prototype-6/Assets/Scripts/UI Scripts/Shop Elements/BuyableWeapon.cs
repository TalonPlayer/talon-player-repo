using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuyableWeapon : ShopElement, IDuplicateItem
{
    public WeaponObj weapon;
    private bool isOwned = false;
    protected override void Start()
    {
        base.Start();
        PlayerManager.onWeaponChange += OnWeaponChange;
        OnWeaponChange();
    }
    protected override void Update()
    {
        if (!isOwned) base.Update();
        else DuplicateItem();
    }

    protected override void OnPay()
    {
        base.OnPay();

        RuntimeWeapon wpn = new RuntimeWeapon(weapon);
        player.combat.Equip(wpn);
    }

    private void OnWeaponChange()
    {
        foreach (Weapon w in player.combat.weaponSlots)
        {
            if (!w) continue;
            if (w.weaponName == weapon.weaponName)
            {
                isOwned = true;
                return;
            }
        }

        isOwned = false;
    }

    public void DuplicateItem()
    {
        isAvailable = false;
        button.interactable = true;
        Color clr = ShopHUDManager.GetTextColor(2);
        costText.color = clr;
        itemText.color = clr;
        itemText.text = "(Owned) " + name;

        button.interactable = false;

    }

}
