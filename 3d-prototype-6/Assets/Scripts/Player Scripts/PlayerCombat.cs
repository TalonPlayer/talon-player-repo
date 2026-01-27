using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Player main;
    public Weapon inHand;
    public Weapon[] weaponSlots;
    public int wpnIndex = -1;

    public float adsSpeed = .25f;

    public bool canFire = true;
    public bool isAiming = false;
    public bool nonAutoRefresh = false;
    public Action swappedInWeapon;
    private bool isFiring;
    private bool isReloading;
    private float recoilReduction;
    private float currentPitchRecoil;
    private float currentYawRecoil;
    private float timePressed;
    private LayerMask _layers;
    Coroutine reloadRoutine;
    // public Weapon offHand;
    void Awake()
    {
        main = GetComponent<Player>();
    }

    void Start()
    {
        _layers = ~(Layer.Ally | Layer.Player | 1 << 2);
    }
    public void Init(int weaponSlot)
    {
        ClearInventory();
        weaponSlots = new Weapon[weaponSlot];
    }

    void Update()
    {
        if (!inHand) return;

        Firing();
        AimDownSights();
    }
    public void ShootWeapon()
    {
        if (!canFire || !inHand.HasAmmo) return;

        inHand.Shoot("Enemy", _layers, main.body.head.position, main.body.head.forward, main, isAiming);
        main.body.PlayWeapon("Shoot");
        CalculateRecoil();
        HUDManager.UpdateAmmoText(inHand.currentBulletCount, inHand.currentRounds);
        canFire = false;
        if (inHand.currentBulletCount <= 0)
            main.body.PlayWeapon("No Bullets", true);
    }

    public void AimDownSights()
    {
        if (!canFire) return;
        if (!main.isSecondaryFiring && isAiming)
        {
            isAiming = false;
            main.body.ADSFade(false, adsSpeed);
            HUDManager.ToggleCursor(true);
        }
        else if (main.isSecondaryFiring && !isAiming)
        {
            isAiming = true;
            main.body.ADSFade(true, adsSpeed);
            HUDManager.ToggleCursor(false);
        }


        recoilReduction = isAiming ? 2f : 4.5f;
    }

    public void Reload()
    {
        if (!canFire || !inHand.CanReload) return;

        main.body.PlayWeapon("IsReloading", true);
        canFire = false;
    }

    public void FullReload()
    {
        inHand.Reload();
        HUDManager.UpdateAmmoText(inHand.currentBulletCount, inHand.currentRounds);
        canFire = true;
    }

    public void CalculateRecoil()
    {
        currentYawRecoil = ((UnityEngine.Random.value - .5f) / 2) * inHand.recoilX;
        currentPitchRecoil = (UnityEngine.Random.value - .5f) / 2;

        if (timePressed >= inHand.maxRecoilTime)
            currentPitchRecoil *= inHand.recoilY;
        else
            currentPitchRecoil *= inHand.recoilY / 4f;

        main.movement.AddRecoil(-currentYawRecoil, -Mathf.Abs(currentPitchRecoil));
    }
    public void Firing()
    {
        if (main.isPrimaryFiring && !isReloading && canFire)
        {
            timePressed += Time.deltaTime;
            timePressed = Mathf.Min(timePressed, inHand.maxRecoilTime);
            isFiring = true;
        }
        else
        {
            isFiring = false;
            timePressed = 0;
        }

        if (!main.isPrimaryFiring && nonAutoRefresh)
        {
            canFire = true;
            nonAutoRefresh = false;
        }
    }

    public void Swap(int increm)
    {
        if (inHand == null) return;

        int newIndex = wpnIndex + increm;

        // +1
        if (newIndex >= weaponSlots.Length)
            newIndex = 0;
        // -1
        else if (newIndex < 0)
            newIndex = weaponSlots.Length - 1;

        Weapon newWeapon = weaponSlots[newIndex];
        if (newWeapon)
        {
            if (newWeapon == inHand) return;
            wpnIndex = newIndex;
            SwapOutWeapon(inHand);
            swappedInWeapon = () =>
            {

                SwapInWeapon(newWeapon);
            };
        }
    }

    public int AvailableOccupancy()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] == null)
                return i;
        }
        // No available room, so replace current weapon
        return wpnIndex;
    }

    public void SwapOutWeapon(Weapon weapon)
    {
        weapon.gameObject.SetActive(false);
        canFire = false;
        main.body.weaponAnimator.CrossFade("Swap Out", .25f);
    }

    public void SwapInWeapon(Weapon weapon)
    {
        inHand = weapon;

        weapon.gameObject.SetActive(true);
        main.body.SetWeapon(weapon.fx.weaponAnimator);
        main.body.weaponAnimator.Play("Swap In");

        main.onPrimaryInteraction -= ShootWeapon;
        main.onPrimaryInteraction += ShootWeapon;

        // Update the ui for the player
        HUDManager.UpdateWeaponText(inHand.weaponName);
        HUDManager.Instance.bulletSpread = inHand.bulletSpread;
        HUDManager.UpdateAmmoText(inHand.currentBulletCount, inHand.currentRounds);
    }

    public void RefillAmmo()
    {
        foreach(Weapon w in weaponSlots) w.RefillWeapon();

        HUDManager.UpdateAmmoText(inHand.currentBulletCount, inHand.currentRounds);
    }

    public void ClearInventory()
    {
        Transform[] c = Helper.GetChildrenArray(main.body.rightHand);
        foreach (Transform t in c) Destroy(t.gameObject);
    }

    public void Equip(RuntimeWeapon w)
    {
        Weapon prev = null;
        int handIndex = AvailableOccupancy();

        // If has weapon in hand
        if (inHand)
        {
            // If the weapon is in the inventory, just take the ammo
            Weapon wpn = Array.Find(weaponSlots, wp =>
            {
                if (!wp) return false;
                return wp.weaponName == w.weaponName;  
            });
            if (wpn)
            {
                wpn.currentRounds += w.bulletsInMag;
                wpn.currentRounds = Mathf.Min(wpn.currentRounds, wpn.maxRounds);
                HUDManager.UpdateAmmoText(inHand.currentBulletCount, inHand.currentRounds);
                return;
            }

            // Queue the previous hand to be dropped or swapped.
            else
            {
                // There is free room here, so swap current hand and equip this.
                if (weaponSlots[handIndex] == null)
                    SwapOutWeapon(inHand);
                else    // There is no room, so queue the previous to drop.
                    prev = inHand;
            }
        }

        WeaponEffects fx = Instantiate(w.model);
        fx.coll.GetComponent<Outline>().SetOutlineActive(false);

        Weapon weapon;
        if (w.used) weapon = fx.GetComponent<Weapon>();
        else weapon = fx.gameObject.AddComponent<Weapon>();

        if (!fx) { Debug.LogWarning($"Weapon Effects not valid for {w.weaponName}"); return; }
        inHand = weapon;

        inHand.Init(w, fx);

        // Set the location to the player's hand
        Vector3 size = inHand.transform.localScale;

        inHand.transform.parent = main.body.rightHand.transform;
        inHand.transform.localRotation = Quaternion.Euler(Vector3.zero);
        inHand.transform.localPosition = Vector3.zero;
        inHand.transform.localScale = size;

        // Set the layer to UI overlay
        inHand.gameObject.layer = 16;
        Transform[] children = inHand.transform.GetComponentsInChildren<Transform>();
        foreach (Transform c in children) c.gameObject.layer = 16;

        fx.rb.isKinematic = true;
        fx.rb.useGravity = false;
        fx.coll.enabled = false;

        // Make it so other scripts recognize that this is the weapon now
        weaponSlots[handIndex] = inHand; wpnIndex = handIndex;
        PlayerManager.Instance.WeaponSlotsChanged();
        main.body.SetWeapon(fx.weaponAnimator);
        main.body.weaponAnimator.Play("Equip");
        main.onPrimaryInteraction -= ShootWeapon;
        main.onPrimaryInteraction += ShootWeapon;

        // Update the ui for the player
        HUDManager.UpdateWeaponText(inHand.weaponName);
        HUDManager.Instance.bulletSpread = inHand.bulletSpread;
        HUDManager.UpdateAmmoText(inHand.currentBulletCount, inHand.currentRounds);

        // Allow the equip animation to player
        canFire = false;

        // If a previous weapon was queued, drop it.
        if (prev != null)
        {
            Weapon.DropHand(prev);
            HUDManager.InteractText("");
        }
    }
}
