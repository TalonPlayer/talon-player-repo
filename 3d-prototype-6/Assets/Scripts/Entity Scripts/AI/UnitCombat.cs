using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    private Unit main;
    public WeaponObj startingWeapon;
    public Weapon inHand;
    [Header("Non-Weapon Stats")]
    public float meleeRange; // Range to validate melee
    public float meleeDist; // Range to count as damage
    public float pauseInterval = 1f;
    public int intervalShotCount;
    public delegate void OnFire();
    public event OnFire onFire;
    public bool isReloading = false;
    private bool _isPaused = false;
    void Awake()
    {
        main = GetComponent<Unit>();
    }

    void Start()
    {
        // if (startingWeapon) Init();
    }

    public void Init()
    {
        if (startingWeapon == null) return;
        RuntimeWeapon wpn = new RuntimeWeapon(startingWeapon);
        Equip(wpn);
        intervalShotCount = Random.Range(4, 6 + main.ai.AILevel);
    }

    public void FSM()
    {
        if (!inHand || main.movement.inCover) return;
        if (main.movement.isFacingTarget && main.vision.isLosClear)
        {
            if (intervalShotCount > 0)
                Fire();
            else
            {
                if (!_isPaused) StartCoroutine(IntervalShooting(Random.Range(1f, pauseInterval)));
            }

        }
    }

    public IEnumerator IntervalShooting(float interval)
    {
        _isPaused = true;
        yield return new WaitForSeconds(interval);
        intervalShotCount = inHand.maxBulletCount - main.ai.AILevelChoice(inHand.maxBulletCount);
        _isPaused = false;

    }

    public void ShootWeapon()
    {
        if (isReloading || !inHand.IsReady || inHand.OutOfAmmo) return;
        
        Vector3 pos = transform.position; pos.y = main.body.eyes.position.y;
        inHand.Shoot(main.ai.targetTag, main.ai.layers, pos, transform.forward, main);
        main.body.ShootWeapon(inHand.weaponType);
        intervalShotCount--;

        if (!inHand.HasAmmo && inHand.currentRounds > 0) { Reload(); return; }
    }

    public void Melee()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, meleeDist, main.ai.layers, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.CompareTag(main.ai.targetTag))
            {
                Debug.Log(hit.collider.name + " was hit by a melee!");
            }
        }
    }

    public void Fire()
    {
        onFire?.Invoke();
    }

    public void Reload()
    {
        if (!isReloading)
        {
            main.body.Play("Reload");
            isReloading = true;
        }
    }
    public void FinishReload()
    {
        main.body.animator.SetLayerWeight(2, .2f);
        isReloading = false;
        inHand.Reload();
    }
    public void Equip(RuntimeWeapon w)
    {
        WeaponEffects fx = Instantiate(w.model);
        Weapon weapon = fx.gameObject.AddComponent<Weapon>();

        if (!fx) {Debug.LogWarning($"Weapon Effects not valid for {w.weaponName}"); return;}
        inHand = weapon;
        if (!w.used) inHand.Init(w, fx);

        // Add logic if the player is carrying only one weapon, the current weapon gets put in off hand
        // Add logic for dropping weapons
        Vector3 size = inHand.transform.localScale;
        inHand.transform.parent = main.body.rightHand.transform;
        inHand.transform.localRotation = Quaternion.Euler(Vector3.zero);
        inHand.transform.localPosition = Vector3.zero;
        inHand.transform.localScale = size;
        
        fx.rb.isKinematic = true;
        fx.rb.useGravity = false;
        inHand.gameObject.layer = 2;
        Transform[] children = inHand.transform.GetComponentsInChildren<Transform>();
        foreach (Transform c in children) c.gameObject.layer = 2;
        fx.coll.enabled = false;

        onFire -= ShootWeapon;
        onFire += ShootWeapon;
        main.body.SetWeapon(inHand.fx.unitWeaponAnimator);
        main.body.SetFireRate(inHand.weaponType);
        main.body.SetReloadSpeed(inHand.reloadTime);
    }
    public void DropHand()
    {
        Weapon.DropHand(main.combat.inHand);
    }
}
