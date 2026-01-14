using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModel : MonoBehaviour
{
    public string weaponName;
    public string weaponID;
    public int maxAmmoCount;
    public int maxMagCount = 3;
    public float fireRate;
    public Collider magObj;
    public Transform magPos;
    public RuntimeAnimatorController weaponAnimator;
    public RuntimeAnimatorController zombieAnimator;
    public List<Projectile> darts;
    public WeaponType weaponType;
    public LoadType loadingType;
    public bool isExclusive = false;
}


public enum WeaponType
{
    Pistol, Rifle, Sniper, Minigun
}

public enum LoadType
{
    Single, Pump, Semi, Auto
}