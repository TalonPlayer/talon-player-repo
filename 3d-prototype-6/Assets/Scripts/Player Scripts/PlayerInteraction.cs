using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float reach = 2.5f;
    private Player main;
    private Transform _cam;
    public Interactable _selected;
    void Awake()
    {
        main = GetComponent<Player>();
    }

    void Start()
    {
        _cam = main.movement.playerCam;
    }

    void Update()
    {
        CheckInteraction();
    }

    public void OnInteract()
    {
        if (_selected)
        {
            _selected.OnInteract(main);
            Unselect();
        }
    }
    private void CheckInteraction()
    {
        if (Physics.Raycast(_cam.position, _cam.forward, out RaycastHit hit, reach, Layer.Interactable, QueryTriggerInteraction.Ignore))
        {
            if (_selected)
            {
                if (_selected.gameObject != hit.collider.gameObject)
                {
                    Unselect();
                    _selected = hit.collider.GetComponent<Interactable>();
                    Select();
                }
            }
            else
            {
                _selected = hit.collider.GetComponent<Interactable>();
                Select();
            }
            return;
        }


        if (_selected) Unselect();
        else
        {
            _selected = null;
            HUDManager.InteractText("");
        }
    }
    private void Select()
    {
        _selected.ToggleOutline(true);
        HUDManager.InteractText(_selected.message);
    }
    private void Unselect()
    {
        _selected.ToggleOutline(false);
        _selected = null;
        HUDManager.InteractText("");
    }
}

