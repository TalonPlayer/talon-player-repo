using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelZone : MonoBehaviour
{
    public Transform playerEnterPoint;
    public Transform door;
    public bool isPortal = false;
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

    public void Spawn()
    {
        HudManager.Instance.ToggleBlackScreen(false);
        PlayerManager.Instance.TeleportPlayer(playerEnterPoint);
    }

    public void DelaySpawn(float delay)
    {
        PlayerManager.Instance.player.movement.enabled = false;
        PlayerManager.Instance.player.hand.enabled = false;
        Invoke(nameof(Spawn), delay);
    }

    public void DelayTransfer(float delay)
    {
        HudManager.Instance.ToggleBlackScreen(true);
        PlayerManager.Instance.player.movement.enabled = false;
        PlayerManager.Instance.player.hand.enabled = false;
        Invoke(nameof(TransferToNewWorld), delay);
    }

    public void TransferToNewWorld()
    {
        HudManager.Instance.DisablePointers();
        HudManager.Instance.AdvanceLevel("hide", 0);
        AudioManager.Instance.bgAudio.Stop();
        SceneWorldManager.Instance.TransferToWorld(WorldManager.Instance.worldIndex + 1);
    }
}
