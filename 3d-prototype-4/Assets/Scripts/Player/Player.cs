using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using Cinemachine;

public class Player : Entity
{
    public UnityEvent onRespawn;
    public int playerIndex;
    public string colorCode;
    public bool controllerDetected;

    [Header("Player Effects")]
    public AudioSource playerAudio;
    public ParticleSystem gStartImm;
    public GameObject gImmune;
    public GameObject yImmune;
    public GameObject magnet;
    public GameObject immuneSphere;
    public Animator shield;
    public bool isTeleporting = false;
    public Transform teleportLocation;
    [HideInInspector] public PlayerMovement movement;
    [HideInInspector] public PlayerHand hand;
    [HideInInspector] public PlayerBody body;
    [HideInInspector] public PlayerLobbyInfo stats;
    [HideInInspector] public PlayerInfo info;
    [HideInInspector] public CapsuleCollider cc;

    Coroutine immunityRoutine;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        hand = GetComponent<PlayerHand>();
        body = GetComponent<PlayerBody>();
        stats = GetComponent<PlayerLobbyInfo>();
        info = GetComponent<PlayerInfo>();
        cc = GetComponent<CapsuleCollider>();
    }

    void Start()
    {

    }

    void FixedUpdate()
    {
        if (isTeleporting)
        {
            transform.SetPositionAndRotation(teleportLocation.position, teleportLocation.rotation);

            if ((transform.position - teleportLocation.position).magnitude < 0.01f)
            {
                FinishTeleport();
            }
        }
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
        if (stats.lives > 0)
            Invoke(nameof(Respawn), delay);
        else
            HudManager.Instance.ToggleDeadUI(playerIndex, true);
    }

    public void Respawn()
    {
        isAlive = true;
        onRespawn?.Invoke();
        HudManager.Instance.ToggleDeadUI(playerIndex, false);
        // Turn on animator
        // Ragdoll(false)
        // Turn on movement
    }

    public IEnumerator ImmunityRoutine(float first, float secondTime)
    {
        yield return new WaitForSeconds(first);
        immuneSphere.SetActive(true);
        stats.isImmune = true;
        gStartImm.Play();
        gImmune.SetActive(true);
        yield return new WaitForSeconds(secondTime);
        gImmune.SetActive(false);
        yImmune.SetActive(true);
        yield return new WaitForSeconds(secondTime);
        yImmune.SetActive(false);
        immuneSphere.SetActive(false);
        stats.isImmune = false;
    }
    public void NukeImmunity()
    {
        if (immunityRoutine != null)
        {
            StopCoroutine(immunityRoutine);
        }
        immunityRoutine = StartCoroutine(ImmunityRoutine(0f, PlayerManager.Instance.immunityTime / 2f));
    }

    public void TeleportPlayer(Transform location)
    {
        teleportLocation = location;
        DisableMovement();
        isTeleporting = true;
    }

    public void FinishTeleport()
    {
        isTeleporting = false;
        CinemachineVirtualCamera vcam = PlayerManager.Instance.vcam;
        Vector3 oldPos = transform.position;
        vcam.OnTargetObjectWarped(transform, transform.position - oldPos);

        // set only yaw on the vcam (Aim = Do nothing → vcam's own rotation is used)
        Vector3 camEuler = vcam.transform.eulerAngles;
        camEuler.y = transform.eulerAngles.y;
        vcam.ForceCameraPosition(vcam.transform.position, Quaternion.Euler(camEuler));

        EntityManager.Instance.UnitSnapToPlayer(this);
        EnableMovement();
    }

    public void EnableMovement()
    {
        movement.enabled = true;
        hand.enabled = true;
        movement.rb.useGravity = true;
    }
    public void DisableMovement()
    {
        movement.rb.useGravity = false;
        movement.enabled = false;
        hand.enabled = false;
    }

    public void PlaySound(AudioClip clip)
    {
        playerAudio.PlayOneShot(clip, Random.Range(.8f, 1.4f));
    }

    public void KillPlayer()
    {
        if (stats.isShielded)
        {
            stats.isShielded = false;
            shield.SetBool("Active", false);
            NukeImmunity();
            return;
        }
        OnDeath();
        PlaySound(RandExt.RandomElement(PlayerManager.Instance.deathSounds));
        StartCoroutine(PlayerManager.Instance.DropGems(transform.position, stats.multiplier - 1));
        stats.multiplier = 1;
        stats.multiplierValue = 0;
        stats.maxMultiplierValue = 100;

        if (stats.lives > 0)
        {
            stats.lives--;
            HudManager.Instance.UpdateText(playerIndex, 0, stats.lives);
            HudManager.Instance.UpdateText(playerIndex, 3, stats.multiplier);
            HudManager.Instance.UpdateBar(playerIndex, 0, stats.multiplierValue, stats.maxMultiplierValue);

            immunityRoutine = StartCoroutine(ImmunityRoutine(3f, PlayerManager.Instance.immunityTime));
        }
        /* end if all players are dead
        else
        {
            // End the game
            EndGame();
        }
        */
    }


}
