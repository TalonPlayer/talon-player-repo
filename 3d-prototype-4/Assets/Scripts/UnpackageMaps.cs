using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnpackageMaps : MonoBehaviour
{
    public GameObject mapObj;
    void Start()
    {
        Transform[] children = mapObj.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child != mapObj.transform && child.localScale != Vector3.one)
            {
                child.localScale = Vector3.one;
            }
        }

        MeshRenderer[] meshes = mapObj.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer mesh in meshes)
        {
            mesh.enabled = true;
        }
    }

}
