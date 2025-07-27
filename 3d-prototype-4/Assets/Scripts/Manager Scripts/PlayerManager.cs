using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public Player player;
    public Transform bulletFolder;
    [Header("In-game Stats")]
    public int lives;
    public int dashes;
    public int nukes;
    public int multiplier = 1;
    public int score;
    public int multiplierValue = 0;
    public int maxMultiplierValue = 100;
    [Header("Predetermined Stats")]
    public float immunityTime = 5f;
    public int extraLifeThreshold = 200000;
    private int lifeScore;
    [Header("Player Effects")]
    public AudioSource playerAudio;
    public List<AudioClip> deathSounds;
    public ParticleSystem gStartImm;
    public GameObject gImmune;
    public GameObject yImmune;
    public Nuke nuke;
    public List<Drop> rewards;
    Coroutine immunityRoutine;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        lifeScore = extraLifeThreshold;
        HudManager.Instance.InitStats(lives, dashes, nukes, multiplier);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && nukes > 0)
        {
            Vector3 pos = player.transform.position;
            pos.y += 20f;
            Instantiate(nuke, pos, Quaternion.identity);
            nukes--;
            HudManager.Instance.UpdateText(2, nukes);
        }
    }

    public void AddMultiplier(int value)
    {
        if (multiplier == 9) return;
        multiplierValue += value;


        if (multiplierValue >= maxMultiplierValue)
        {
            multiplier++;
            multiplierValue = multiplierValue - maxMultiplierValue;
            maxMultiplierValue += 500;
            HudManager.Instance.UpdateText(3, multiplier);
        }

        HudManager.Instance.UpdateBar(0, multiplierValue, maxMultiplierValue);
    }

    public void AddScore(int _score)
    {
        score += multiplier * _score;
        HudManager.Instance.UpdateText(4, score);
        if (score >= lifeScore)
        {
            lifeScore += extraLifeThreshold;
            GrantLife();
        }
    }

    public bool CanDash()
    {
        return dashes > 0;
    }

    public void Dash()
    {
        dashes--;
        HudManager.Instance.UpdateText(1, dashes);
    }

    public void GrantLife()
    {
        if (lives < 9)
        {
            lives++;
            HudManager.Instance.UpdateText(0, lives);
        }
        else
        {
            GiveReward();
        }
    }
    public void GrantDash()
    {
        if (dashes < 9)
        {
            dashes++;
            HudManager.Instance.UpdateText(1, dashes);
        }
            
    }
    public void GrantNuke()
    {
        if (nukes < 9)
        {
            nukes++; 
           HudManager.Instance.UpdateText(2, nukes);
        }
            
    }
    public void GiveReward()
    {
        Vector3 pos = player.transform.position;
        pos.y += 20f;

        DropObject obj = DropManager.Instance.SpawnDropObject(rewards[Random.Range(0, rewards.Count)], pos);

        obj.MoveTo(player.transform);
        obj.moveSpeed = 2.5f;
    }
    public void KillPlayer()
    {
        player.OnDeath();
        PlaySound(deathSounds[Random.Range(0, deathSounds.Count)]);
        lives--;
        HudManager.Instance.UpdateText(0, lives);
        immunityRoutine = StartCoroutine(ImmunityRoutine(3f, immunityTime));
    }
    public IEnumerator ImmunityRoutine(float first, float secondTime)
    {
        yield return new WaitForSeconds(first);
        player.isImmune = true;
        gStartImm.Play();
        gImmune.SetActive(true);
        yield return new WaitForSeconds(secondTime);
        gImmune.SetActive(false);
        yImmune.SetActive(true);
        yield return new WaitForSeconds(secondTime);
        yImmune.SetActive(false);
        player.isImmune = false;
    }

    public void NukeImmunity()
    {
        if (immunityRoutine != null)
        {
            StopCoroutine(immunityRoutine);
        }
        immunityRoutine = StartCoroutine(ImmunityRoutine(0f, immunityTime / 2f));
    }

    public void TeleportPlayer(Transform location)
    {
        player.movement.enabled = false;
        player.hand.enabled = false;
        player.transform.position = location.position;
        player.transform.rotation = location.rotation;
        EntityManager.Instance.UnitSnapToPlayer();
        Invoke(nameof(EnableMovement), 2f);
    }

    public void EnableMovement()
    {
        player.movement.enabled = true;
        player.hand.enabled = true;
    }

    public void PlaySound(AudioClip clip)
    {
        playerAudio.PlayOneShot(clip, Random.Range(.8f, 1.4f));
    }

    /*
        (Info)

        Zombie kills worth 100 points
        Multiplier goes up by collecting silver, gold, and gems
        Start game with 3 lives, 2 dashes, 1 nuke
        Max number for multiplier, lives, dashes, and nukes is 9
        Every 200,000 points gives extra life
        If player already had 9 lives, grant them an AI buddy (that's permanent)
    */
}
