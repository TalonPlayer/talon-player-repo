using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Entity
{
    public Entity target;
    public Vector3 TargetPos
    {
        get
        {
            if (target != null) return target.transform.position;
            else return transform.position;
        }
    }
    [Header("Unit Components")]
    public NPCBody body;
    public UnitMovement movement;
    public UnitBehaviour ai;
    public UnitBrain brain;
    public UnitCombat combat;
    public UnitVision vision;
    public Collider coll;
    public int points = 0;
    public bool isUnderFire;
    public float underFireTime = 1f;
    private float _ufTimer;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        EntityManager.AddEnemy(this);
    }

    public void Init()
    {
        brain.Init();
        combat.Init();
    }

    void Update()
    {
        if (!isAlive) return;

        movement.ToggleMovement(!isUnderFire);

        if (isUnderFire)
        {
            _ufTimer -= Time.deltaTime;
            if (_ufTimer > 0f) return;
            isUnderFire = false;

        }
        else
        {
            movement.FSM();
            combat.FSM();
        }
        
    }
    public override void OnHit(int damage, Entity attacker)
    {
        if (!isAlive) return;
        isUnderFire = true;
        _ufTimer = underFireTime;
        body.BodyHit();
        base.OnHit(damage, attacker);
    }


    public override void OnDeath()
    {
        coll.excludeLayers = ~(Layer.Ground | Layer.Wall);
        base.OnDeath();
    }

    public UnitWorldState Tick(UnitWorldState s)
    {

        s = vision.Tick(s);
        s = movement.Tick(s);

        return s;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
