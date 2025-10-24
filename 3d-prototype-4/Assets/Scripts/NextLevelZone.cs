using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelZone : MonoBehaviour
{
    // Zones that handle the transfer to new levels and worlds
    public Transform playerEnterPoint;
    public Transform door;
    public bool isPortal = false;
    public bool active = true;
    private Zone zone;
    void Awake()
    {
        if (isPortal)
        {
            zone = GetComponent<Zone>();
            zone.enabled = false;
            Invoke(nameof(EnablePortal), 2f);
        }
    }
    void EnablePortal()
    {
        zone.enabled = true;
    }

    public void OpenDoor()
    {
        door.gameObject.SetActive(false);
    }

    public void CloseDoor()
    {
        door.gameObject.SetActive(true);
    }

    /// <summary>
    /// Teleports player to transform
    /// </summary>
    public void Spawn()
    {
        HudManager.Instance.ToggleBlackScreen(false);

        foreach (Player player in PlayerManager.Instance.players)
        {
            player.TeleportPlayer(playerEnterPoint);
        }
    }

    public void DelaySpawn(float delay)
    {
        DisableInput();
        Invoke(nameof(Spawn), delay);
    }

    public void DelayTransfer(float delay)
    {
        HudManager.Instance.ToggleBlackScreen(true);
        DisableInput();
        Invoke(nameof(TransferToNewWorld), delay);
    }

    public void DelayReset(float delay)
    {
        Invoke(nameof(DelayReset), delay);
    }

    public void DelayReset()
    {
        HudManager.Instance.ToggleBlackScreen(true);
        DisableInput();
        Invoke(nameof(ResumeWorld), 2f);
    }

    /// <summary>
    /// Transfers player to new world
    /// </summary>
    public void TransferToNewWorld()
    {
        if (!active) return;
        active = false;
        foreach (Player player in PlayerManager.Instance.players)
        {
            player.stats.bufferedUnits.Clear();

            List<Unit> units = EntityManager.Instance.units.FindAll(u => u.owner == player);

            foreach (Unit u in units)
            {
                player.stats.bufferedUnits.Add(u._name);
            }
        }
        foreach (Unit u in EntityManager.Instance.units)
            u.OnHit(9999);
        EntityManager.Instance.ClearAggro();

        HudManager.Instance.DisablePointers();
        HudManager.Instance.AdvanceLevel("hide", 0);
        AudioManager.Instance.bgAudio.Stop();
        SceneWorldManager.Instance.TransferToWorld(WorldManager.Instance.worldIndex + 1);
    }

    /// <summary>
    /// Return to normal world path
    /// </summary>
    public void ResumeWorld()
    {
        if (!active) return;
        active = false;
        foreach (Player player in PlayerManager.Instance.players)
        {
            player.stats.bufferedUnits.Clear();

            List<Unit> units = EntityManager.Instance.units.FindAll(u => u.owner == player);

            foreach (Unit u in units)
            {
                player.stats.bufferedUnits.Add(u._name);
            }
        }
        foreach (Unit u in EntityManager.Instance.units)
            u.OnHit(9999);
        EntityManager.Instance.ClearAggro();

        HudManager.Instance.DisablePointers();
        HudManager.Instance.AdvanceLevel("hide", 0);
        AudioManager.Instance.bgAudio.Stop();
        SceneWorldManager.Instance.TransferToWorld(PlayerManager.Instance.currentWorldIndex + 1);
    }

    /// <summary>
    /// Transfer player to Slaughterhouse
    /// </summary>
    public void RedRoom()
    {
        if (!active) return;
        active = false;
        // Units can't help you.
        foreach (Player player in PlayerManager.Instance.players)
            player.stats.bufferedUnits.Clear();

        foreach (Unit u in EntityManager.Instance.units)
            u.OnHit(9999);
        EntityManager.Instance.ClearAggro();

        HudManager.Instance.DisablePointers();
        HudManager.Instance.AdvanceLevel("hide", 0);
        AudioManager.Instance.bgAudio.Stop();
        SceneWorldManager.Instance.TransferToRedRoom();
    }

    public void DelayRedRoom(float delay)
    {
        HudManager.Instance.ToggleBlackScreen(true);
        DisableInput();
        Invoke(nameof(RedRoom), delay);
    }

    public void DisableInput()
    {
        foreach (Player player in PlayerManager.Instance.players)
        {
            player.movement.enabled = false;
            player.hand.enabled = false;
        }
    }
}

