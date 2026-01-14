using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    private Unit main;
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
        if (inHand)
        {
            Equip(inHand);
        }
        else
        {
            Transform t = main.body.rightHand.GetChild(0);

            if (t)
            {
                Weapon wpn = t.GetComponent<Weapon>();
                if (wpn)
                {
                    Equip(wpn);
                }
            }
        }

        intervalShotCount = Random.Range(6, 12 + main.ai.AILevel);
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

        if (!inHand.HasAmmo && inHand.HasMags) { Reload(); return; }
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
            main.body.animator.SetLayerWeight(2, 1f);
            isReloading = true;
        }
    }
    public void FinishReload()
    {
        main.body.animator.SetLayerWeight(2, .2f);
        isReloading = false;
        inHand.Reload();
    }
    public void Equip(Weapon weapon)
    {
        // Add logic if the player is carrying only one weapon, the current weapon gets put in off hand
        // Add logic for dropping weapons
        weapon.transform.parent = main.body.rightHand.transform;
        weapon.rb.isKinematic = true;
        weapon.rb.useGravity = false;
        weapon.coll.gameObject.layer = 2;
        weapon.coll.enabled = false;
        weapon.transform.localPosition = Vector3.zero;

        inHand = weapon;
        onFire -= ShootWeapon;
        onFire += ShootWeapon;
        inHand.Init();
        main.body.SetFireRate(inHand.weaponType);
    }
}
