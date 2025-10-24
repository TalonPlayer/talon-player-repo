using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ISpyWorld : Entity
{
    public ParticleSystem bloodParticles;
    public List<AudioClip> damageSounds;
    public AudioSource damageAudio;
    public List<SpecialEnemySpawn> zombieSpawns;
    public float minCD = 10f;
    public float maxCD = 20f;
    public float maxHealth;
    public bool isAttacking = false;
    public Image healthBar;
    public Animator animator;
    Coroutine attackRoutine;
    void Start()
    {
        attackRoutine = StartCoroutine(Attack(Random.Range(10f, 20f)));
    }
    IEnumerator Attack(float WaitTime)
    {
        yield return new WaitForSeconds(WaitTime);

        isAttacking = true;
        animator.SetBool("IsAttacking", true);
        int rand = Random.Range(0, 2);
        animator.SetInteger("Attack", rand);
        switch (rand)
        {
            case 0: // Spawn Zombies
                foreach (SpecialEnemySpawn obj in zombieSpawns)
                {
                    obj.ImmediateSpawn();
                }
                Invoke(nameof(StopAttack), 2.5f);
                break;
            case 1: // Charge
                Invoke(nameof(StopAttack), 4.5f);
                break;
            case 2: // Shoot Projectiles
                break;
            case 3:
                break;
            case 4:
                break;
        }

        attackRoutine = StartCoroutine(Attack(Random.Range(10f, 20f)));
    }
    public void StopAttack()
    {

        isAttacking = false;
        animator.SetBool("IsAttacking", false);
    }
    public void OnHit(int damage)
    {
        if (bloodParticles != null)
        {
            bloodParticles.Play();
        }
        damageAudio.PlayOneShot(RandExt.RandomElement(damageSounds));

        // Subscribe the damage taken so that all damage can happen on the same tick
        EntityManager.damageTick += () =>
        {
            health -= damage;
            healthBar.fillAmount = health / maxHealth;
        };
    }
}
