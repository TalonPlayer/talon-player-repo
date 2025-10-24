using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    // This class is basically just the Hazards class, but im not going to rename it
    public float rechargeTime;
    public float duration;
    public bool automatic = false;
    private bool isPaused = false;
    public CapsuleCollider damageCollider;
    public Animator animator;
    Coroutine rechargeRoutine;
    Coroutine activeRoutine;
    void Start()
    {
        if (automatic) Init();
    }

    /// <summary>
    /// Start the loop for hazard
    /// </summary>
    public void Init()
    {
        animator.SetBool("Reset", false);
        isPaused = false;
        rechargeRoutine = StartCoroutine(RechargeRoutine(Random.Range(0f, rechargeTime))); // Offset
    }

    /// <summary>
    /// Hazard is recharging, then it will give a warning before becoming active
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator RechargeRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetTrigger("Charge");
        yield return new WaitForSeconds(4f);
        activeRoutine = StartCoroutine(ActiveRoutine());
    }

    /// <summary>
    /// Hazard is active
    /// </summary>
    /// <returns></returns>
    IEnumerator ActiveRoutine()
    {
        damageCollider.enabled = true;
        animator.SetBool("IsActive", true);
        yield return new WaitForSeconds(duration);
        damageCollider.enabled = false;
        animator.SetBool("IsActive", false);

        if (!isPaused) rechargeRoutine = StartCoroutine(RechargeRoutine(rechargeTime));
    }

    /// <summary>
    /// Hazard will despawn itself
    /// </summary>
    public void Despawn()
    {
        if (rechargeRoutine != null) StopCoroutine(rechargeRoutine);
        if (activeRoutine != null) StopCoroutine(activeRoutine);

        animator.SetTrigger("Despawn");
        Invoke(nameof(DelayDestroy), 2f);
    }

    /// <summary>
    /// Hazard will pause and reset itself
    /// </summary>
    public void Pause()
    {
        if (rechargeRoutine != null) StopCoroutine(rechargeRoutine);

        isPaused = true;
        animator.SetBool("Reset", true);
    }

    /// <summary>
    /// Delay destroy so that despawn animation can be played
    /// </summary>
    void DelayDestroy()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Kill anything that touches it while its active
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            if (!player.stats.isImmune)
            {
                player.KillPlayer();
            }
        }
        else if (other.tag == "Enemy")
        {
            Enemy e = other.GetComponent<Enemy>();

            if (e) e.OnHit(9999);
        }
        else if (other.tag == "Unit")
        {
            Unit u = other.GetComponent<Unit>();
            u.OnHit(9999);
        }
    }
}
