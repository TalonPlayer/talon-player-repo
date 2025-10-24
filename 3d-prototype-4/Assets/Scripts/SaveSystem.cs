using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public static class SaveSystem
{
    // The saving system
    public static string selectedPlayerName;
    private static string playerListPath = Application.persistentDataPath + "/players.data";
    public static List<PlayerData> currentPlayers = new List<PlayerData>();
    public static void SavePlayer(PlayerInfo player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player_" + player._name + ".data";
        Debug.Log(path);
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);

        formatter.Serialize(stream, data);
        stream.Close();

        AddPlayerToList(player._name);
    }

    public static PlayerData LoadPlayer(string name)
    {
        string path = Application.persistentDataPath + "/player_" + name + ".data";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            PlayerData data = (PlayerData)formatter.Deserialize(stream);
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    private static void AddPlayerToList(string name)
    {
        List<string> players = LoadPlayerList();

        if (!players.Contains(name))
        {
            players.Add(name);
            SavePlayerList(players);
        }
    }
    public static List<string> LoadPlayerList()
    {
        if (File.Exists(playerListPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(playerListPath, FileMode.Open);
            List<string> players = (List<string>)formatter.Deserialize(stream);
            stream.Close();
            return players;
        }
        else
        {
            return new List<string>();
        }
    }

    private static void SavePlayerList(List<string> players)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(playerListPath, FileMode.Create);
        formatter.Serialize(stream, players);
        stream.Close();
    }

    public static void DeletePlayer(string name)
    {
        // Delete the player data file
        string path = Application.persistentDataPath + "/player_" + name + ".data";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {
            Debug.LogWarning("Player data file not found: " + path);
        }

        // Remove the name from the list
        List<string> players = LoadPlayerList();
        if (players.Contains(name))
        {
            players.Remove(name);
            SavePlayerList(players);
        }
    }

    public static void LoadSelectedPlayerName()
    {
        if (PlayerPrefs.HasKey("SelectedPlayer"))
        {
            selectedPlayerName = PlayerPrefs.GetString("SelectedPlayer");
        }
        else
        {
            selectedPlayerName = null;
        }
    }
}
