using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBody : MonoBehaviour
{
    private Unit main;
    public Transform eyes;
    public Transform rightHand;
    public Transform leftHand;
    public BodyPart[] bodyParts;
    public float headMult = 2.5f, limbMult = .65f, bodyMult = 1f;
    public int deathAnimCount;
    public Animator animator;
    void Awake()
    {
        main = GetComponent<Unit>();
    }
    void Start()
    {
        bodyParts = GetComponentsInChildren<BodyPart>();

        foreach (BodyPart b in bodyParts)
        {
            b.main = main;
            switch (b.name)
            {
                case "Head":
                    b.damageMult = headMult;
                    break;
                case "Root":
                    b.damageMult = bodyMult;
                    break;
                case "Spine.02":
                    b.damageMult = bodyMult;
                    break;
                default:
                    b.damageMult = limbMult;
                    break;
            }
        }

        Play("WalkOffset", Random.Range(0f,1f));
    }

    public void RagDoll()
    {
        // Disable animator
        animator.enabled = false;

        // For each body of the part that has a joint, make them non kinematic and parent
        // the body to the ragdoll folder
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        // entity.rb.isKinematic = false;
        foreach (Rigidbody r in rigidbodies)
        {
            r.isKinematic = false;
            r.velocity = Vector3.zero;
            r.gameObject.layer = 14;
            r.gameObject.tag = "Ragdoll";
        }
    }

    public void RandomDeathAnim()
    {
        int rand = Random.Range(0, deathAnimCount);

        animator.CrossFade("Death " + rand, .5f);
        animator.SetLayerWeight(1, 0f);
        Play("IsDead", true);
    }

    public void BodyHit()
    {
        animator.SetLayerWeight(1, Random.Range(.75f, 1f));
        Play("OnHit");
    }

    public void ShootWeapon(WeaponType wpnType)
    {
        Play("Shoot");

        if (wpnType == WeaponType.Pump)
            animator.SetLayerWeight(2, .25f);
        else if (wpnType == WeaponType.Auto)
            animator.SetLayerWeight(2, .1f);
    }

    public void SetFireRate(WeaponType wpnType)
    {
        if (wpnType == WeaponType.Pump)
            Play("FireRate", 1f);
        else if (wpnType == WeaponType.Auto)
            Play("FireRate", 3f);
    }

    public void SetReloadSpeed(float reloadSpeed)
    {
        Play("ReloadSpeed", 1 / reloadSpeed); 
    }

    #region Animations
    /// <summary>
    /// Sets float parameter
    /// </summary>
    /// <param name="para"></param>
    /// <param name="val"></param>
    public void Play(string para, float val)
    {
        animator.SetFloat(para, val);
    }

    /// <summary>
    /// Sets integer parameter
    /// </summary>
    /// <param name="para"></param>
    /// <param name="val"></param>
    public void Play(string para, int val)
    {
        animator.SetInteger(para, val);
    }

    /// <summary>
    /// Sets boolean parameter
    /// </summary>
    /// <param name="para"></param>
    /// <param name="val"></param>
    public void Play(string para, bool val)
    {
        animator.SetBool(para, val);
    }

    /// <summary>
    /// Plays a trigger
    /// </summary>
    /// <param name="para"></param>
    public void Play(string para)
    {
        animator.SetTrigger(para);
    }

    /// <summary>
    /// Sets a trigger parameter and plays a random animation
    /// </summary>
    /// <param name="para">Parameter Trigger</param>
    /// <param name="count">Num of animations</param>
    public void PlayRandom(string para, int count)
    {
        Play("RandomInt", Random.Range(0, count));
        Play(para);
    }
    /// <summary>
    /// Sets a boolean parameter and plays a random animation
    /// </summary>
    /// <param name="para">Parameter of Boolean</param>
    /// <param name="count">Num of animations</param>
    /// <param name="active">Sets the boolean parameter</param>
    public void PlayRandom(string para, int count, bool active)
    {
        Play("RandomInt", Random.Range(0, count));
        Play(para, active);
    }
    #endregion
}




