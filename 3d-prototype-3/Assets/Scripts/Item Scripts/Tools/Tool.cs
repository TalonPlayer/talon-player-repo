using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Tool : Item
{
    void Start()
    {
        isStackable = false;
    }

    public abstract override void PrimaryUse();
    public abstract override void SecondaryUse();
    public abstract override bool Validation();
}
