using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    public List<PlayerData> players;
    public List<PlayerBuilder> builders;
    public Transform localList;
    public List<Transform> partySpawn;
    public PlayerBuilder buildPrefab;
    public int numOfPlayers = 0;
    void Awake()
    {
        Instance = this;
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        PlayerData data = new PlayerData();

        PlayerBuilder b = input.GetComponent<PlayerBuilder>();
        b.transform.parent = localList;

        b.playerData = data;
        b.playerIndex = numOfPlayers;
        players.Add(data);
        numOfPlayers++;
    }
}
