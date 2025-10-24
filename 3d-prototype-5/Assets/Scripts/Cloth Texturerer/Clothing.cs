using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clothing : MonoBehaviour
{
    public SkinnedMeshRenderer clothing;
    public Color primaryColor;
    public Color secondaryColor;
    public bool hasPrint;
    public Texture printTexture;
    public int printIndex;
    public List<int> primaryIndex;
    public List<int> secondaryIndex;
    public void SetClothing()
    {
        clothing.materials[printIndex].mainTexture = printTexture;

        foreach (int index in primaryIndex)
            clothing.materials[index].color = primaryColor;
        foreach (int index in secondaryIndex)
            clothing.materials[index].color = secondaryColor;
    }
    public void SetPrimary(Color color)
    {
        primaryColor = color;
    }

    public void SetSecondary(Color color)
    {
        secondaryColor = color;
    }

    public void SetPrint(Texture texture)
    {
        printTexture = texture;
    }
}
