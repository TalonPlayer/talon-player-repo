using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float health;
    public float max;
    public bool isAlive = true;
    public bool isPassive = false;
    public bool hideBar = false;
    public float maxSlopeAngle = 35f;
    public float distance;

    public bool isKnocked = false;
    public bool isAggro = false;
    public bool knockbackResistant = false;
    public bool canBlock = false;
    public LayerMask ignoreLayers;

    [Header("Components")]
    public Transform head;
    public Rigidbody rb;
    public CapsuleCollider cc;
    public Interactable interactable;
    public Healthbar healthbar;
    public ParticleSystem damageFX;
    private ParticleSystem instanceDamageFX;
    [Header("Scripts")]
    public EnemyMovement movement;
    public EnemyCombat combat;
    public EnemyBody body;
    [Header("Events")]
    public UnityEvent onAggro;
    public UnityEvent onDeath;
    [HideInInspector] public Transform target;
    [HideInInspector] public int aggroGroup;
    [HideInInspector] public Vector3 kbDirection;
    [HideInInspector] public bool pause;
    [HideInInspector] public float kbForce;
    void Awake()
    {
        movement.SetEnemy(this);
        combat.SetEnemy(this);
        body.SetEnemy(this);
        healthbar.gameObject.SetActive(false);
        max = health;

        if (interactable)
        {
            interactable.onInteraction.RemoveAllListeners(); // clear old ones just in case
            interactable.onInteraction.AddListener(() =>
                FindObjectOfType<PlayerMovement>().DashTotarget(gameObject)
            );
        }

    }

    void Update()
    {
        if (!isAggro || isKnocked || !isAlive) return;
        distance = (target.position - transform.position).magnitude;

        combat.CheckDistance();
    }

    bool IsFloor(Vector3 normal)
    {
        float angle = Vector3.Angle(Vector3.up, normal);
        return angle < maxSlopeAngle;
    }

    private void OnCollisionStay(Collision other)
    {
        if (pause) return;

        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            if (IsFloor(normal))
            {
                if (isKnocked)
                    movement.ResetAfterKnockback();
                break;
            }
        }
    }


    public void OnHit(float damage, Vector3 incomingDir, float knockBackForce)
    {
        if (isAlive)
        {
            kbDirection = incomingDir;

            if (canBlock && movement.isFacing)
            {
                TakeDamage(damage / 20);
                KnockBack(kbDirection.normalized, knockBackForce / 6);
                Play("Block");
            }
            else
            {
                TakeDamage(damage);
                if (!knockbackResistant)
                {
                    KnockBack(kbDirection.normalized, knockBackForce);
                }
            }
        }
    }

    void TakeDamage(float damage)
    {
        PlayDamageFX();
        health -= damage;
        healthbar.UpdateHealth(health, max);
        CheckHealth();
    }

    void PlayDamageFX()
    {
        instanceDamageFX = Instantiate(
            damageFX,
            transform.position,
            head.rotation,
            head.transform);
        instanceDamageFX.Play(true);
    }
    void CheckHealth()
    {
        if (health <= 0.0f)
        {
            onDeath.Invoke();
        }
    }
    public void OnDeath()
    {

        isAlive = false;
        isAggro = false;

        // Disable Head Rigs
        EnemyManager.Instance.PlayDeathSound();
        HUDController.Instance.CloseIntButton();
        WorldController.Instance.CheckEnemyCount();
        healthbar.gameObject.SetActive(false);

        // Interactable disable outline and disable


        cc.enabled = false;
        // Movement and combat on death

        // animator = false

        // Ragdoll(true)


        // Drop Items

        gameObject.layer = 2;
        gameObject.tag = "Untagged";
    }



    void ResetVel()
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody r in rigidbodies)
        {
            r.velocity = r.velocity * 0.05f;
        }
    }


    public void TargetPlayer()
    {
        target = GameManager.Instance.player.transform;
        isAggro = true;
    }

    public void KnockBack(Vector3 direction, float knockBackForce)
    {
        if (isAlive && !combat.noInterruption)
        {

            if (!canBlock || !movement.isFacing)
            {
                body.BoolAnim("IsKnocked", true);
                body.animator.Play("Hurt");
            }
        }
        kbForce = knockBackForce;
        movement.navMeshAgent.enabled = false;
        isKnocked = true;
        pause = true;
        Invoke(nameof(UnPause), .5f);
        rb.isKinematic = false;
        rb.AddForce(Vector3.up * knockBackForce * .25f);
        rb.AddForce(direction * knockBackForce);
    }

    public void UnPause()
    {
        pause = false;
    }

    public void Play(string anim)
    {
        body.animator.Play(anim);
    }

}
