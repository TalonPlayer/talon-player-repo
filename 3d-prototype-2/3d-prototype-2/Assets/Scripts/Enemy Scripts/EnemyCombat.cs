using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class EnemyCombat : MonoBehaviour
{
    
    [Header("Player Detection")]
    public bool isAttacking = false;
    public bool noInterruption = false;
    public bool canAttack = true;
    public Coroutine attackRoutine;
    protected float distanceToTarget;
    protected Enemy enemy;
    public void SetEnemy(Enemy e) => enemy = e;
    public abstract void CheckDistance();
    public abstract void OnDeath();
    public Vector3 GetDirection(Vector3 targetPos, Vector3 thisPos){
        Vector3 direction = (targetPos - thisPos).normalized;
        return direction;
    }
}
