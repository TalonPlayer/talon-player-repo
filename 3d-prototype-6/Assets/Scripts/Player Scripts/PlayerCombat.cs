using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Player main;
    public Weapon inHand;
    [HideInInspector] public bool isFiring;
    public bool canFire;
    public bool isMeleeing;
    public float meleeCooldown;
    public int meleeDamage = 200;
    public bool isAiming;
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
        if (inHand)
        {
            Equip(inHand);
        }

        _layers = ~(Layer.Ally | Layer.Player | 1 << 2);
    }

    void Update()
    {
        Firing();
        AimDownSights();
    }
    public void ShootWeapon()
    {
        if (isReloading) return;
        if (isMeleeing) return;
        if (!inHand.IsReady) return;

        inHand.Shoot("Enemy", _layers, main.body.head.position, main.body.head.forward, main, isAiming);

        CalculateRecoil();
        HUDManager.UpdateAmmoText(inHand.currentBulletCount, inHand.currentRounds);
    }

    public void AimDownSights()
    {
        if (isReloading)
        {
            main.body.PlayWeapon("IsAiming", false);
            return;
        }

        isAiming = main.isSecondaryFiring;
        main.body.PlayWeapon("IsAiming", isAiming);
        recoilReduction = isAiming ? 2f : 4f;
    }

    public void Melee()
    {
        if (isMeleeing) return;

        StartCoroutine(MeleeRoutine());
        if (isReloading)
        {
            isReloading = false;
            StopCoroutine(reloadRoutine);
        }
    }



    IEnumerator MeleeRoutine()
    {
        isMeleeing = true;

        if (Physics.Raycast(transform.position, main.body.head.forward, out RaycastHit hit, 2f, _layers, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                float damage = meleeDamage;
                BodyPart b = hit.collider.GetComponent<BodyPart>();
                Entity e = hit.collider.GetComponent<Entity>();

                if (b)
                {
                    e = b.main;
                    damage = Mathf.RoundToInt(b.damageMult * meleeDamage);
                }

                if (e) e.OnHit(Mathf.RoundToInt(damage), main);
            }
        }
        yield return new WaitForSeconds(meleeCooldown);

        isMeleeing = false;

    }

    public void Reload()
    {
        if (isReloading) return;
        if (inHand.currentBulletCount <= inHand.maxBulletCount && inHand.currentRounds > 0)
            reloadRoutine = StartCoroutine(ReloadRoutine());
    }
    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(inHand.reloadTime);
        isReloading = false;
        inHand.Reload();
        HUDManager.UpdateAmmoText(inHand.currentBulletCount, inHand.currentRounds);
    }

    public void CalculateRecoil()
    {
        currentYawRecoil = ((Random.value - .5f) / 2) * inHand.recoilX;
        currentPitchRecoil = (Random.value - .5f) / 2;
        if (timePressed >= inHand.maxRecoilTime || isAiming)
            currentPitchRecoil *= inHand.recoilY / 4f;
        else
            currentPitchRecoil *= inHand.recoilY;

        main.movement.AddRecoil(-currentYawRecoil, -Mathf.Abs(currentPitchRecoil));
    }
    public void Firing()
    {
        if (inHand.currentBulletCount <= 0 && !isReloading) reloadRoutine = StartCoroutine(ReloadRoutine());

        if (main.isPrimaryFiring && !isReloading && inHand)
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
    }

    public void Equip(Weapon weapon)
    {
        // Add logic if the player is carrying only one weapon, the current weapon gets put in off hand
        // Add logic for dropping weapons
        weapon.transform.parent = main.body.rightHand.transform;
        weapon.transform.rotation = main.body.transform.rotation;
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localScale = Vector3.one;

        weapon.gameObject.layer = 16;

        inHand = weapon;
        if (weapon.fx.weaponAnimator)
            main.body.SetWeapon(weapon.fx.weaponAnimator);
        main.onPrimaryInteraction -= ShootWeapon;
        main.onPrimaryInteraction += ShootWeapon;
        //main.onSecondaryInteraction -= AimDownSights;
        //main.onSecondaryInteraction += AimDownSights;
        inHand.Init();
        HUDManager.UpdateWeaponText(inHand.weaponName);


    }
}
