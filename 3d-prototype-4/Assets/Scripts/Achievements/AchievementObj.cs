using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Achievement", menuName = "New Achievement", order = 1)]
public class AchievementObj : ScriptableObject
{
    public string _name;
    public string id;
    public string desc;
    public Sprite pic;
    public int max;
    public string unlockableName;
}
