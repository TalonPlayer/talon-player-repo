using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public Player player;
    public string colorCode;
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
    public PhysicalDrop gem;
    public Animator shield;
    public Nuke nuke;
    public List<Drop> itemRewards;
    public List<Drop> unitRewards;
    public List<string> bufferedUnits;
    public UnityEvent onGameOver;
    public bool activeRedRoom;
    public int currentWorldIndex;
    private bool isShielded = false;
    Coroutine immunityRoutine;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        lifeScore = extraLifeThreshold;

        SaveSystem.LoadSelectedPlayerName(); // Load the previously selected name

        if (!string.IsNullOrEmpty(SaveSystem.selectedPlayerName))
        {
            PlayerData data = SaveSystem.LoadPlayer(SaveSystem.selectedPlayerName);
            player.info._name = data._name;
            player.info.attempt = data.attempt + 1;

            colorCode = data.colorCode;
            player.info.colorCode = colorCode;
            player.body.ChangeColor();
        }


        HudManager.Instance.InitStats(lives, dashes, nukes, multiplier);
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Joystick1Button4))  && nukes > 0)
        {
            Vector3 pos = player.transform.position;
            pos.y += 20f;
            Instantiate(nuke, pos, Quaternion.identity);
            nukes--;
            HudManager.Instance.UpdateText(2, nukes);
        }
    }

    public void PlayerIsStuck()
    {
        TeleportPlayer(WorldManager.Instance.worldCenter);
    }

    public void TogglePause(bool isPaused)
    {
        Time.timeScale = isPaused ? 0 : 1;
        player.enabled = !isPaused;
        Cursor.visible = isPaused;
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
        player.info.currentScore = score;
        if (score >= lifeScore)
        {
            lifeScore += extraLifeThreshold;
            GiveItemReward(0);
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
            if (Random.Range(0, 2) == 1)
                GiveUnitReward();
            else
                GiveItemReward(Random.Range(1, 3));
        }
    }
    public void GrantDash()
    {
        if (dashes < 9)
        {
            dashes++;
            HudManager.Instance.UpdateText(1, dashes);
        }
        else
        {
            GiveUnitReward();
        }

    }
    public void GrantNuke()
    {
        if (nukes < 9)
        {
            nukes++;
            HudManager.Instance.UpdateText(2, nukes);
        }
        else
        {
            GiveUnitReward();
        }

    }
    public void GiveUnitReward()
    {
        Vector3 pos = player.transform.position;
        pos.y += 20f;

        DropObject obj = DropManager.Instance.SpawnDropObject(unitRewards[Random.Range(0, unitRewards.Count)], pos);

        obj.MoveTo(player.transform);
        obj.moveSpeed = 2.5f;
    }
    public void GiveItemReward(int num)
    {
        Vector3 pos = player.transform.position;
        pos.y += 20f;
        DropObject obj;
        switch (num)
        {
            case 0: // Life
                obj = DropManager.Instance.SpawnDropObject(itemRewards[0], pos);
                obj.MoveTo(player.transform);
                obj.moveSpeed = 2.5f;
                break;

            case 1: // Dash
                for (int i = 0; i < 3; i++)
                {
                    obj = DropManager.Instance.SpawnDropObject(itemRewards[1], pos);
                    obj.MoveTo(player.transform);
                    obj.moveSpeed = 2.5f;
                }
                
                break;
            case 2: // Nuke
                for (int i = 0; i < 2; i++)
                {
                    obj = DropManager.Instance.SpawnDropObject(itemRewards[2], pos);
                    obj.MoveTo(player.transform);
                    obj.moveSpeed = 2.5f;
                }
                break;
        }


    }
    public void KillPlayer()
    {
        if (isShielded)
        {
            isShielded = false;
            shield.SetBool("Active", false);
            NukeImmunity();
            return;
        }
        player.OnDeath();
        PlaySound(deathSounds[Random.Range(0, deathSounds.Count)]);
        StartCoroutine(DropGems(multiplier - 1));
        multiplier = 1;
        multiplierValue = 0;
        maxMultiplierValue = 100;

        if (lives > 0)
        {
            lives--;
            HudManager.Instance.UpdateText(0, lives);
            HudManager.Instance.UpdateText(3, multiplier);
            HudManager.Instance.UpdateBar(0, multiplierValue, maxMultiplierValue);

            immunityRoutine = StartCoroutine(ImmunityRoutine(3f, immunityTime));
        }
        else
        {
            // End the game
            EndGame();
        }
    }

    IEnumerator DropGems(int num)
    {
        Vector3 pos = player.transform.position;

        pos.y += 3f;
        for (int i = 0; i < num; i++)
        {

            PhysicalDrop g = Instantiate(
                gem,
                pos,
                Quaternion.identity
            );

            g.LaunchUp();

            yield return new WaitForSeconds(.15f);
        }
    }

    public void EndGame()
    {
        foreach (Unit u in EntityManager.Instance.units) u.OnHit(9999);
        PlayerData newData = new PlayerData(player.info);

        PlayerData data = HudManager.Instance.GameOverStats(newData);

        player.info.CopyPlayer(data);
        player.info.SavePlayer();

        StartCoroutine(EndGame(8f, 3f));
    }

    public IEnumerator EndGame(float firstTime, float secondtime)
    {
        yield return new WaitForSeconds(firstTime);
        HudManager.Instance.ToggleBlackScreen(true);
        yield return new WaitForSeconds(secondtime);
        SceneWorldManager.Instance.EndScene(WorldManager.Instance.worldIndex);
        SceneWorldManager.Instance.TransferToMenu();
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
        player.movement.rb.useGravity = false;
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
        player.movement.rb.useGravity = true;
    }

    public void PlaySound(AudioClip clip)
    {
        playerAudio.PlayOneShot(clip, Random.Range(.8f, 1.4f));
    }

    public void SpawnBufferedUnits()
    {
        bool ankleBiterSpawned = false;
        bool pirateSpawned = false;
        bool gunnerSpawned = false;
        foreach (string name in bufferedUnits)
        {
            Vector3 pos = player.transform.position;
            pos.y += 20f;
            DropObject obj;
            switch (name)
            {
                case "Skeleton":
                    obj = DropManager.Instance.SpawnDropObject(unitRewards[0], pos);
                    obj.MoveTo(player.transform);
                    obj.moveSpeed = 2.5f;
                    break;
                case "Ankle Biter":
                    if (ankleBiterSpawned) break;
                    obj = DropManager.Instance.SpawnDropObject(unitRewards[1], pos);
                    obj.MoveTo(player.transform);
                    obj.moveSpeed = 2.5f;
                    ankleBiterSpawned = true;
                    break;
                case "Gunner":
                    if (gunnerSpawned) break;
                    obj = DropManager.Instance.SpawnDropObject(unitRewards[2], pos);
                    obj.MoveTo(player.transform);
                    obj.moveSpeed = 2.5f;
                    gunnerSpawned = true;
                    break;
                case "Warrior":
                    obj = DropManager.Instance.SpawnDropObject(unitRewards[3], pos);
                    obj.MoveTo(player.transform);
                    obj.moveSpeed = 2.5f;
                    break;
                case "Pirate":
                    if (pirateSpawned) break;
                    obj = DropManager.Instance.SpawnDropObject(unitRewards[4], pos);
                    obj.MoveTo(player.transform);
                    obj.moveSpeed = 2.5f;
                    pirateSpawned = true;
                    break;
            }
        }
    }

    public void GameOver()
    {


        onGameOver?.Invoke();
    }

    public Color GetColor()
    {
        Color color;
        if (!ColorUtility.TryParseHtmlString(colorCode, out color))
        {
            Debug.LogWarning("Invalid Hex Code");
            return new Color();
        }
        return color;
    }

    public void GrantShield()
    {
        if (isShielded) return;

        shield.SetBool("Active", true);
        isShielded = true;
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
