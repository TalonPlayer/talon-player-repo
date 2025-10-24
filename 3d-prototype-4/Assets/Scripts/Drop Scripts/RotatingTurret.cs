using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingTurret : RotatingObject
{
    public Transform muzzlePoint;
    public ParticleSystem muzzleFlash;
    public List<Renderer> bodyColor;
    private Projectile projPrefab;
    public bool isDead = false;
    private int damage;
    private int maxCollateral;
    protected override void Start()
    {
        base.Start();
        ChangeBodyColor(PlayerManager.Instance.GetColor(player.colorCode));
        rotateSpeed *= Random.Range(1f, 2.5f);
        radius *= Random.Range(.75f, 2f);
    }
    protected override void Update()
    {
        if (!isDead)
        {
            base.Update();
            transform.rotation = player.transform.rotation;
        }
        else
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }

    }

    public void ChangeBodyColor(Color color)
    {
        foreach (Renderer r in bodyColor)
            r.material.color = color;
    }

    protected override IEnumerator LifeTimeRoutine()
    {
        yield return new WaitForSeconds(lifeTime);

        isDead = true;
        rotateSpeed = 360f;
        player.hand.turrets.Remove(this);
        for (int i = 0; i < 30; i++)
        {
            Shoot(projPrefab, transform.forward, damage, maxCollateral);
            yield return new WaitForSeconds(.25f);
        }
        transform.parent = EntityManager.Instance.itemFolder;

        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.isKinematic = false;

        if (bc)
        {
            bc.enabled = true;
            bc.isTrigger = false;
        }
        if (cc)
        {
            cc.enabled = true;
            cc.isTrigger = false;
        }
        if (sc)
        {
            sc.enabled = true;
            sc.isTrigger = false;
        }
        Vector3 direction = RandExt.RandomDirection(30f, 80f);
        Vector3 torque = RandExt.ApplyTorque(direction, 5f);
        rb.AddForce(direction * 3f, ForceMode.Impulse);
        rb.AddTorque(torque, ForceMode.Impulse);

        enabled = false;
    }

    public void Shoot(Projectile projectile, Vector3 shootDirection, int damage, int collateral)
    {
        projPrefab = projectile;
        Projectile proj = Instantiate(projectile,
        muzzlePoint.position + shootDirection.normalized,
        Quaternion.LookRotation(shootDirection),
        PlayerManager.Instance.bulletFolder);

        proj.Launch(shootDirection, damage, "Turret Buddy");
        proj.maxCollateral = collateral;
        maxCollateral = collateral;
        this.damage = damage;
        muzzleFlash.Play();
    }
}
