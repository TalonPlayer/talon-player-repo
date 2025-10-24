using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementHandler : MonoBehaviour
{
    public List<AchievementObj> achievements;
    public List<Button> unlockables;
    public AchievementHolder prefab;
    public Transform scrollContent;
    void Start()
    {
        // InitializeAchievements();
        // CreateAchievementTabs();
        // CreateUnlockedAchievements();
        // Unlockables();
    }

    /// <summary>
    /// Create new achievements if they don't exist
    /// </summary>
    public void InitializeAchievements()
    {
        foreach (AchievementObj obj in achievements)
        {
            if (GlobalSaveSystem.HasAchievement(obj.id)) continue;
            if (GlobalSaveSystem.GetAchievementProgress(obj.id) != null) continue;

            GlobalSaveSystem.Data.achievementsProgress.Add(new AchievementProgress(obj));
            GlobalSaveSystem.Save();
        }
    }

    public void CreateAchievementTabs()
    {
        foreach (AchievementProgress ach in GlobalSaveSystem.Data.achievementsProgress)
        {
            AchievementHolder holder = Instantiate(prefab, scrollContent);

            holder.Init(
                ach.name,
                ach.desc,
                ach.picture,
                false,
                ach.unlockableName);
            holder.Setbar(ach.current, ach.target);
        }
    }

    public void CreateUnlockedAchievements()
    {
        foreach (string ach in GlobalSaveSystem.Data.achievementsUnlocked)
        {
            AchievementHolder holder = Instantiate(prefab, scrollContent);
            AchievementObj obj = achievements.Find(x => x.id == ach);
            holder.Init(
                obj.name,
                obj.desc,
                obj.pic,
                true,
                obj.unlockableName);

            holder.Setbar();
        }
    }

    public void Unlockables()
    {
        foreach (Button u in unlockables)
        {
            if (GlobalSaveSystem.IsUnlocked(u.name))
            {
                u.interactable = true;
            }
        }
    }
}
