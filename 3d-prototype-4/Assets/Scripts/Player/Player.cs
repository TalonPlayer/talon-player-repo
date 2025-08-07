using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : Entity
{
    public UnityEvent onRespawn;
    public bool isImmune;
    public int score;
    [HideInInspector] public PlayerMovement movement;
    [HideInInspector] public PlayerHand hand;
    [HideInInspector] public PlayerBody body;
    [HideInInspector] public PlayerInfo info;
    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        hand = GetComponent<PlayerHand>();
        body = GetComponent<PlayerBody>();
        info = GetComponent<PlayerInfo>();
    }
    void Start()
    {

    }

    void Update()
    {

    }

    public void OnDeath()
    {
        isAlive = false;
        onDeath?.Invoke();
        // Turn off Animator
        // Ragdoll(true)
        // Turn off movement
        // DelayRespawn 3 seconds?
    }

    public void DelayRespawn(float delay)
    {
        if (PlayerManager.Instance.lives > 0)
            Invoke(nameof(Respawn), delay);
    }

    public void Respawn()
    {
        isAlive = true;
        onRespawn?.Invoke();
        // Turn on animator
        // Ragdoll(false)
        // Turn on movement
    }
}
