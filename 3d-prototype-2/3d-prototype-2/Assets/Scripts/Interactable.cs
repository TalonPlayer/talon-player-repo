using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    private MeshRenderer mr;
    public SkinnedMeshRenderer smr;
    public string message;
    public string letter;
    public UnityEvent onInteraction;
    public Material highlightMat;
    public KeyCode keyCode;
    public bool isSkinned = false;
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        
        DisableOutline();
    }

    public void Interact(){
        onInteraction.Invoke();
    }

    public void DisableOutline(){

        if (isSkinned){
            Material[] mats = smr.materials;
            if (mats.Length > 1) {
                List<Material> updatedMats = new List<Material>(mats);
                updatedMats.RemoveAt(1);
                smr.materials = updatedMats.ToArray();
            }
        }
        else{
            Material[] mats = mr.materials;

            if (mats.Length > 1) {
                List<Material> updatedMats = new List<Material>(mats);
                updatedMats.RemoveAt(1);
                mr.materials = updatedMats.ToArray();
            }
        }
        
    }

    public void EnableOutline(){
        if (isSkinned){
            Material[] mats = smr.materials;
            if (mats.Length == 1) {
                Material[] updatedMats = new Material[2];
                updatedMats[0] = mats[0];
                updatedMats[1] = highlightMat;
                smr.materials = updatedMats;
            }
        }
        else{
            Material[] mats = mr.materials;
            if (mats.Length == 1) {
                Material[] updatedMats = new Material[2];
                updatedMats[0] = mats[0];
                updatedMats[1] = highlightMat;
                mr.materials = updatedMats;
            }
        }
    }

}
