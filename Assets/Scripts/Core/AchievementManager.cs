using UnityEngine;
using System.Collections.Generic;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CheckAchievement(string type, int value = 0, string unitId = "")
    {
        foreach (var achievement in GameManager.instance.achievementsData.achievements)
        {
            if (achievement.condition.type != type)
                continue;

            if (!string.IsNullOrEmpty(unitId) && achievement.condition.unitId != unitId)
                continue;

            int currentValue = GetCurrentProgress(achievement.id);
            int targetValue = achievement.condition.value;

            if (type == "level_clear")
            {
                if (value >= targetValue && currentValue < targetValue)
                {
                    currentValue = value;
                    SetProgress(achievement.id, currentValue);
                    CheckAndAward(achievement, currentValue);
                }
            }
            else if (type == "win_streak")
            {
                if (value > currentValue)
                {
                    currentValue = value;
                    SetProgress(achievement.id, currentValue);
                    CheckAndAward(achievement, currentValue);
                }
            }
            else if (type == "unit_kills")
            {
                currentValue++;
                SetProgress(achievement.id, currentValue);
                CheckAndAward(achievement, currentValue);
            }
            else if (type == "max_star")
            {
                if (currentValue < targetValue)
                {
                    currentValue = targetValue;
                    SetProgress(achievement.id, currentValue);
                    CheckAndAward(achievement, currentValue);
                }
            }
            else if (type == "collect_count")
            {
                if (value > currentValue)
                {
                    currentValue = value;
                    SetProgress(achievement.id, currentValue);
                    CheckAndAward(achievement, currentValue);
                }
            }
        }
    }

    int GetCurrentProgress(string achievementId)
    {
        if (GameManager.instance.currentSave.achievements.TryGetValue(achievementId, out int value))
        {
            return value;
        }
        return 0;
    }

    void SetProgress(string achievementId, int value)
    {
        GameManager.instance.currentSave.achievements[achievementId] = value;
        GameManager.instance.SaveGame();
    }

    void CheckAndAward(AchievementData achievement, int currentValue)
    {
        if (currentValue >= achievement.condition.value)
        {
            AwardAchievement(achievement);
        }
    }

    void AwardAchievement(AchievementData achievement)
    {
        if (achievement.rewards.coin > 0)
            CurrencyManager.instance.AddCurrency("coin", achievement.rewards.coin);
        
        if (achievement.rewards.token > 0)
            CurrencyManager.instance.AddCurrency("token", achievement.rewards.token);
        
        if (achievement.rewards.jade > 0)
            CurrencyManager.instance.AddCurrency("jade", achievement.rewards.jade);
        
        if (achievement.rewards.fragments != null)
        {
            foreach (var kvp in achievement.rewards.fragments)
            {
                AddFragments(kvp.Key, kvp.Value);
            }
        }
        
        UIManager.instance.ShowMessage($"成就解锁: {achievement.name}");
    }

    void AddFragments(string troopId, int amount)
    {
        InventoryItem item;
        if (GameManager.instance.currentSave.inventory.TryGetValue(troopId, out item))
        {
            item.fragments += amount;
        }
        else
        {
            GameManager.instance.currentSave.inventory[troopId] = new InventoryItem
            {
                count = 0,
                star = 1,
                level = 1,
                fragments = amount
            };
        }
        GameManager.instance.SaveGame();
    }

    public void OnLevelClear(int level)
    {
        CheckAchievement("level_clear", level);
    }

    public void OnWinStreak(int streak)
    {
        CheckAchievement("win_streak", streak);
    }

    public void OnUnitKill(string unitId)
    {
        CheckAchievement("unit_kills", 0, unitId);
    }

    public void OnMaxStar()
    {
        CheckAchievement("max_star", 1);
    }

    public void OnCollectCount(int count)
    {
        CheckAchievement("collect_count", count);
    }
}
