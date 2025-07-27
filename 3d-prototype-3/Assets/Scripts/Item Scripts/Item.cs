using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string itemName;
    public string itemId;
    public string primaryText;
    public string secondaryText;
    public Sprite itemSprite;
    public GameObject itemModel;
    public GameObject itemIndicator;
    public bool hasIndicator;
    public bool isStackable;
    public float delay;
    public float cooldown;
    public Animator animator;
    [HideInInspector] public Vector3 indicatorPos;
    [HideInInspector] public bool showIndicator;
    public abstract void PrimaryUse();
    public abstract void SecondaryUse();
    public abstract bool Validation();
}
