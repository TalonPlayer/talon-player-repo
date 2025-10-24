using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerHand combat;
    private Player player;
    private PlayerLobbyInfo stats;
    void Awake()
    {
        player = GetComponent<Player>();
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerHand>();
        stats = GetComponent<PlayerLobbyInfo>();
    }

    void FixedUpdate()
    {
        if (!player.controllerDetected)
        {
            combat.IsFiring(Input.GetMouseButton(0));
        }
    }

    public void OnMove(CallbackContext ctx)
    {
        if (movement != null)
            movement.MovementInput(ctx.ReadValue<Vector2>());
    }

    public void OnAim(CallbackContext ctx)
    {
        if (movement != null)
            combat.JoystickRotation(ctx.ReadValue<Vector2>());
    }

    public void OnNuke(CallbackContext ctx)
    {
        if (ctx.started)
            stats.DropNuke();
    }

    public void OnDash(CallbackContext ctx)
    {
        if (ctx.started)
        {
            movement.Dash();
        }
    }

    public void OnEmoteOne(CallbackContext ctx)
    {
        if (movement != null)
            movement.EmoteInput(-2);
    }
    public void OnEmoteTwo(CallbackContext ctx)
    {
        if (movement != null)
            movement.EmoteInput(-1);
    }
    public void OnEmoteThree(CallbackContext ctx)
    {
        if (movement != null)
            movement.EmoteInput(1);
    }
    public void OnEmoteFour(CallbackContext ctx)
    {
        if (movement != null)
            movement.EmoteInput(2);
    }


    public void OnFire(CallbackContext ctx)
    {
        if (combat != null && player.controllerDetected)
        {
            if (ctx.started)
            {
                combat.IsFiring(true);
            }
            else if (ctx.canceled)
            {
                combat.IsFiring(false);
            }
        }
    }
}
