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
    public Transform cameraTransform;
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

        if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;
    }
    void Start()
    {
        // Equip the default weapon
        Equip(hand);

        // Create an object pool
        CreateProjectilePool();
        currentRounds = hand.shots;
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (hand != null)
        {
            // Discard the current weapon if it's not the default weapon
            if (Input.GetKeyDown(KeyCode.B) && hand.name != defaultWeapon.name)
            {
                Equip(defaultWeapon);
            }
            
            // Is able to shoot when not reloading
            if (!isReloading)
            {
                if ((Input.GetMouseButton(0) || 0.5f <= Input.GetAxis("Shoot")) && currentRounds > 0 && fireCooldown <= 0f)
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
        if (player.controllerDetected)
            ControllerRotation();
        else
            LookAtMouse();
    }

    /// <summary>
    /// Creates the object pool for projectiles
    /// </summary>
    void CreateProjectilePool()
    {
        poolSize = hand.shots * 2; // Increase pool size so that the player always has enough bullets

        for (int i = 0; i < poolSize; i++)
        {
            Projectile p = Instantiate(hand.projectilePrefab, PlayerManager.Instance.bulletFolder);
            p.gameObject.SetActive(false);
            p.SetOwner(this);
            projPool.Add(p);
        }
    }
    
    /// <summary>
    /// Shoot a projectile
    /// </summary>
    void Shoot()
    {
        fireCooldown = hand.fireRate;

        // Send the projectile in a direction
        Vector3 shootDirection;
        shootDirection = GetAimDirection();

        // Get a projectile thats available
        Projectile proj = projPool[currentRounds - 1];

        // Projectile comes out of the muzzle point of the gun
        // The projectile also has the same rotation in the direction
        proj.transform.position = muzzlePoint.position + shootDirection.normalized; // offset forward
        proj.transform.rotation = Quaternion.LookRotation(shootDirection);
        proj.gameObject.SetActive(true);
        proj.Launch(shootDirection);

        playerAudio.PlayOneShot(hand.shootSound, Random.Range(.6f, 1.4f));

        currentRounds--;

        // Update the ammo bar if the gun doesn't has infinite ammo
        if (!hand.infiniteAmmo)
            HudManager.Instance.UpdateBar(1, currentRounds, hand.shots);

        // If player runs out of bullets, reload unless infinite ammo
        if (currentRounds <= 0)
        {
            if (!hand.infiniteAmmo)
                reloadRoutine = StartCoroutine(StartReloading());
            else
                currentRounds = hand.shots;
        }

        // Play muzzleflash
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
    }

    /// <summary>
    /// Get the direction of where the player is aiming
    /// </summary>
    /// <returns></returns>
    Vector3 GetAimDirection()
    {
        Vector3 enemyPos;
        Vector3 dir = Vector3.zero;
        if (player.controllerDetected)
        {
            // Attempt to find an enemy in front of the player
            enemyPos = MouseWorld.GetControllerWorldPosition(1 << 8, transform);
            dir = transform.forward;
        }
        else
        {
            // Attempt to find an enemy that the cursor is hovering over
            Vector3 mouseWorldPos = MouseWorld.GetMouseWorldPosition(1 << 6);
            dir = (mouseWorldPos - muzzlePoint.position).normalized;
            enemyPos = MouseWorld.GetControllerWorldPosition(1 << 8, transform);
        }

        if (enemyPos != Vector3.zero)
            dir = (enemyPos - muzzlePoint.position).normalized;
        else
            dir.y = 0f;
            
        return dir;
    }

    /// <summary>
    /// Sets up the weapon for the player to use
    /// </summary>
    /// <param name="newWeapon"></param>
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

        HudManager.Instance.UpdateBar(1, currentRounds, hand.shots);

        player.body.Play("Reload");
        playerAudio.PlayOneShot(hand.reloadSound, Random.Range(.8f, 1.2f));
    }

    /// <summary>
    /// Weapon has limited duration
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Returns projectile back to player
    /// </summary>
    /// <param name="p"></param>
    public void ReturnToPool(Projectile p)
    {
        p.gameObject.SetActive(false);
    }

    /// <summary>
    /// Player reloads
    /// </summary>
    /// <returns></returns>
    IEnumerator StartReloading()
    {
        isReloading = true;
        player.body.Play("Reload");
        playerAudio.PlayOneShot(hand.reloadSound, Random.Range(.8f, 1.2f));
        yield return new WaitForSeconds(hand.reloadTime);
        isReloading = false;
        currentRounds = hand.shots;
        HudManager.Instance.UpdateBar(1, currentRounds, hand.shots);
    }

    /// <summary>
    /// Player rotates towards mouse if there is no controller detected
    /// </summary>
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

    /// <summary>
    /// Player rotates based off of controller usage
    /// </summary>
    public void ControllerRotation()
    {
        float x = Input.GetAxis("RightStickX");
        float y = Input.GetAxis("RightStickY");

        // deadzone
        Vector2 v = new Vector2(x, y);
        if (v.sqrMagnitude < 0.15f * 0.15f) return;

        v = new Vector2(
            v.x * Mathf.Sqrt(1f - 0.5f * v.y * v.y),
            v.y * Mathf.Sqrt(1f - 0.5f * v.x * v.x)
        );

        // build world-space look direction (camera-relative)
        Vector3 f = Vector3.forward;
        Vector3 r = Vector3.right;
        if (cameraTransform)
        {
            f = cameraTransform.forward; f.y = 0; f.Normalize();
            r = cameraTransform.right; r.y = 0; r.Normalize();
        }

        Vector3 lookDir = (r * v.x + f * v.y);
        if (lookDir.sqrMagnitude < 1e-6f) return;

        Quaternion target = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, 20f * Time.deltaTime);
    }

    /// <summary>
    /// Allows the current weapon to be heald on for longer
    /// </summary>
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
