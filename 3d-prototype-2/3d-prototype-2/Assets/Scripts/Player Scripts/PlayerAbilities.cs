using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    /*
    Ability Name
    Duration
    OnCast
    OnFinish
    is it a movement ability? <- turn off kinematics to influence movement
    is it a combat ability? <- prevent player from attacking
    is it both? <- prevent player from attacking and turn off kinematics
    Does the ability have multiple parts to it? How can we allow multiple functions in one ability?


    turn off movement requires playermovement to have a new boolean, -> movementAbility, use dashing.
    */

    private Player player;
    public void SetPlayer(Player p) => player = p;
    public Ability movementSKill;
    public Ability combatSkill;
    public float castCD;
    public bool isCasting = false;
    private Coroutine castRoutine;

    void Awake()
    {
        InitAbilities();
    }

    public void InitAbilities()
    {
        movementSKill.Init();
        combatSkill.Init();
    }

    void Update()
    {
        if (isCasting) return;

        if (Input.GetKeyDown(KeyCode.Q)) // Skill 1
        {
            CastSkill(movementSKill);
        }
        else if (Input.GetKeyDown(KeyCode.R)) // Skill 2
        {
            CastSkill(combatSkill);
        }
    }

    public void CastSkill(Ability ability)
    {
        if (!ability.conditionReady && !ability.ready) return;
        if (castRoutine != null)
        {
            StopCoroutine(castRoutine);
        }

        castRoutine = StartCoroutine(CastRoutine());
        ability.OnCast();
    }

    public IEnumerator CastRoutine()
    {
        isCasting = true;
        yield return new WaitForSeconds(castCD);
        isCasting = false;
    }
}
