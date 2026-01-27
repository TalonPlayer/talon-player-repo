using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "New Weapon")]
public class WeaponObj : ScriptableObject
{
    [Header("Weapon Info")]
    public string weaponName;
    public string weaponFamily; // If upgradeable weapons are implemented
    public WeaponType weaponType;
    public ReloadType reloadType;

    [Header("Weapon Damage")]
    public int baseDamage;      // Base damage of gun
    public float critMult;      // Damage increase when landing a crit

    [Header("Bullets")]
    public int bulletsInMag;    // Amount of bullets in the clip
    public int numOfMags;       // Amount of mags. Will be multiplied by bullets in mag to get rounds count.
    public int bulletsPerShot;  // Amount of bullets that come out when fired once.
    [Range(0f, 1f)] public float bulletSpread;

    [Header("Weapon Handling")]
    public float fireRate;
    public float reloadTime;
    [Range(0, 10f)] public float recoilX;
    [Range(0, 50f)] public float recoilY;
    [Range(0, 10f)] public float maxRecoilTime;

    [Header("Weapon Assets")]
    public WeaponEffects model;
}

[System.Serializable]
public class RuntimeWeapon
{
    [Header("Weapon Info")]
    public string weaponName;
    public string weaponFamily;
    public WeaponType weaponType;
    public ReloadType reloadType;
    public int baseDamage;
    public float critMult;
    public int bulletsInMag;
    public int numOfMags;
    public int bulletsPerShot;
    [Range(0f, 1f)] public float bulletSpread;
    public float fireRate;
    public float reloadTime;
    [Range(0, 10f)] public float recoilX;
    [Range(0, 50f)] public float recoilY;
    [Range(0, 10f)] public float maxRecoilTime;

    // Runtime exclusives
    public int currentBulletCount = 0;
    public int currentRounds = 0;
    public WeaponEffects model;
    public Weapon instance;
    public bool used;

    public RuntimeWeapon(Weapon w)
    {
        weaponName = w.weaponName;
        weaponFamily = w.weaponFamily;
        weaponType = w.weaponType;
        reloadType = w.reloadType;
        baseDamage = w.baseDamage;
        critMult = w.critMult;
        bulletsInMag = w.maxBulletCount;
        numOfMags = w.maxRounds;
        bulletsPerShot = w.bulletsPerShot;
        bulletSpread = w.bulletSpread;
        fireRate = w.fireRate;
        reloadTime = w.reloadTime;
        recoilX = w.recoilX;
        recoilY = w.recoilY;
        maxRecoilTime = w.maxRecoilTime;
        model = w.fx;

        currentBulletCount = w.currentBulletCount;
        currentRounds = w.currentRounds;

        instance = w;
        used = true;
    }

    public RuntimeWeapon(WeaponObj w)
    {
        weaponName = w.weaponName;
        weaponFamily = w.weaponFamily;
        weaponType = w.weaponType;
        reloadType = w.reloadType;
        baseDamage = w.baseDamage;
        critMult = w.critMult;
        bulletsInMag = w.bulletsInMag;
        numOfMags = w.numOfMags;
        bulletsPerShot = w.bulletsPerShot;
        bulletSpread = w.bulletSpread;
        fireRate = w.fireRate;
        reloadTime = w.reloadTime;
        recoilX = w.recoilX;
        recoilY = w.recoilY;
        maxRecoilTime = w.maxRecoilTime;
        model = w.model;

        currentBulletCount = w.bulletsInMag;
        currentRounds = w.numOfMags * w.bulletsInMag;

        used = false;
    }
}
