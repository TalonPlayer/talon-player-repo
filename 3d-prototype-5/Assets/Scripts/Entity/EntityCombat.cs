using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCombat : MonoBehaviour
{
    public float attackRange;
    public float attackDelay;
    public float attackTimeMin;
    public float attackTimeMax;
    public bool isAttacking;
    public bool isThrowing;
    public bool canAttack = true;

    [Header("Human Weapons")]
    public WeaponModel weapon;
    public List<Projectile> throwablePrefabs;
    private Transform magPos;
    private Collider magPrefab;
    public int maxAmmoCount = 30;
    public int currentAmmoCount = 0;
    public int throwableCount;
    public int maxThrowableCount;
    public int currentMags;
    public int maxMagCount = 3;
    public bool isReloading;
    public bool hasAmmo;
    public float reloadTime = 2f;
    Coroutine reloadRoutine;

    private Vector3 direction;
    Coroutine attackRoutine;
    Coroutine throwRoutine;
    private Entity entity;
    public EntityBody body;
    private EntityBrain brain;
    private Vector3 pos;
    void Awake()
    {

    }

    void Start()
    {
        entity = GetComponent<Entity>();
        brain = GetComponent<EntityBrain>();
    }

    public void Equip(WeaponModel newWeapon)
    {
        weapon = newWeapon;

        maxAmmoCount = weapon.maxAmmoCount;

        attackTimeMax = weapon.fireRate + .25f;
        attackTimeMin = weapon.fireRate - .25f;

        currentAmmoCount = maxAmmoCount;
        if (body.hand.childCount != 0)
        {
            foreach (Transform child in body.hand)
            {
                Destroy(child.gameObject);
            }
        }


        WeaponModel obj = Instantiate(weapon, body.hand);
        magPrefab = obj.magObj;
        magPos = obj.magPos;
        body.animator.runtimeAnimatorController = obj.weaponAnimator;

        body.heldItems.Add(obj.gameObject);

    }

    public void SetAnimatorController(RuntimeAnimatorController animatorController)
    {
        body.animator.runtimeAnimatorController = animatorController;
    }

    public void MeleeAttack()
    {
        body.Play("RandomAttack", Random.Range(0, 6));
        body.Play("IsShooting", true);
    }

    public void StopAttack()
    {
        body.Play("IsShooting", false);
    }

    public void RangeAttack()
    {
        Vector3 targetPos = brain.facingTarget.position;
        targetPos.y += 1f;
        direction = targetPos - transform.position;
        if (!canAttack || direction.magnitude > attackRange) return;
        if (attackRoutine != null) StopCoroutine(attackRoutine);
        attackRoutine = StartCoroutine(RangeRoutine());
    }

    IEnumerator RangeRoutine()
    {
        canAttack = false;
        Shoot();
        entity.body.Play("IsShooting", true);
        yield return new WaitForSeconds(Random.Range(attackTimeMin, attackTimeMax));
        entity.body.Play("IsShooting", false);
        canAttack = true;
    }

    public void Shoot()
    {
        Vector3 pos = transform.position;
        pos.y += 1f;

        // small random angular spread (in degrees)
        float spread = 50f;
        Quaternion yaw = Quaternion.AngleAxis(Random.Range(-spread, spread), transform.up);
        Quaternion pitch = Quaternion.AngleAxis(Random.Range(-spread, spread), transform.right);
        Vector3 shotDir = yaw * pitch * direction;
        shotDir.Normalize();

        Projectile proj = Instantiate
        (Helper.RandomElement(weapon.darts),
        pos,
        Quaternion.LookRotation(shotDir),
        EntityManager.Instance.projectileFolder);

        proj.Launch(direction, entity.entityName);

        currentAmmoCount--;
    }

    public void ThrowAttack()
    {
        Vector3 targetPos = brain.facingTarget.position;
        targetPos.y += 1f;
        direction = targetPos - transform.position;
        if (direction.magnitude > 5f) return;
        if (throwRoutine != null) StopCoroutine(throwRoutine);
        throwRoutine = StartCoroutine(ThrowRoutine());
    }

    IEnumerator ThrowRoutine()
    {
        isThrowing = true;
        body.Play("Throw");
        yield return new WaitForSeconds(.85f);
        Throw();
        yield return new WaitForSeconds(1f);
        isThrowing = false;
    }
    
    public void Throw()
    {
        Vector3 pos = transform.position;
        pos.y += 1f;

        // small random angular spread (in degrees)
        float spread = 50f;
        Quaternion yaw = Quaternion.AngleAxis(Random.Range(-spread, spread), transform.up);
        Quaternion pitch = Quaternion.AngleAxis(Random.Range(-spread, spread), transform.right);
        Vector3 shotDir = yaw * pitch * direction;
        shotDir.Normalize();

        Projectile proj = Instantiate
        (Helper.RandomElement(throwablePrefabs),
        pos,
        Quaternion.LookRotation(shotDir),
        EntityManager.Instance.projectileFolder);

        proj.Launch(direction, entity.entityName);

        throwableCount--;
    }

    public void Reload()
    {
        
        if (reloadRoutine != null) StopCoroutine(reloadRoutine);
        reloadRoutine = StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        currentMags--;
        hasAmmo = currentMags <= 0;
        isReloading = true;
        entity.body.Play("Reload");

        if (magPrefab != null)
        {
            Collider mag = Instantiate(magPrefab,
            magPos.position,
            Quaternion.identity,
            EntityManager.Instance.itemFolder);
            mag.gameObject.SetActive(true);
            mag.enabled = true;
            mag.gameObject.transform.localScale = magPrefab.gameObject.transform.lossyScale;
            Rigidbody rb = mag.attachedRigidbody;

            rb.useGravity = true;
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.None;
        }


        yield return new WaitForSeconds(reloadTime);
        currentAmmoCount = maxAmmoCount;
        isReloading = false;
    }

}
