using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverPoint : MonoBehaviour
{
    public Transform peekPoint;
    public Transform peekLocation;
    public CoverType coverType;
    public CoverDirection coverDirection;

    public Unit occupant;
    public Unit potentialOccupant;
    public Quaternion Rotation{ get {return transform.rotation; }}
    public bool isOccupied; // Is the occupant here?
    public bool isTargeted; // Is the occupant moving towards the cover?
    public void TargetCover(Unit unit)
    {
        potentialOccupant = unit;
        isTargeted = true;
    }
    public void OccupyCover(Unit unit)
    {
        potentialOccupant = unit;
        isOccupied = true;
    }

    public void RemoveOccupant()
    {
        isOccupied = false;
        isTargeted = false;
        potentialOccupant = null;
        occupant = null;
    }
}

public enum CoverType
{
    Stand,
    Crouch,
    Prone
}

public enum CoverDirection
{
    Left,
    Right
}



