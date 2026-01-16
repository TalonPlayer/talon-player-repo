using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Runtime Instantiated weapon
/// </summary>
public class Weapon : MonoBehaviour
{
    [Header("Weapon Info")]
    public string weaponName;
    public string weaponFamily; // If upgradeable weapons are implemented
    public WeaponType weaponType;
    public ReloadType reloadType;
    public bool isDual;

    [Header("Weapon Stats")]
    public int currentBulletCount = 0;
    public int maxBulletCount;
    public int currentRounds = 0;
    public int maxRounds;
    public int baseDamage;
    [Range(1, 15)]
    public int bulletsPerShot = 1;
    public float fireRate, reloadTime, critMult, bulletSpread;
    [Range(0, 10f)]
    public float recoilX;
    [Range(0, 50f)]
    public float recoilY;
    [Range(0, 10f)]
    public float maxRecoilTime = 1f;
    private float _lastShootTime;
    [Header("Extras - Leave empty if can't be applied")]
    public float explosionRadius;
    public bool HasAmmo { get { return currentBulletCount > 0; } }
    public int CurrentMags { get { return currentRounds / maxBulletCount; } }
    public bool HasMags { get { return CurrentMags > 0; } }
    public bool OutOfAmmo { get { return !HasAmmo && !HasMags; } }
    public bool IsReady { get { return _lastShootTime + fireRate < Time.time; } }
    public WeaponEffects fx;
    public void Init(RuntimeWeapon w, WeaponEffects f)
    {
        gameObject.name = w.weaponName;
        weaponName = w.weaponName;
        weaponFamily = w.weaponFamily;
        weaponType = w.weaponType;
        reloadType = w.reloadType;
        baseDamage = w.baseDamage;
        critMult = w.critMult;
        maxBulletCount = w.bulletsInMag;
        maxRounds = w.numOfMags * maxBulletCount;
        bulletsPerShot = w.bulletsPerShot;
        bulletSpread = w.bulletSpread;
        fireRate = w.fireRate;
        reloadTime = w.reloadTime;
        recoilX = w.recoilX;
        recoilY = w.recoilY;
        maxRecoilTime = w.maxRecoilTime;
        fx = f;

        if (w.used)
        {
            currentBulletCount = w.currentBulletCount;
            currentRounds = w.currentRounds;
        }
        else
        {
            currentBulletCount = maxBulletCount;
            currentRounds = maxRounds;
        }
    }
    public void Shoot(string tag, LayerMask layers, Vector3 headPos, Vector3 aimDir, Entity attacker, bool isAiming = false)
    {
        if (_lastShootTime + fireRate < Time.time)
        {
            Vector3 baseDir;
            for (int i = 0; i < bulletsPerShot; i++)
            {

                if (isAiming) baseDir = Helper.SpreadRandomizer(aimDir, bulletSpread / 2f);
                else baseDir = Helper.SpreadRandomizer(aimDir, bulletSpread);
                if (Physics.Raycast(headPos, baseDir, out RaycastHit hit, 100f, layers, QueryTriggerInteraction.Ignore))
                {
                    Action onHit = null;

                    if (hit.collider.CompareTag(tag))
                    {
                        float damage = baseDamage;
                        BodyPart b = hit.collider.GetComponent<BodyPart>();
                        Entity e = hit.collider.GetComponent<Entity>();

                        if (b)
                        {
                            e = b.main;
                            damage = Mathf.RoundToInt(b.damageMult * baseDamage);
                        }

                        damage += Mathf.RoundToInt(critMult * baseDamage);
                        if (e) onHit = () => e.OnHit(Mathf.RoundToInt(damage), attacker);
                    }

                    StartCoroutine(fx.SpawnTrail(hit, onHit));
                }
                else StartCoroutine(fx.SpawnTrail(headPos + fx.barrelPoint.forward * 50f));
            }
            currentBulletCount--;
            _lastShootTime = Time.time;
        }
    }
    public void Reload()
    {

        if (CurrentMags > 0)
        {
            int remaining = maxBulletCount - currentBulletCount;
            currentBulletCount = maxBulletCount;
            currentRounds -= remaining;
        }
        else
        {
            int remaining = maxBulletCount - currentBulletCount;
            currentBulletCount += remaining;
            currentRounds = 0;
        }

    }

    public static void DropHand(Weapon weapon)
    {
        if (weapon)
        {
            weapon.transform.parent = EntityManager.Instance.weaponDropFolder;
            weapon.fx.rb.isKinematic = false;
            weapon.fx.rb.useGravity = true;
            weapon.fx.coll.enabled = true;
            weapon.fx.coll.GetComponent<Outline>().SetOutlineActive(false);;
            weapon.gameObject.layer = 15;
            Transform[] children = weapon.transform.GetComponentsInChildren<Transform>();
            foreach (Transform c in children) c.gameObject.layer = 15;
        }
    }
}
public enum WeaponType
{
    Semi, // Press once to shoot
    Pump, // Press once to shoot then must wait to pump
    Auto, // Hold down shoot
    Melee,
}

public enum ReloadType
{
    Individual,
    All,
}