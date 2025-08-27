using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
    public bool controllerDetected;
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
        CheckForControllers();
    }

    /// <summary>
    /// Always check for a controller
    /// </summary>
    void CheckForControllers()
    {
        string[] joystickNames = Input.GetJoystickNames();

        if (joystickNames.Length > 0 && joystickNames.Any(name => !string.IsNullOrEmpty(name)))
        {
            foreach (string name in joystickNames)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    controllerDetected = true;
                    return;
                }
            }
        }
        controllerDetected = false;
    }


    void OnEnable()
    {
        movement.enabled = true;
        hand.enabled = true;
        body.enabled = true;
    }

    void OnDisable()
    {
        movement.enabled = false;
        hand.enabled = false;
        body.enabled = false;
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
