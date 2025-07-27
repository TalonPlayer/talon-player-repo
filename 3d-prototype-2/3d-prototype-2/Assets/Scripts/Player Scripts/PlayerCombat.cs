using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float hitRange;
    public LayerMask enemyLayer;
    public Transform hitBox;
    public Transform kickHitBox;
    public Transform head;


    public float attackForce;
    public float flyingKickForce = 25f;
    public float knockBackForce = 75f;
    public float attackCooldown;
    public float armInterval;
    public float attackDelay = .015f;
    public int combo = 0;

    private bool canAttackL = true;
    private bool canAttackR = true;
    public bool canAttack = true;
    public Coroutine leftRoutine;
    public Coroutine rightRoutine;
    private Player player;
    public PlayerAudio _audio;
    private int attacked = 0;
    public void SetPlayer(Player p) => player = p;
    void Update(){
        if (!canAttack) return;

            if (Input.GetMouseButtonDown(0) && canAttackL)
            {
                Punch("Left");

            }

            if (Input.GetMouseButtonDown(1) && canAttackR)
            {
                Punch("Right");
            }


    }

    void Punch(string side)
    {
        canAttack = false;
        StartCoroutine(ResetRoutine(attackCooldown, "canAttack"));
        StartCoroutine(DelayAttack(hitBox, player.punchDamage));

        _audio.PlayWooshSound();
        player.IntAnim("AttackRand", Random.Range(0, 2));
        switch (side)
        {
            case "Left":
                player.animator.Play("Left Attack");
                player.PlayAnim("Left Punch");
                canAttackL = false;
                if (rightRoutine != null)
                {
                    canAttackR = true;
                    StopCoroutine(rightRoutine);
                }
                leftRoutine = StartCoroutine(ResetRoutine(armInterval + attackCooldown, "canAttackL"));
                break;
            case "Right":
                player.animator.Play("Right Attack");
                player.PlayAnim("Right Punch");
                canAttackR = false;
                if (leftRoutine != null)
                {
                    canAttackL = true;
                    StopCoroutine(leftRoutine);
                }
                rightRoutine = StartCoroutine(ResetRoutine(armInterval + attackCooldown, "canAttackR"));
                break;
        }

    }
    public IEnumerator DelayAttack(Transform hitBox, float damage)
    {
        yield return new WaitForSeconds(attackDelay);
        foreach (Enemy e in Attack(hitBox))
        {
            Vector3 kbDirection = (e.transform.position - transform.position).normalized;
            kbDirection.y *= .25f;

            e.OnHit(damage, kbDirection, attackForce);
        }

        if (attacked > 0)
        {
            _audio.PlayHitSound();
        }
    }

    public List<Enemy> Attack(Transform hitBox)
    {
        Vector3 center = head.position + head.forward * 1.0f;
        Collider[] hit = Physics.OverlapBox(
            center,
            hitBox.localScale,
            head.rotation,
            enemyLayer);
        List<Enemy> enemies = new List<Enemy>();
        attacked = 0;
        foreach (Collider coll in hit)
        {
            if (coll.gameObject.tag == "Enemy")
            {
                Enemy enemy = coll.GetComponent<Enemy>();
                if (!enemy.isAggro) enemy.onAggro?.Invoke();
                if (!enemies.Contains(enemy)) enemies.Add(enemy);
                attacked++;
            }
        }
        combo += attacked;
        combo = Mathf.Min(combo, 8);

        if (combo == 8)
        {
            combo = 0;
            player.health++;
            player.health = Mathf.Min(player.health, 8);
            HUDController.Instance.ChangeHealth(player.health);
        }

        HUDController.Instance.UpdateCombo(combo);
        EnemyManager.Instance.NearAggro(15f, transform.position);
        return enemies;
    }

    IEnumerator ResetRoutine(float time, string attackType)
    {
        yield return new WaitForSeconds(time);
        switch (attackType)
        {
            case "canAttack":
            canAttack = true;
                break;
            case "canAttackL":
            canAttackL = true;
                break;
            case "canAttackR":
            canAttackR = true;
                break;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (player.movement.flyKickWindow)
        {
            if (other.collider.tag == "Enemy")
            {
                player.PlayAnim("Right Arm Punch");
                player.movement.flyKickWindow = false;
                player.movement.StopMovement();
                foreach (Enemy e in Attack(kickHitBox))
                {
                    if (!e.isAggro) e.TargetPlayer();
                    Vector3 kbDirection = e.transform.position - player.movement.startingPos;

                    if (attacked > 4)
                        e.OnHit(player.flyKickDamage, kbDirection, flyingKickForce * 1.25f);
                    else
                        e.OnHit(player.flyKickDamage, kbDirection, flyingKickForce);

                    kbDirection = (transform.position - e.transform.position).normalized;
                    kbDirection.y = 0f;
                    player.movement.KnockBack(kbDirection, knockBackForce);
                }

                if (attacked > 0)
                {
                    _audio.PlayKickSound();
                }
            }
        }
        else if (player.movement.abilityWindow)
        {
            if (other.collider.tag == "Enemy")
            {
                player.PlayAnim("Right Arm Punch");
                player.movement.abilityMovement = false;
                player.movement.StopMovement();
                foreach (Enemy e in Attack(player.abilities.combatSkill.hitbox.transform))
                {
                    if (!e.isAggro) e.TargetPlayer();
                    Vector3 kbDirection = e.transform.position - player.movement.startingPos;

                    e.OnHit(player.abilities.combatSkill.damage, kbDirection, player.abilities.combatSkill.knockBackForce);
                }

                if (attacked > 0)
                {
                    _audio.PlayKickSound();
                }
            }  
        }
    }

    void OnDrawGizmos()
    {
        if (head == null || hitBox== null) return;

        Gizmos.color = Color.red;
        Vector3 center = head.position + head.forward * 1.5f;

        // Set the matrix to apply rotation and position
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(
            center,
            head.rotation,
            Vector3.one
        );

        Gizmos.matrix = rotationMatrix;

        // Draw a wireframe box using half-extents (scale * 0.5)
        Gizmos.DrawWireCube(Vector3.zero, hitBox.localScale);
        // Draw a wireframe box using half-extents (scale * 0.5)
        Gizmos.DrawWireCube(Vector3.zero, kickHitBox.localScale);
    }
}
