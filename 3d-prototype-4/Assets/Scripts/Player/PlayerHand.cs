using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public Weapon hand;
    public Transform handObj;
    public LayerMask groundLayer;
    public List<Projectile> projPool = new List<Projectile>();
    public int currentRounds;
    public int poolSize;
    private bool isReloading = false;
    private float fireCooldown = 0f;
    private ParticleSystem muzzleFlash;
    private Transform muzzlePoint;
    Coroutine reloadRoutine;
    Coroutine weaponLifeRoutine;
    Coroutine ammoRoutine;
    private Player player;
    public AudioSource playerAudio;
    public Weapon defaultWeapon;
    public bool isHoldingAmmo = false;
    void Awake()
    {
        player = GetComponent<Player>();
    }
    void Start()
    {
        Equip(hand);
        CreateProjectilePool();
        currentRounds = hand.shots;
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (hand != null)
        {
            if (Input.GetKeyDown(KeyCode.B) && hand.name != "1911")
            {
                Equip(defaultWeapon);
            }
            if (!isReloading)
            {
                if (Input.GetMouseButton(0) && currentRounds > 0 && fireCooldown <= 0f)
                {
                    Shoot();
                    player.body.Play("IsShooting", true);
                }
                else
                {
                    player.body.Play("IsShooting", false);
                }
            }
        }

        else
        {
            player.body.Play("IsShooting", false);
        }
    }


    void FixedUpdate()
    {
        LookAtMouse();
    }

    void CreateProjectilePool()
    {
        poolSize = hand.shots;

        for (int i = 0; i < poolSize; i++)
        {
            Projectile p = Instantiate(hand.projectilePrefab, PlayerManager.Instance.bulletFolder);
            p.gameObject.SetActive(false);
            p.SetOwner(this);
            projPool.Add(p);
        }
    }

    void Shoot()
    {
        fireCooldown = hand.fireRate;

        Vector3 shootDirection = GetAimDirection();
        Projectile proj = projPool[currentRounds - 1];

        proj.transform.position = muzzlePoint.position + shootDirection.normalized; // offset forward
        proj.transform.rotation = Quaternion.LookRotation(shootDirection);
        proj.gameObject.SetActive(true);
        proj.Launch(shootDirection);

        playerAudio.PlayOneShot(hand.shootSound, Random.Range(.6f, 1.4f));

        currentRounds--;

        if (!hand.infiniteAmmo)
            HudManager.Instance.UpdateBar(1, currentRounds, poolSize);

        if (currentRounds <= 0)
        {
            if (!hand.infiniteAmmo)
                reloadRoutine = StartCoroutine(StartReloading());
            else
                currentRounds = hand.shots;
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
    }


    Vector3 GetAimDirection()
    {
        Vector3 mouseWorldPos = MouseWorld.GetMouseWorldPosition(1 << 6);
        Vector3 dir = (mouseWorldPos - muzzlePoint.position).normalized;


        Vector3 enemyPos = MouseWorld.GetMouseWorldPosition(1 << 8);
        if (enemyPos != Vector3.zero)
            dir = (enemyPos - muzzlePoint.position).normalized;
        else
            dir.y = 0f;
            
        return dir;
    }

    public void Equip(Weapon newWeapon)
    {
        if (newWeapon == null) return;
        hand = newWeapon;
        currentRounds = hand.shots;
        muzzlePoint = null;
        foreach (Projectile p in projPool)
            Destroy(p.gameObject);

        projPool.Clear();

        CreateProjectilePool();

        if (hand.model != null)
        {
            foreach (Transform child in handObj)
            {
                Destroy(child.gameObject); // Remove old model
            }

            GameObject newModel = Instantiate(hand.model, handObj);
            newModel.transform.localPosition = Vector3.zero;
            newModel.transform.localRotation = Quaternion.identity;

            muzzlePoint = newModel.transform.GetChild(0);

            player.body.animator.runtimeAnimatorController = hand.holdingAnim;
        }

        if (hand.muzzleFlashPrefab != null && muzzlePoint != null)
        {
            muzzleFlash = Instantiate(hand.muzzleFlashPrefab, muzzlePoint);
            muzzleFlash.transform.position = muzzlePoint.transform.position;
            muzzleFlash.transform.rotation = muzzlePoint.transform.rotation;
        }
        HudManager.Instance.UpdateBarColor(1, hand.ammoColor);

        if (hand.infiniteTime)
            HudManager.Instance.UpdateBarColor(2, Color.black);
        else
        {
            HudManager.Instance.UpdateBarColor(2, Color.white);

            if (weaponLifeRoutine != null)
                StopCoroutine(weaponLifeRoutine);
            weaponLifeRoutine = StartCoroutine(WeaponTimer(hand.lifeTime));
        }

        HudManager.Instance.UpdateBar(1, currentRounds, poolSize);

        player.body.Play("Reload");
        playerAudio.PlayOneShot(hand.reloadSound, Random.Range(.8f, 1.2f));
    }

    public IEnumerator WeaponTimer(float duration)
    {
        float elapsed = duration;

        while (elapsed > 0f)
        {
            if (!isHoldingAmmo) elapsed -= Time.deltaTime;
            HudManager.Instance.UpdateBar(2, elapsed, duration);
            yield return null;
        }

        Weapon w = WeaponLibrary.Instance.Downgrade(hand);
        Equip(w);
    }


    public void ReturnToPool(Projectile p)
    {
        p.gameObject.SetActive(false);
    }

    IEnumerator StartReloading()
    {
        isReloading = true;
        player.body.Play("Reload");
        playerAudio.PlayOneShot(hand.reloadSound, Random.Range(.8f, 1.2f));
        yield return new WaitForSeconds(hand.reloadTime);
        isReloading = false;
        currentRounds = hand.shots;
        HudManager.Instance.UpdateBar(1, currentRounds, poolSize);
    }

    public void LookAtMouse()
    {
        Vector3 mouseWorldPos = MouseWorld.GetMouseWorldPosition(1 << 6);
        Vector3 lookDirection = mouseWorldPos - transform.position;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude > 0.01f)
        {
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 20f * Time.deltaTime);
        }
    }

    public void PickUpAmmo()
    {
        if (isHoldingAmmo)
        {
            if (ammoRoutine != null)
            {
                StopCoroutine(ammoRoutine);
            }

            Weapon w = WeaponLibrary.Instance.Upgrade(hand);

            Equip(w);
        }

        ammoRoutine = StartCoroutine(AmmoRoutine());
    }

    public IEnumerator AmmoRoutine()
    {
        isHoldingAmmo = true;
        yield return new WaitForSeconds(25f);

        isHoldingAmmo = false;
    }
}
