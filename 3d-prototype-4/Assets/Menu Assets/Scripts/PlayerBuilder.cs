using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerBuilder : MonoBehaviour
{
    public TMP_InputField playerName;
    public TMP_InputField playerHexColor;
    public TMP_InputField playerHat;
    public int playerIndex;
    public PlayerData playerData;
    public PlayerModel prefab;
    public PlayerModel instance;
    public string _name;
    public string colorCode;
    public int costumeIndex;
    public void InputName()
    {
        playerData._name = playerName.text;
        _name = playerName.text;
    }

    public void InputHexColor()
    {
        playerData.colorCode = "#" + playerHexColor.text;
        colorCode = "#" + playerHexColor.text;
        Debug.Log(playerData.colorCode);
    }

    public void InputHat()
    {
        playerData.costumeIndex = int.Parse(playerHat.text);
        costumeIndex = int.Parse(playerHat.text);
        Debug.Log(playerData.costumeIndex);
    }

    public void CreatePlayer()
    {
        if (instance == null)
        {
            Transform spawn = RandExt.RandomElement(LobbyManager.Instance.partySpawn);
            instance = Instantiate(prefab, spawn);
            LobbyManager.Instance.partySpawn.Remove(spawn);
        }

        instance.ChangeColor(colorCode);
        instance.ChangeOutfit(costumeIndex);
        instance.SetRandom();

    }
}
