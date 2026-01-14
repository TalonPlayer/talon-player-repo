using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Info")]
    public string weaponName;
    public string weaponFamily; // If upgradeable weapons are implemented
    public WeaponType weaponType;
    public bool isDual;

    [Header("Weapon Stats")]
    [HideInInspector] public int currentBulletCount = 0;
    public int maxBulletCount;
    [HideInInspector] public int currentRounds = 0;
    public int maxRounds;
    public int baseDamage;
    [Range(1, 15)]
    public int bulletsPerShot = 1;
    public float fireRate, reloadTime, damageMultiplier, bulletSpread;
    [Range(0, 10f)]
    public float recoilX;
    [Range(0, 40f)]
    public float recoilY;
    [Range(0, 2f)]
    public float maxRecoilTime = 1f;
    public float adsSpread;
    private float _lastShootTime;
    [Header("Extras - Leave empty if can't be applied")]
    public float explosionRadius;
    public bool HasAmmo { get { return currentBulletCount > 0; } }
    public int CurrentMags { get { return currentRounds / maxBulletCount; } }
    public bool HasMags { get { return CurrentMags > 0; } }
    public bool OutOfAmmo { get { return !HasAmmo && !HasMags; } }
    public bool IsReady { get { return _lastShootTime + fireRate < Time.time; } }
    public WeaponEffects fx;
    public Collider coll;
    public Rigidbody rb;
    void Start()
    {
        currentBulletCount = maxBulletCount;
        currentRounds = maxRounds;
    }
    public void Init()
    {
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

                        damage += Mathf.RoundToInt(damageMultiplier * baseDamage);
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

}
public enum WeaponType
{
    Semi, // Press once to shoot
    Sniper, // Press once to shoot, then must wait to cock
    Shotgun, // Press once to shoot & spread, then must wait to cock
    Auto, // Hold down shoot
    Melee,
}