using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public Weapon hand;
    public Transform handObj;
    public LayerMask groundLayer;
    public int currentRounds;
    private bool isReloading = false;
    private bool isFiring = false;
    private float fireCooldown = 0f;
    private ParticleSystem muzzleFlash;
    private Transform muzzlePoint;
    Coroutine reloadRoutine;
    Coroutine weaponLifeRoutine;
    Coroutine ammoRoutine;
    public List<RotatingTurret> turrets;
    public AudioSource playerAudio;
    public Weapon defaultWeapon;
    public bool isHoldingAmmo = false;
    private Player player;
    private PlayerMovement movement;
    private PlayerBody body;
    private PlayerLobbyInfo stats;
    public bool isRotating = false;
    public Vector3 rot;
    void Awake()
    {
        player = GetComponent<Player>();
        movement = GetComponent<PlayerMovement>();
        body = GetComponent<PlayerBody>();
        stats = GetComponent<PlayerLobbyInfo>();
    }
    void Start()
    {
        // Equip the default weapon
        Equip(hand);

        // Create an object pool
        currentRounds = hand.shots;
    }

    public void IsFiring(bool firing)
    {
        isFiring = firing;
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

            // Is able to shoot when not reloading or dashing
            if (!isReloading || !movement.isDashing)
            {
                if (isFiring && currentRounds > 0 && fireCooldown <= 0f)
                {
                    Shoot();
                    body.Play("IsShooting", true);
                }
                else
                {
                    body.Play("IsShooting", false);
                }
            }
        }

        else
        {
            body.Play("IsShooting", false);
        }
    }


    void FixedUpdate()
    {
        if (!player.controllerDetected)
        {
            LookAtMouse();
        }
        else if (player.controllerDetected)
        {
            if (isRotating) ControllerRotation(rot);
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

        Projectile proj = Instantiate(hand.projectilePrefab,
        muzzlePoint.position + shootDirection.normalized,
        Quaternion.LookRotation(shootDirection),
        PlayerManager.Instance.bulletFolder);

        proj.Launch(shootDirection, hand.damage, player._name);
        proj.maxCollateral = hand.collateral;

        foreach (RotatingTurret turret in turrets)
            turret.Shoot(hand.projectilePrefab, shootDirection, hand.damage, hand.collateral);

        playerAudio.PlayOneShot(hand.shootSound, Random.Range(.6f, 1.4f));

        currentRounds--;

        // Update the ammo bar if the gun doesn't has infinite ammo
        if (!hand.infiniteAmmo)
            HudManager.Instance.UpdateBar(player.playerIndex, 1, currentRounds, hand.shots);

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

            body.animator.runtimeAnimatorController = hand.holdingAnim;
        }

        if (hand.muzzleFlashPrefab != null && muzzlePoint != null)
        {
            muzzleFlash = Instantiate(hand.muzzleFlashPrefab, muzzlePoint);
            muzzleFlash.transform.position = muzzlePoint.transform.position;
            muzzleFlash.transform.rotation = muzzlePoint.transform.rotation;
        }
        HudManager.Instance.UpdateBarColor(player.playerIndex, 1, hand.ammoColor);

        if (hand.infiniteTime)
            HudManager.Instance.UpdateBarColor(player.playerIndex, 2, Color.black);
        else
        {
            HudManager.Instance.UpdateBarColor(player.playerIndex, 2, Color.white);

            if (weaponLifeRoutine != null)
                StopCoroutine(weaponLifeRoutine);
            weaponLifeRoutine = StartCoroutine(WeaponTimer(hand.lifeTime));
        }

        HudManager.Instance.UpdateBar(player.playerIndex, 1, currentRounds, hand.shots);

        body.Play("Reload");
        playerAudio.PlayOneShot(hand.reloadSound, Random.Range(.8f, 1.2f));

        if (stats.isRampage && newWeapon._name != "Sword")
        {
            player.stats.CancelRampage();
        }
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
            HudManager.Instance.UpdateBar(player.playerIndex, 2, elapsed, duration);
            yield return null;
        }

        Weapon w = WeaponLibrary.Instance.Downgrade(player, hand);
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
        body.Play("Reload");
        playerAudio.PlayOneShot(hand.reloadSound, Random.Range(.8f, 1.2f));
        yield return new WaitForSeconds(hand.reloadTime);
        isReloading = false;
        currentRounds = hand.shots;
        HudManager.Instance.UpdateBar(player.playerIndex, 1, currentRounds, hand.shots);
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

    public void JoystickRotation(Vector2 input)
    {
        isRotating = input.magnitude > .00001f;
        rot = input;
    }

    /// <summary>
    /// Player rotates based off of controller usage
    /// </summary>
    public void ControllerRotation(Vector2 input)
    {
        // read input you stored earlier
        Vector3 targetDirection = new Vector3(input.x, 0f, input.y);
        Vector3 f = Vector3.forward;
        Vector3 r = Vector3.right;
        Transform cameraTransform = Camera.main.transform;
        if (cameraTransform)
        {
            f = cameraTransform.forward; f.y = 0; f.Normalize();
            r = cameraTransform.right; r.y = 0; r.Normalize();
        }
        targetDirection = (r * targetDirection.x) + (f * targetDirection.z);
        // The step size is equal to speed times frame time.
        float singleStep = 180f * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);
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

            Weapon w = WeaponLibrary.Instance.Upgrade(player, hand);

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
