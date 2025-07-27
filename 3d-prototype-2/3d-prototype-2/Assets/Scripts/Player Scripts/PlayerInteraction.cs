using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float enemyReach = 10f;
    public float buttonReach = 3f;
    public bool canFlyKick;
    public LayerMask buttonLayer;
    public LayerMask enemyLayer;
    Interactable currentInteractable;
    private Player player;
    public void SetPlayer(Player p) => player = p;
    void Update()
    {
        CheckInteraction();
        if (currentInteractable != null){
            if (Input.GetKeyDown(currentInteractable.keyCode)){
                currentInteractable.Interact();
            }
        }
    }

    public void SetReach(){

    }

    void CheckInteraction(){
        RaycastHit hit;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
    
        if (Physics.Raycast(ray, out hit, buttonReach, buttonLayer)){
            RaycastDetection(hit, "Interactable");
        }
        else if (Physics.Raycast(ray, out hit, enemyReach, enemyLayer) && player.movement.canFlyKick){
            RaycastDetection(hit, "Enemy");
        }
        else{
            DisableCurrentInteractable();
        }
    }

    void RaycastDetection(RaycastHit hit, string tag){
        if (hit.collider.tag == tag){
            if (currentInteractable && hit.collider.gameObject == currentInteractable.gameObject) return;
            else{
                Interactable newInteractable = hit.collider.GetComponent<Interactable>();
                if (!newInteractable) return;
                if (currentInteractable && newInteractable != currentInteractable)
                    currentInteractable.DisableOutline();
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

    void SetNewCurrentInteractable(Interactable newInteractable){
        HUDController.Instance.OpenIntButton(newInteractable);
        currentInteractable = newInteractable;
        currentInteractable.EnableOutline();
    }

    void DisableCurrentInteractable(){
        if (currentInteractable){
            HUDController.Instance.CloseIntButton();

            currentInteractable.DisableOutline();
            currentInteractable = null;
        }
    }
}
