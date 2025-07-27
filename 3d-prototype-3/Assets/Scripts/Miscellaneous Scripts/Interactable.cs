using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public MeshRenderer mr;
    public SkinnedMeshRenderer smr;
    public string message;
    public UnityEvent onInteraction;
    public Material highlightMat;
    public KeyCodeType type;
    public KeyCode keyCode;
    public int inputKey;
    void Start()
    {
        // DisableOutline();
    }

    public void Interact()
    {
        onInteraction.Invoke();
    }
    public enum KeyCodeType
    {
        MouseClick,
        KeyCode
    }
    /*
    public void DisableOutline()
    {
        if (smr)
        {
            Material[] mats = smr.materials;
            if (mats.Length > 1)
            {
                List<Material> updatedMats = new List<Material>(mats);
                updatedMats.RemoveAt(1);
                smr.materials = updatedMats.ToArray();
            }
        }
        else if (mr)
        {
            Material[] mats = mr.materials;

            if (mats.Length > 1)
            {
                List<Material> updatedMats = new List<Material>(mats);
                updatedMats.RemoveAt(1);
                mr.materials = updatedMats.ToArray();
            }
        }
    }
    */
    /*
    public void EnableOutline(){
        if (smr){
            Material[] mats = smr.materials;
            if (mats.Length == 1) {
                Material[] updatedMats = new Material[2];
                updatedMats[0] = mats[0];
                updatedMats[1] = highlightMat;
                smr.materials = updatedMats;
            }
        }
        else if (mr){
            Material[] mats = mr.materials;
            if (mats.Length == 1) {
                Material[] updatedMats = new Material[2];
                updatedMats[0] = mats[0];
                updatedMats[1] = highlightMat;
                mr.materials = updatedMats;
            }
        }
    }
    */
}
