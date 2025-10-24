using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GlobalSettings
{
    public float masterVolume = 0.8f;
    public float musicVolume = 0.8f;
    public float sfxVolume = 0.8f;
}

[Serializable]
public class AchievementProgress
{
    public string name;
    public string id;
    public string desc;
    public Sprite picture;
    public int current;
    public int target;
    public string unlockableName;
    public AchievementProgress() { }
    public AchievementProgress(AchievementObj obj)
    {
        name = obj._name;
        id = obj.id;
        desc = obj.desc;
        picture = obj.pic;
        target = obj.max;
        unlockableName = obj.unlockableName;
    }
}

[Serializable]
public class GlobalSaveData
{
    public int version = 1;
    public GlobalSettings settings = new GlobalSettings();

    public List<string> achievementsUnlocked = new List<string>();
    public List<AchievementProgress> achievementsProgress = new List<AchievementProgress>();

    public List<string> unlockedIds = new List<string>();

    public long firstLaunchUtcTicks;
    public long lastLaunchUtcTicks;

    public GlobalSaveData()
    {
        var now = DateTime.UtcNow.Ticks;
        firstLaunchUtcTicks = now;
        lastLaunchUtcTicks = now;
    }
}
