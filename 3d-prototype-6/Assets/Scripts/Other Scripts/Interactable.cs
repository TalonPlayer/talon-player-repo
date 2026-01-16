using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    private Outline outline;
    public string message;
    [SerializeField] private UnityEvent<Player> onInteraction;

    void Awake()
    {
        outline = GetComponent<Outline>();
    }
    public void ToggleOutline(bool active)
    {
        outline.SetOutlineActive(active);
    }

    public void OnInteract(Player player)
    {
        onInteraction?.Invoke(player);
        ToggleOutline(false);
    }

    void OnDisable()
    {
        ToggleOutline(false);
    }

    void OnDestroy()
    {
        ToggleOutline(false);
    }
}
