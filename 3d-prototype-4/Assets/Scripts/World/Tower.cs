using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float rechargeTime;
    public float duration;
    public CapsuleCollider damageCollider;
    public Animator animator;
    Coroutine rechargeRoutine;
    Coroutine activeRoutine;
    public void Init()
    {
        rechargeRoutine = StartCoroutine(RechargeRoutine(Random.Range(5f, 15f))); // Offset
    }
    IEnumerator RechargeRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetTrigger("Charge");
        yield return new WaitForSeconds(4f);
        activeRoutine = StartCoroutine(ActiveRoutine());
    }
    IEnumerator ActiveRoutine()
    {
        damageCollider.enabled = true;
        animator.SetBool("IsActive", true);
        yield return new WaitForSeconds(duration);
        damageCollider.enabled = false;
        animator.SetBool("IsActive", false);

        rechargeRoutine = StartCoroutine(RechargeRoutine(rechargeTime));
    }

    public void Despawn()
    {
        if (rechargeRoutine != null) StopCoroutine(rechargeRoutine);
        if (activeRoutine != null) StopCoroutine(activeRoutine);

        animator.SetTrigger("Despawn");
        Invoke(nameof(DelayDestroy), 5f);
    }

    void DelayDestroy()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            if (!player.isImmune || !player.movement.isDashing)
                PlayerManager.Instance.KillPlayer();
        }
        else if (other.tag == "Enemy")
        {
            Enemy e = other.GetComponent<Enemy>();
            e.OnHit(9999);
        }
        else if (other.tag == "Unit")
        {
            Unit u = other.GetComponent<Unit>();
            u.OnHit(9999);
        }
    }
}
