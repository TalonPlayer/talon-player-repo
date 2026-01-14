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

    public bool isPrimaryFiring;
    public bool isSecondaryFiring;
    protected override void Awake()
    {
        base.Awake();

    }
    protected override void Start()
    {
        base.Start();

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
}
