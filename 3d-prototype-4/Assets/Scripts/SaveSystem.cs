using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public static class SaveSystem
{
    public static string selectedPlayerName;
    private static string playerListPath = Application.persistentDataPath + "/players.data";
    public static void SavePlayer(PlayerInfo player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player_" + player._name + ".data";
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
}
