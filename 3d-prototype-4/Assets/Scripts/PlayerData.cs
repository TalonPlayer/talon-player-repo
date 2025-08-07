using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string _name;
    public int highScore;
    public int currentScore;
    public int kills;
    public int skulls;
    public int gems;
    public int level;
    public int worldIndex;
    public int highestWorld;
    public int attempt;
    public string world;
    public string colorCode;

    public float masterVolume;
    public float sfxVolume;
    public float musicVolume;

    public PlayerData(PlayerInfo player)
    {
        _name = player._name;
        highScore = player.highScore;
        currentScore = player.currentScore;
        kills = player.kills;
        skulls = player.skulls;
        gems = player.gems;
        level = player.level;
        worldIndex = player.worldIndex;
        highestWorld = player.highestWorld;
        attempt = player.attempt;
        world = player.world;
        colorCode = player.colorCode;
    }

    public PlayerData(string name, string _colorCode)
    {
        _name = name;
        colorCode = _colorCode;
        highScore = 0;
        attempt = 0;
        highestWorld = -1;
    }
}
