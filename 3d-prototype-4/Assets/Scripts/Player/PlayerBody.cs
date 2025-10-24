using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBody : MonoBehaviour
{
    public Animator animator;
    public GameObject currentCostume;
    public Transform body;
    public Transform head;
    public Renderer bodyRenderer;
    public SpriteRenderer circle;
    public TrailRenderer trail;
    private Player player;
    void Awake()
    {
        player = GetComponent<Player>();
    }
    public void Play(string para, bool val)
    {
        animator.SetBool(para, val);
    }

    public void Play(string para, float val)
    {
        animator.SetFloat(para, val);
    }

    public void Play(string para)
    {
        animator.SetTrigger(para);
    }

    public void RagDoll(bool ragdoll)
    {
        animator.enabled = !ragdoll;
        Rigidbody[] rigidbodies = body.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody r in rigidbodies)
        {
            r.isKinematic = !ragdoll;
        }
    }

    public void SetCostume()
    {
        currentCostume = PlayerCostumeManager.Instance.ChangeCostume(player.info.costumeIndex, head, currentCostume);
    }

    public void ChangeColor()
    {
        Color color = PlayerManager.Instance.GetColor(player.colorCode);
        circle.color = color;
        trail.material.color = color;
        bodyRenderer.material.color = color;

        // Modify the gradient on the TrailRenderer
        Gradient gradient = trail.colorGradient;
        GradientColorKey[] colorKeys = gradient.colorKeys;
        GradientAlphaKey[] alphaKeys = gradient.alphaKeys;

        // Update the first color key (start color)
        colorKeys[0].color = color;
        // Change last color key to white
        colorKeys[colorKeys.Length - 1].color = Color.white;

        // Change last alpha key to 0
        alphaKeys[alphaKeys.Length - 1].alpha = 0f;
        // Create new gradient with updated color key
        Gradient newGradient = new Gradient();
        newGradient.SetKeys(colorKeys, alphaKeys);
        trail.colorGradient = newGradient;
    }
}
