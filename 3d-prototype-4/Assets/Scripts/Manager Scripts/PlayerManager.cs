using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public List<Player> players;
    public string colorCode;
    public Transform bulletFolder;
    public CinemachineVirtualCamera vcam;
    public CinemachineTargetGroup camTargetGroup;

    public int numOfPlayers = 0;

    [Header("Predetermined Stats")]
    public float immunityTime = 5f;


    [Header("Universal Player Items")]
    public PhysicalDrop gem;
    public Nuke nuke;
    public List<Drop> itemRewards;
    public List<Drop> unitRewards;
    public List<AudioClip> deathSounds;
    public UnityEvent onGameOver;
    public bool activeRedRoom;
    public int currentWorldIndex;
    public Transform camCenter;
    PlayerInputManager playerInputManager;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
        JoinNewPlayer();
    }

    void Update()
    {
        camCenter.position = PlayerCohesion();
    }

    public Vector3 PlayerCohesion()
    {
        Vector3 center = Vector3.zero;
        float avgDistance = 0f;
        int count = 0;

        for (int i = 0; i < players.Count; i++)
        {
            var a = players[i];
            if (a == null || !a.isAlive) continue;
            center += a.transform.position;
            count++;
        }

        if (count < 0) return Vector3.zero;

        center /= count;

        for (int i = 0; i < players.Count; i++)
        {
            var a = players[i];

            avgDistance += Vector3.Distance(a.transform.position, center);
        }

        avgDistance /= count;

        if (avgDistance > 9f) center.y += avgDistance - 9f;

        return center;
    }

    public void JoinNewPlayer()
    {
        if (playerInputManager == null)
        {
            Debug.LogError("PlayerInputManager reference missing.");
            return;
        }

        int desired = SaveSystem.currentPlayers.Count;

        
        // 1) Try spawn P1 on Keyboard+Mouse (optional)
        var p1 = playerInputManager.JoinPlayer(
            playerIndex: numOfPlayers,
            controlScheme: "Keyboard&Mouse",
            pairWithDevice: Keyboard.current
        );

        InitPlayerFromSave(p1, numOfPlayers);

        if (desired == 1) return;
        
         numOfPlayers++;
        // 2) Spawn remaining players on available gamepads
        foreach (var pad in Gamepad.all)
        {
            Debug.Log(pad.name);
            if (pad.name == "XInputControllerWindows") continue;

            // Join a new player with this device
            var input = playerInputManager.JoinPlayer(
                playerIndex: numOfPlayers,               // let Unity assign
                controlScheme: "Gamepad",
                pairWithDevice: pad
            );

            InitPlayerFromSave(input, numOfPlayers);

            numOfPlayers++;
        }

        if (numOfPlayers < desired)
        {
            Debug.LogWarning($"Not enough input devices to spawn all players. Wanted {desired}, joined {numOfPlayers}.");
        }
    }

    private void InitPlayerFromSave(PlayerInput input, int index)
    {
        var player = input.GetComponent<Player>();
        player.playerIndex = index;


        for (int i = 0; i < SaveSystem.currentPlayers.Count; i++)
        {
            Debug.Log(SaveSystem.currentPlayers[i]._name + " at index " + i);
        }
        player.stats.Init(SaveSystem.currentPlayers[index]);


        players.Add(player);

        // mark controller presence
        player.controllerDetected = (input.devices.Count > 0 && input.devices[0] is Gamepad);

        Debug.Log($"Player Joined → Index: {player.playerIndex}, Device: {(player.controllerDetected ? "Gamepad" : "Keyboard&Mouse")}");
    }

    public void TogglePause(bool isPaused)
    {
        Time.timeScale = isPaused ? 0 : 1;
        foreach (Player player in players)
            player.enabled = !isPaused;

        Cursor.visible = isPaused;
    }

    public void LifeScore(Player player)
    {
        if (player.stats.score >= player.stats.lifeScore)
        {
            player.stats.lifeScore += player.stats.extraLifeThreshold;
            GiveItemReward(player, 0);
        }
    }

    public void GiveUnitReward(Player player)
    {
        Vector3 pos = player.transform.position;
        pos.y += 20f;

        DropObject obj = DropManager.Instance.SpawnDropObject(unitRewards[Random.Range(0, unitRewards.Count - 1)], pos);

        obj.MoveTo(player.transform);
        obj.moveSpeed = 2.5f;
    }
    public void GiveItemReward(Player player, int num)
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
    public void KillPlayer(Player player)
    {

    }

    public IEnumerator DropGems(Vector3 pos, int num)
    {

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

        //PlayerData data = HudManager.Instance.GameOverStats(newData);

        //player.info.CopyPlayer(data);
        //player.info.SavePlayer();

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
    public void SpawnBufferedUnits(Player player)
    {
        bool pirateSpawned = false;
        foreach (string name in player.stats.bufferedUnits)
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
                    obj = DropManager.Instance.SpawnDropObject(unitRewards[1], pos);
                    obj.MoveTo(player.transform);
                    obj.moveSpeed = 2.5f;
                    break;
                case "Gunner":
                    obj = DropManager.Instance.SpawnDropObject(unitRewards[2], pos);
                    obj.MoveTo(player.transform);
                    obj.moveSpeed = 2.5f;
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
    public bool AllPlayersAlive()
    {
        foreach (Player player in players)
        {
            if (player.isAlive)
            {
                return true;
            }
        }

        return false;
    }
    public void GameOver()
    {
        onGameOver?.Invoke();
    }

    public Color GetColor(string colorCode)
    {
        Color color;
        if (!ColorUtility.TryParseHtmlString(colorCode, out color))
        {
            Debug.LogWarning("Invalid Hex Code");
            return new Color();
        }
        return color;
    }

    public void Stuck()
    {
        foreach(Player player in players)
        {
            player.TeleportPlayer(WorldManager.Instance.worldCenter);
        }
    }
}
