using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : Projectile
{
    public float radius;
    public ParticleSystem explosiveParticles;
    public AudioSource audioSource;
    public Renderer _renderer;
    private CapsuleCollider cc;
    private SphereCollider sc;

    [Header("Splitting")]
    public Projectile splitProjectile;
    public bool canSplit;
    public int splitAmount;
    private float angleStep;
    private bool exploded = false;
    void Start()
    {
        cc = GetComponent<CapsuleCollider>();
        sc = GetComponent<SphereCollider>();

        angleStep = 360f / splitAmount;
    }
    public override void Launch(Vector3 dir, int dmg, string name = "None")
    {
        damage = dmg;
        collateral = 0;
        direction = dir.normalized;
        rb.velocity = direction * speed;
        owner = name;
        lifeRoutine = StartCoroutine(LifeTime());
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            transform.position -= rb.velocity * 0.05f;
        }
        Explode();
    }

    public void Explode()
    {
        if (canSplit) Split();
        exploded = true;
        explosiveParticles.Play();
        audioSource.Play();
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, 1 << 8);
        foreach (var h in hits)
        {
            if (h == null) continue;

            // Try Enemy
            Enemy e = h.GetComponentInParent<Enemy>();
            if (e != null && e != this && e.isAlive)
            {
                if (owner != "None")
                    if (e.IsKilled(damage, owner))
                        GlobalSaveSystem.AddAchievementProgress(owner + "_kills", 1);
                e.OnHit(damage);
                continue;
            }
            ISpyWorld boss = h.GetComponent<ISpyWorld>();
            if (boss != null && boss != this && boss.isAlive)
            {
                boss.OnHit(damage);
                continue;
            }
        }
        rb.velocity = Vector3.zero;
        _renderer.enabled = false;
        if (cc) cc.enabled = false;
        if (sc) sc.enabled = false;
        StopCoroutine(lifeRoutine);
        lifeRoutine = StartCoroutine(LifeTime());
    }
    public override IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifetime);


        if (!exploded) Explode();
        else Destroy(gameObject);
    }

    /// <summary>
    /// Fires out projectiles around this projectile when it hits something
    /// </summary>
    void Split()
    {
        for (int i = 0; i < splitAmount; i++)
        {
            float angleRad = angleStep * i * Mathf.Deg2Rad;
            float x = Mathf.Cos(angleRad);
            float z = Mathf.Sin(angleRad);
            Vector3 sprayDir = new Vector3(x, 0, z);
            Projectile proj = Instantiate(splitProjectile, transform.position, Quaternion.LookRotation(sprayDir), PlayerManager.Instance.bulletFolder);
            proj.Launch(sprayDir, damage, owner);
            proj.collateral = collateral;
            proj.damage = damage;
        }
    }
}
