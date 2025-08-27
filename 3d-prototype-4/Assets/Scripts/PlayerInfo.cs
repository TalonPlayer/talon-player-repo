using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    // Saves the Player Data into the system
    public string _name;
    public int highScore = 0;
    public int currentScore = 0;
    public int kills = 0;
    public int skulls = 0;
    public int gems = 0;
    public int level = 0;
    public int worldIndex = 0;
    public int highestWorld = 0;
    public int attempt = 0;
    public string world = "";
    public string colorCode;
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayer(string name)
    {
        PlayerData data = SaveSystem.LoadPlayer(name);
        _name = data._name;
        highScore = data.highScore;
        currentScore = data.currentScore;
        kills = data.kills;
        skulls = data.skulls;
        gems = data.gems;
        level = data.level;
        worldIndex = data.worldIndex;
        highestWorld = data.highestWorld;
        attempt = data.attempt;
        world = data.world;
        colorCode = data.colorCode;
    }

    public void CopyPlayer(PlayerData data)
    {
        _name = data._name;
        highScore = data.highScore;
        currentScore = data.currentScore;
        kills = data.kills;
        skulls = data.skulls;
        gems = data.gems;
        level = data.level;
        worldIndex = data.worldIndex;
        highestWorld = data.highestWorld;
        attempt = data.attempt;
        world = data.world;
        colorCode = data.colorCode;
    }
}
