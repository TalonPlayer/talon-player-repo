using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerMovement movement;
    [HideInInspector] public PlayerCombat combat;
    [HideInInspector] public PlayerInteract interact;
    [HideInInspector] public PlayerInventory inventory;
    [HideInInspector] public PlayerGardening gardening;
    [HideInInspector] public PlayerBody body;
    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
        interact = GetComponent<PlayerInteract>();
        inventory = GetComponent<PlayerInventory>();
        gardening = GetComponent<PlayerGardening>();
        body = GetComponent<PlayerBody>();
    }

    public void OnEnable()
    {
        movement.enabled = true;
        combat.enabled = true;
        interact.enabled = true;
        inventory.enabled = true;
        gardening.enabled = true;
        body.enabled = true;
    }

    public void OnDisable()
    {
        movement.enabled = false;
        combat.enabled = false;
        interact.enabled = false;
        inventory.enabled = false;
        gardening.enabled = false;
        body.enabled = false;
    }

    void Start()
    {

    }

    void Update()
    {
        
    }
}
