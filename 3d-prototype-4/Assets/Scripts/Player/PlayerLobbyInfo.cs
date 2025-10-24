using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLobbyInfo : MonoBehaviour
{
    [Header("Player Stats")]
    public int lives;
    public int dashes;
    public int nukes;
    public int multiplier = 1;
    public int score;
    public int multiplierValue = 0;
    public int maxMultiplierValue = 100;
    public int extraLifeThreshold = 200000;
    public int lifeScore;
    public List<string> bufferedUnits;

    [Header("Player States")]
    public bool isImmune;
    public bool isRampage;
    public bool isShielded = false;
    Coroutine magnetRoutine;
    Coroutine rampageRoutine;

    private Player player;
    void Awake()
    {
        player = GetComponent<Player>();
    }

    public void Init(PlayerData data)
    {
        lifeScore = extraLifeThreshold;
        Debug.Log("data: " + data._name + "," + data.colorCode + ", " + data.costumeIndex);

        player._name = data._name;
        player.info._name = data._name;
        player.info.attempt = data.attempt + 1;
        player.info.costumeIndex = data.costumeIndex;
        player.info.colorCode = data.colorCode;
        player.colorCode = data.colorCode;
        Debug.Log("player Info: " + data._name + "," + data.colorCode + ", " + data.costumeIndex);

        player.body.ChangeColor();
        player.body.SetCostume();
        HudManager.Instance.InitStats(player.playerIndex, lives, dashes, nukes, multiplier, player.colorCode);
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
            HudManager.Instance.UpdateText(player.playerIndex, 3, multiplier);
        }

        HudManager.Instance.UpdateBar(player.playerIndex, 0, multiplierValue, maxMultiplierValue);
    }

    public void AddScore(int _score)
    {
        score += multiplier * _score;
        HudManager.Instance.UpdateText(player.playerIndex, 4, score);
        player.info.currentScore = score;
        PlayerManager.Instance.LifeScore(player);
    }

    public bool CanDash()
    {
        return dashes > 0;
    }

    public void Dash()
    {
        dashes--;
        HudManager.Instance.UpdateText(player.playerIndex, 1, dashes);
    }

    public void GrantLife()
    {
        if (lives < 9)
        {
            lives++;
            HudManager.Instance.UpdateText(player.playerIndex, 0, lives);
        }
        else
        {
            GlobalSaveSystem.AddAchievementProgress("player_lives_9", 1);
            if (Random.Range(0, 2) == 1)
                PlayerManager.Instance.GiveUnitReward(player);
            else
                PlayerManager.Instance.GiveItemReward(player, Random.Range(1, 3));
        }
    }
    public void GrantDash()
    {
        if (dashes < 9)
        {
            dashes++;
            HudManager.Instance.UpdateText(player.playerIndex, 1, dashes);
        }
        else
        {
            PlayerManager.Instance.GiveUnitReward(player);
        }

    }
    public void GrantNuke()
    {
        if (nukes < 9)
        {
            nukes++;
            HudManager.Instance.UpdateText(player.playerIndex, 2, nukes);
        }
        else
        {
            PlayerManager.Instance.GiveUnitReward(player);
        }

    }

    public void DropNuke()
    {
        if (nukes > 0)
        {
            Vector3 pos = transform.position;
            pos.y += 20f;
            Nuke nuke = Instantiate(PlayerManager.Instance.nuke, pos, Quaternion.identity);
            nuke.Init(player);
            nukes--;
            HudManager.Instance.UpdateText(player.playerIndex, 2, nukes);
        }
    }



    public void StartRampage()
    {
        if (!isRampage)
            PlayerCostumeManager.Instance.ChangeCostume(1, true);

        isRampage = true;
        HudManager.Instance.rampageScreen.SetActive(true);
        if (rampageRoutine != null) StopCoroutine(rampageRoutine);
        rampageRoutine = StartCoroutine(RampageRoutine());
    }
    IEnumerator RampageRoutine()
    {
        yield return new WaitForSeconds(15f);
        HudManager.Instance.rampageScreen.SetActive(false);
        isRampage = false;
        player.hand.Equip(player.hand.defaultWeapon);
        PlayerCostumeManager.Instance.ResetCostume();
        GlobalSaveSystem.ResetProgress("Sword_kills");
    }

    public void CancelRampage()
    {
        if (rampageRoutine != null) StopCoroutine(rampageRoutine);
        HudManager.Instance.rampageScreen.SetActive(false);
        isRampage = false;
        PlayerCostumeManager.Instance.ResetCostume();
        GlobalSaveSystem.ResetProgress("Sword_kills");
    }

    public void GrantShield()
    {
        if (isShielded) return;

        player.shield.SetBool("Active", true);
        isShielded = true;
    }

    public void GrantMagnet()
    {
        if (magnetRoutine != null) StopCoroutine(magnetRoutine);
        magnetRoutine = StartCoroutine(MagnetRoutine(10f));
    }

    IEnumerator MagnetRoutine(float time)
    {
        player.magnet.SetActive(true);
        yield return new WaitForSeconds(time);

        player.magnet.SetActive(false);
    }
}
