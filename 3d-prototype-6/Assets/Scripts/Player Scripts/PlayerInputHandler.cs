using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    private Player main;
    private PlayerInteraction interaction;
    void Awake()
    {
        main = GetComponent<Player>();
        interaction = GetComponent<PlayerInteraction>();
    }

    void Start()
    {

    }

    public void OnMove(CallbackContext ctx)
    {
        if (main.movement != null)
            main.movement.MovementInput(ctx.ReadValue<Vector2>());
    }
    public void OnLook(CallbackContext ctx)
    {
        if (main.movement != null)
            main.movement.LookInput(ctx.ReadValue<Vector2>());
    }
    public void OnJump(CallbackContext ctx)
    {
        if (main.movement != null)
            if (ctx.started)
                main.movement.Jump();
    }

    public void OnCrouch(CallbackContext ctx)
    {
        if (main.movement != null)
            if (ctx.started)
                main.movement.Crouch();
            else if (ctx.performed)
                main.movement.Prone();

    }

    public void OnSprint(CallbackContext ctx)
    {
        if (main.movement != null)
        {
            if (ctx.started)
                main.movement.SprintInput(true);
            else if (ctx.canceled)
                main.movement.SprintInput(false);
        }
    }

    public void OnPrimaryFire(CallbackContext ctx)
    {
        if (ctx.started)
        {
            main.PrimaryInteraction(true);
        }
        else if (ctx.canceled)
        {
            main.PrimaryInteraction(false);
        }
    }

    public void OnSecondaryFire(CallbackContext ctx)
    {
        if (ctx.started)
        {
            main.SecondaryInteraction(true);
        }
        else if (ctx.canceled)
        {
            main.SecondaryInteraction(false);
        }
    }

    public void OnMelee(CallbackContext ctx)
    {
        if (ctx.started)
        {
            main.combat.Melee();
        }
    }

    public void OnReload(CallbackContext ctx)
    {
        if (ctx.started)
        {
            main.combat.Reload();
        }
    }

    public void OnInteract(CallbackContext ctx)
    {
        if (ctx.started)
        {
            interaction.OnInteract();
        }
    }

    public void OnNextWave(CallbackContext ctx)
    {
        if (ctx.started)
        {
            WaveManager.Instance.SkipIntermission();
        }
    }
}
