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
    public bool isUnderFire;
    public float underFireTime = 5f;
    public float underFireThreshold = 0f; // Damage that will make the AI consider running away
    private float _undCurrent;
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

    void Update()
    {
        if (!isAlive) return;

        movement.FSM();
        combat.FSM();
        
        if (isUnderFire)
        {
            _ufTimer -= Time.deltaTime;
            if (_ufTimer > 0f) return;
            _undCurrent = 0f;
            isUnderFire = false;
        }
        
    }
    public override void OnHit(int damage, Entity attacker)
    {
        if (!isAlive) return;
        isUnderFire = true;
        _ufTimer = underFireTime;
        _undCurrent += damage;
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
}
