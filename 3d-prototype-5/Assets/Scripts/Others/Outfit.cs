using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Outfit", menuName = "ScriptObjs/Outfit", order = 1)]
public class Outfit : ScriptableObject
{ 
    public bool presetExclusive;
    public string _name = "No Name";
    public string ID = "000";
    public GameObject outfit;
}
