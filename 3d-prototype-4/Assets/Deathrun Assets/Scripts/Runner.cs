using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Runner : MonoBehaviour
{
    [Header("Components")]
    public Transform target;
    public Rigidbody rb;
    public ParticleSystem bloodParticles;
    public ParticleSystem deathParticles;
    public LayerMask deathLayer;
    [Header("Stats")]
    public int health;
    public int damage;
    public bool isAlive;
    public bool fellOver;
    public float standUpTime = 2f;
    [HideInInspector] public RunnerMovement movement;
    [HideInInspector] public RunnerCombat combat;
    [HideInInspector] public RunnerBody body;
    [HideInInspector] public CapsuleCollider cc;
    public UnityEvent onDeath;
    public UnityEvent onTarget;
    private Coroutine ragdollRoutine;
    void Awake()
    {
        movement = GetComponent<RunnerMovement>();
        combat = GetComponent<RunnerCombat>();
        body = GetComponent<RunnerBody>();
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
    }
    void Start()
    {
        RunnerManager.healthTick += CheckHealth;
    }

    void Update()
    {

    }

    public void CheckHealth()
    {
        if (health <= 0)
        {
            isAlive = false;
            OnDeath();
        }
    }

    public void OnDeath()
    {
        RunnerManager.Instance.RunnerDeath(this);
        body.PlayRandom("IsDead", body.deathAnims);
        body.Play("RandomFloat", Random.Range(.75f, 1.5f));
        body.CloseEyes();
        cc.excludeLayers += deathLayer;
        // Ragdoll
        // Destroyself 1f
        onDeath?.Invoke();
    }

    public void NoAnimDeath()
    {
        RunnerManager.Instance.RunnerDeath(this);
        cc.excludeLayers += deathLayer;
        // Ragdoll
        // Destroyself 1f
        movement.OnDeath();
        body.animator.enabled = false;
        body.RagDoll();
        body.DelayDropAll(0.1f);
        body.CloseEyes();
        body.DelayDropAll(0.2f);
        DestroySelf(.35f);
    }

    /// <summary>
    /// Delay the destroy
    /// </summary>
    /// <param name="time"></param>
    public void DestroySelf(float time)
    {
        Invoke(nameof(DestroySelf), time);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// When the runner gets hit
    /// </summary>
    /// <param name="damage"></param>
    public void OnHit(int damage)
    {
        if (bloodParticles != null)
        {
            bloodParticles.Play();
        }

        RunnerManager.damageTick += () =>
        {
            health -= damage;
        };
    }

    public void Target(Transform _target)
    {
        target = _target;
    }

    public void KnockOver(float time)
    {
        if (fellOver)
        {
            if (ragdollRoutine != null) StopCoroutine(ragdollRoutine);
            ragdollRoutine = StartCoroutine(StandUp(time));
            return;
        }
        ragdollRoutine = StartCoroutine(StandUp(time));
        fellOver = true;
        movement.OnDeath();
        movement.agent.enabled = false;
        body.animator.enabled = false;
        body.RagDoll();

    }
    public IEnumerator StandUp(float time)
    {
        yield return new WaitForSeconds(time);
        fellOver = false;
        body.UnRagdoll();
        movement.agent.enabled = true;
        movement.UnDeath();
    }
}
