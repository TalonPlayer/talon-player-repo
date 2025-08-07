using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] private Renderer obj;
    [SerializeField] private Animator animator;

    public void ChangeColor(string colorCode)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(colorCode, out color))
        {
            obj.material.color = color;
        }
        else
        {
            Debug.LogWarning("Invalid Hex Code");
        }
    }

    public void SetNormal()
    {
        animator.SetBool("Normal", true);
    }
    public void SetRandom()
    {
        animator.SetFloat("RandomFloat", Random.Range(0f, .75f));
        animator.SetInteger("Random", Random.Range(0, 3));
    }
}   
