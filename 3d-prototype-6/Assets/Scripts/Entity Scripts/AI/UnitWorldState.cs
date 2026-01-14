using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitWorldState
{
    // Awareness
    public bool HasTargets { get { return Targets.Count > 0; } } // Do I have Targets
    public bool TargetAcquired = false; // Have I chosen a target
    public bool HasClearShot = false; // Is there a clear view of the target from where I am?
    public bool HasAimOnTarget = false; // Am I aiming at the target
    public bool TargetAlive = false;
    public bool HasCoverNearby = false;
    public bool HasAllies = false;
    public bool TargetsCanBeSeen = false; // At least one target is visible

    // Tactical Status
    public bool IsAggro = false;
    public bool IsUnderFire = false;
    public bool IsLowHealth = false;
    public bool InMeleeRange = false;
    public bool InCombatRange = false;
    public bool InAllyRange = false;
    public bool InCover = false;
    public bool CoverBlown = false;

    // Weapon / Ammo
    public bool HasWeapon = false;
    public bool IsWeaponRanged = false;
    public bool HasAmmo = false;
    public bool HasMags = false;
    public bool IsWeaponEmpty = false;
    public List<Entity> Targets = new List<Entity>();
    public List<Entity> ViewableTargets = new List<Entity>();

    public UnitWorldState(){}
}