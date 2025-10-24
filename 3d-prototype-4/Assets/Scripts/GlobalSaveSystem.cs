using System;
using System.IO;
using UnityEngine;

public static class GlobalSaveSystem
{
    private const string FileName = "global_save.json";
    private static string PathFull => Path.Combine(Application.persistentDataPath, FileName);
    public static bool trackAchivement = false;
    public static GlobalSaveData Data { get; private set; }

    public static void LoadOrCreate()
    {
        try
        {
            if (File.Exists(PathFull))
            {
                string json = File.ReadAllText(PathFull);
                Data = JsonUtility.FromJson<GlobalSaveData>(json);

                if (Data == null) Data = new GlobalSaveData();
                if (Data.settings == null) Data.settings = new GlobalSettings();
            }
            else
            {
                Data = new GlobalSaveData();
                Save();
            }

            Data.lastLaunchUtcTicks = DateTime.UtcNow.Ticks;
        }
        catch (Exception e)
        {
            Debug.LogError($"GlobalSaveSystem.LoadOrCreate error: {e}");
            Data = new GlobalSaveData();
        }
    }

    public static void Save()
    {
        if (Data == null) { Debug.LogWarning("GlobalSaveSystem.Save called with null Data"); return; }

        try
        {
            string json = JsonUtility.ToJson(Data, prettyPrint: false);
            string tmp = PathFull + ".tmp";
            File.WriteAllText(tmp, json);
            if (File.Exists(PathFull)) File.Replace(tmp, PathFull, null);
            else File.Move(tmp, PathFull);
        }
        catch (Exception e)
        {
            Debug.LogError($"GlobalSaveSystem.Save error: {e}");
        }
    }


    public static void SetMasterVolume(float v)
    {
        Data.settings.masterVolume = Mathf.Clamp01(v);
        Save();
    }

    // Unlockables
    public static bool IsUnlocked(string id) => Data.unlockedIds.Contains(id);

    public static void Unlock(string id)
    {
        if (!Data.unlockedIds.Contains(id))
        {
            Data.unlockedIds.Add(id);
            Save();
        }
    }

    /// <summary>
    /// Checks if the achievement exists
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool HasAchievement(string id)
    {
        return false;
        if (!trackAchivement) return false;
        return Data.achievementsUnlocked.Contains(id);
    }
    /// <summary>
    /// Finds Achievement Data given the id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static AchievementProgress GetAchievementProgress(string id)
    {
        return Data.achievementsProgress.Find(x => x.id == id);
    }
    /// <summary>
    /// Unlock the achievement
    /// </summary>
    /// <param name="id"></param>
    public static void UnlockAchievement(string id, string name, string unlockable = null)
    {
        
        if (!HasAchievement(id))
        {
            HudManager.Instance.DisplayAchievement(name, unlockable);
            AddAchievementProgress("100%", 1);
            if (unlockable != null) Unlock(unlockable);
            Data.achievementsUnlocked.Add(id);
            Data.achievementsProgress.RemoveAll(p => p.id == id);
            Save();
        }
    }

    /// <summary>
    /// Change achievement progress
    /// </summary>
    /// <param name="id"></param>
    /// <param name="add"></param>
    public static void AddAchievementProgress(string id, int add)
    {
        return;
        if (!trackAchivement) return;
        if (HasAchievement(id)) return;
        var p = Data.achievementsProgress.Find(x => x.id == id);
        if (p == null) return;
        p.current = Mathf.Clamp(p.current + add, 0, p.target);
        if (p.current >= p.target)
        {
            UnlockAchievement(id, p.name, p.unlockableName);
        }
        else
        {
            Save();
        }
    }

    /// <summary>
    /// Reset the achievement progress back to 0
    /// </summary>
    /// <param name="id"></param>
    public static void ResetProgress(string id)
    {
        return;
        if (!trackAchivement) return;
        if (HasAchievement(id)) return;
        var p = Data.achievementsProgress.Find(x => x.id == id);
        if (p == null)
        {
            return;
        }
        p.current = 0;
        Save();
    }



}
