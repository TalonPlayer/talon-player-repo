using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public delegate void OnInteraction(); 
    public event OnInteraction onPrimaryInteraction;
    public event OnInteraction onSecondaryInteraction;


    [Header("Player Components")]
    public PlayerMovement movement;
    public PlayerInputHandler input;
    public PlayerCombat combat;
    public PlayerBody body;
    public int points;
    public bool isPrimaryFiring;
    public bool isSecondaryFiring;
    public float regenSpeed = 3f;
    public float underFireTime = 5f;
    public bool isUnderFire = false;
    public bool isRegening = false;
    private Coroutine underFireRoutine, regenRoutine;
    protected override void Awake()
    {
        base.Awake();

    }
    protected override void Start()
    {
        base.Start();
        death.AppendEvent(GameManager.Reset);
    }

    void Update()
    {
        if (isPrimaryFiring)
            onPrimaryInteraction?.Invoke();

        if (isSecondaryFiring)
            onSecondaryInteraction?.Invoke();
    }

    public void PrimaryInteraction(bool isFiring)
    {
        isPrimaryFiring = isFiring;
    }

    public void SecondaryInteraction(bool isFiring)
    {
        isSecondaryFiring = isFiring;
    }

    public void AddPoints(int p)
    {
        points += p;
        HUDManager.UpdatePoints(points);
    }

    public void Reset()
    {
        AddPoints(-points);
        AddPoints(50000);
        health = _maxHealth;
        isAlive = true;
        HUDManager.UpdateHealthBar(1f);

        
    }

    public override void OnHit(int damage, Entity attacker)
    {
        if (regenRoutine != null) StopCoroutine(regenRoutine);
        if (underFireRoutine != null) StopCoroutine(underFireRoutine);
        underFireRoutine = StartCoroutine(UnderFireRoutine());

        EntityManager.onHit += () =>
        {
            health -= damage;
            HUDManager.UpdateHealthBar((float) health / _maxHealth);
            CheckHealth();
        }; 
    }

    private IEnumerator UnderFireRoutine()
    {

        isUnderFire = true;
        isRegening = false;
        yield return new WaitForSeconds(underFireTime);
        isUnderFire = false;

        regenRoutine = StartCoroutine(Regeneration());
    }

    private IEnumerator Regeneration()
    {
        isRegening = true;
        int cur = health;
        float t = 0f;
        while (t < 1)
        {
            int lerp = Mathf.RoundToInt(Mathf.Lerp(0, _maxHealth, t));

            if (lerp > cur) health = lerp;
            t += Time.deltaTime / regenSpeed;

            HUDManager.UpdateHealthBar((float) health / _maxHealth);

            yield return null;

            if (!isRegening) break;
        }
        isRegening = false;
    }

    
}
