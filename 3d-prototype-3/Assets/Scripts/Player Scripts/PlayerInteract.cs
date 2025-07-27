using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float reach = 3f;
    public LayerMask buttonLayer;
    private Interactable current;
    private Player player;
    void Start()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        CheckInteraction();
        if (current != null)
        {
            switch (current.type)
            {
                case Interactable.KeyCodeType.KeyCode:
                    if (Input.GetKeyDown(current.keyCode))
                        current.Interact();
                    break;
                case Interactable.KeyCodeType.MouseClick:
                if (Input.GetMouseButtonDown(current.inputKey))
                        current.Interact();
                    break;
            }

        }
    }
    
    void CheckInteraction(){
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    
        if (Physics.Raycast(ray, out hit, reach, buttonLayer)){
            RaycastDetection(hit);
        }
        else{
            DisableCurrentInteractable();
        }
    }

    void RaycastDetection(RaycastHit hit){
        if (hit.collider.tag == "Interactable"){
            if (current && hit.collider.gameObject == current.gameObject) return;
            else{
                Interactable newInteractable = hit.collider.GetComponent<Interactable>();
                if (!newInteractable) return;
                // if (current && newInteractable != current)
                    // current.DisableOutline();
                if (newInteractable.enabled) 
                    SetNewCurrentInteractable(newInteractable);
                else 
                    DisableCurrentInteractable();
            }
        }
        else{
            DisableCurrentInteractable();
        }
    }

    void SetNewCurrentInteractable(Interactable newInteractable)
    {
        current = newInteractable;
        HudManager.Instance.SetInteractText(true, current.message);
        
        // current.EnableOutline();
    }

    void DisableCurrentInteractable(){
        if (current){
            HudManager.Instance.SetInteractText(false, "");

            // current.DisableOutline();
            current = null;
        }
    }
}
