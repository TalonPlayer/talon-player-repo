using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Player main;
    public Weapon inHand;
    [HideInInspector] public bool isFiring;
    private bool canFire = true;
    public bool isMeleeing;
    public float meleeCooldown;
    public int meleeDamage = 200;
    public float adsSpeed = .25f;
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
        _layers = ~(Layer.Ally | Layer.Player | 1 << 2);
    }

    void Update()
    {
        if (!inHand) return;

        Firing();
        AimDownSights();
    }
    public void ShootWeapon()
    {
        if (!inHand.IsReady || isReloading || isMeleeing || !canFire) return;

        inHand.Shoot("Enemy", _layers, main.body.head.position, main.body.head.forward, main, isAiming);

        CalculateRecoil();
        HUDManager.UpdateAmmoText(inHand.currentBulletCount, inHand.currentRounds);

        if (inHand.weaponType == WeaponType.Semi || inHand.weaponType == WeaponType.Pump) canFire = false;
    }

    public void AimDownSights()
    {
        if (isReloading || isMeleeing)
        {
            main.body.ADSFade(false, adsSpeed);
            HUDManager.ToggleCursor(true);
            return;
        }

        isAiming = main.isSecondaryFiring;
        HUDManager.ToggleCursor(!isAiming);
        main.body.ADSFade(isAiming, adsSpeed);
        recoilReduction = isAiming ? 2f : 4f;
    }

    public void Melee()
    {
        if (isMeleeing) return;

        StartCoroutine(MeleeRoutine());
        if (isReloading)
        {
            HUDManager.ToggleReloadInfo(false);
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
        if (isReloading || isMeleeing) return;
        if (inHand.currentBulletCount <= inHand.maxBulletCount && inHand.currentRounds > 0)
        {

            reloadRoutine = StartCoroutine(ReloadRoutine());
        }
    }
    IEnumerator ReloadRoutine()
    {
        HUDManager.ToggleReloadInfo(true);

        isReloading = true;

        float start = 0f;
        float t = 0f;
        float r = inHand.reloadTime;

        if (inHand.reloadType == ReloadType.All)
        {

            while (t < 1f)
            {
                if (!isReloading) break;

                float fill = Mathf.Lerp(start, 1f, t);
                t += Time.deltaTime / r;

                HUDManager.UpdateReloadBar(fill);
                yield return null;
            }
            if (isReloading) inHand.Reload();
            HUDManager.UpdateAmmoText(inHand.currentBulletCount, inHand.currentRounds);
        }
        else
        {
            while (inHand.currentBulletCount < inHand.maxBulletCount)
            {
                
                while (t < 1f)
                {
                    if (!isReloading) break; 
                    

                    float fill = Mathf.Lerp(start, 1f, t);
                    t += Time.deltaTime / r;

                    HUDManager.UpdateReloadBar(fill);
                    yield return null;
                }
                t = 0f;
                
                if (isReloading) inHand.SingleReload();
                else break;
                HUDManager.UpdateAmmoText(inHand.currentBulletCount, inHand.currentRounds);

                if (inHand.currentRounds <= 0) break;
                yield return null;
            }


        }
        HUDManager.ToggleReloadInfo(false);
        isReloading = false;

    }

    public void CalculateRecoil()
    {
        currentYawRecoil = ((Random.value - .5f) / 2) * inHand.recoilX;
        currentPitchRecoil = (Random.value - .5f) / 2;
        currentPitchRecoil *= inHand.recoilY;

        main.movement.AddRecoil(-currentYawRecoil, -Mathf.Abs(currentPitchRecoil));
    }
    public void Firing()
    {
        if (inHand.currentBulletCount <= 0 && !isReloading && !isMeleeing)
        {

            reloadRoutine = StartCoroutine(ReloadRoutine());
        }

        if (main.isPrimaryFiring && !isReloading && !isMeleeing && canFire)
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

        if (!main.isPrimaryFiring) canFire = true;
    }

    public void Equip(RuntimeWeapon w)
    {
        Weapon prev = null;
        if (inHand)
        {

            if (inHand.weaponName == w.weaponName)
            {
                inHand.currentRounds += w.bulletsInMag;
                inHand.currentRounds = Mathf.Min(inHand.currentRounds, inHand.maxRounds);
                HUDManager.UpdateAmmoText(inHand.currentBulletCount, inHand.currentRounds);
                return;
            }
            else prev = inHand;

        }

        if (isReloading)
        {
            HUDManager.ToggleReloadInfo(false);
            isReloading = false;

            StopCoroutine(reloadRoutine);
        }

        WeaponEffects fx = Instantiate(w.model);
        fx.coll.GetComponent<Outline>().SetOutlineActive(false);

        Weapon weapon = fx.gameObject.AddComponent<Weapon>();

        if (!fx) { Debug.LogWarning($"Weapon Effects not valid for {w.weaponName}"); return; }
        inHand = weapon;

        inHand.Init(w, fx);

        // Add logic if the player is carrying only one weapon, the current weapon gets put in off hand
        // Add logic for dropping weapons
        Vector3 size = inHand.transform.localScale;

        inHand.transform.parent = main.body.rightHand.transform;
        inHand.transform.localRotation = Quaternion.Euler(Vector3.zero);
        inHand.transform.localPosition = Vector3.zero;
        inHand.transform.localScale = size;

        inHand.gameObject.layer = 16;
        Transform[] children = inHand.transform.GetComponentsInChildren<Transform>();
        foreach (Transform c in children) c.gameObject.layer = 16;

        fx.rb.isKinematic = true;
        fx.rb.useGravity = false;
        fx.coll.enabled = false;

        main.body.SetWeapon(fx.weaponAnimator);
        main.onPrimaryInteraction -= ShootWeapon;
        main.onPrimaryInteraction += ShootWeapon;
        //main.onSecondaryInteraction -= AimDownSights;
        //main.onSecondaryInteraction += AimDownSights;
        HUDManager.UpdateWeaponText(w.weaponName);
        HUDManager.Instance.bulletSpread = w.bulletSpread;
        HUDManager.UpdateAmmoText(inHand.currentBulletCount, inHand.currentRounds);

        if (prev != null)
        {
            Weapon.DropHand(prev);
            HUDManager.InteractText("");
        }
    }
}
