using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ability : MonoBehaviour
{
    [Header("Info")]
    public string _name;
    public float duration;
    public bool hasDelay = false;
    public float delay;
    public float cooldown;
    public float range;
    public float damage;
    public float moveForce;
    public float knockBackForce;
    public Vector3 knockbackDirection;

    [Header("Types")]
    public ConditionType conType;
    public DamageType dmgType;

    [Header("AOE")]
    public GameObject hitbox;
    public Vector3 hitBoxSize;
    public float pivotOffset;


    [Header("Others")]
    public bool ready;
    public bool conditionReady;
    public UnityEvent onCast;
    public UnityEvent onDelay;
    public UnityEvent onFinish;
    public delegate bool Condition();
    public event Condition condition;


    // the direction offset based on the where the player is looking
    // Vector3.Zero would mean the dash would be forward
    public Vector3 forceDirection; // For Free Type
    private Vector3 targetPoint; // Save the hit point
    private PlayerMovement movement;
    private PlayerCombat combat;
    private Rigidbody rb;

    /*
        Name: Uppercut
        Duration: .5fs (in this case, the amount of time adding force upwards)
        Cooldown: 2f
        Range: 0f (type is set to free so leave as 0)
        Damage: 25f
        KBForce: 350f;
        KBDirection: Vector3.Up; (an additional force applied to change overall kb direction; leaving it as Vector3.Zero makes overall kb direction on enemies from the player to the enemy)
        Condition: Free to use
        DamageType: AOE

        onCast: 
            - Player can't attack on cast
            - PlayerMovement.DashState(), sets kinematic to false
            - Apply the force to the player and enable hitbox for this ability

        onFinish:
            - Player can attack
            - Player is now kinematic
            - hitbox turned off

    */
    void Start()
    {

    }

    public void Init()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();

        switch (conType)
        {
            case ConditionType.LocationAim:
                condition += () =>
                {
                    Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, range))
                    {
                        if (hit.collider.CompareTag("Ground"))
                        {
                            targetPoint = hit.point;
                            return true;
                        }
                    }
                    return false;
                };
                break;
            case ConditionType.FreeAim:
                condition += () =>
                {
                    return true;
                };
                break;
        }

        switch (dmgType)
        {
            case DamageType.AOE:
                hitbox.transform.position = transform.position + transform.forward * pivotOffset;
                hitbox.transform.localScale = hitBoxSize;
                break;
            case DamageType.Single:
                break;
            case DamageType.None:
                break;
        }
    }


    void Update()
    {
        conditionReady = condition();
    }

    public void OnCast()
    {
        if (ready)
        {
            if (!hasDelay)
            {
                onCast?.Invoke();
                StartCoroutine(DurationRoutine());
            }
            else
            {
                onDelay?.Invoke();
                Invoke(nameof(DelayCast), delay);
            }
        }
    }

    public void DelayCast()
    {
        onCast?.Invoke();
        StartCoroutine(DurationRoutine());
    }

    public void DashToPoint()
    {
        if (movement != null && conType == ConditionType.LocationAim)
        {
            movement.DashToPoint(targetPoint, moveForce);
        }
    }

    public void AttackAOE()
    {
        hitbox.SetActive(true);
        foreach (Enemy e in combat.Attack(hitbox.transform))
        {
            Vector3 kbDirection = ((e.transform.position - transform.position) * combat.knockBackForce) + (knockbackDirection * knockBackForce);
            e.OnHit(damage, kbDirection.normalized, knockBackForce);
        }
        hitbox.SetActive(false);
    }

    public void FreeDash()
    {
        movement.DashState(); // Enables ability movement mode
        rb.velocity = Vector3.zero;

        // Create directional offset based on where the player is looking (forward, up, right)
        Vector3 direction = 
            (movement.orientation.forward * forceDirection.z) +
            (Vector3.up * forceDirection.y) +
            (movement.orientation.right * forceDirection.x);

        // Normalize the direction to avoid exaggerated diagonals
        direction.Normalize();

        rb.AddForce(direction * moveForce);
    }

    public IEnumerator DurationRoutine()
    {
        ready = false;
        yield return new WaitForSeconds(duration);
        StartCoroutine(CooldownRoutine());
    }

    public IEnumerator CooldownRoutine()
    {
        onFinish?.Invoke();
        yield return new WaitForSeconds(cooldown);
        ready = true;
    }

    public enum ConditionType
    {
        LocationAim,
        FreeAim
    }

    public enum DamageType
    {
        AOE,
        Single,
        None
    }


}
