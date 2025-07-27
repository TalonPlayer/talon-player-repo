using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Weapon : ScriptableObject
{
    [Header("Gun Info")]
    public string _name;
    public string id;
    [Header("Gun Stats")]
    public bool infiniteAmmo;
    public int shots;
    public int damage;
    public int collateral;
    public float reloadTime;
    public float fireRate;
    public bool infiniteTime;
    public float lifeTime;
    [Header("Gun Model")]
    public GameObject model;
    public Projectile projectilePrefab;
    public Color ammoColor;
    public ParticleSystem muzzleFlashPrefab;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AnimatorController holdingAnim;
}
