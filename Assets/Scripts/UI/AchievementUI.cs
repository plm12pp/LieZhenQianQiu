using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AchievementUI : MonoBehaviour
{
    public GameObject achievementItemPrefab;
    public Transform achievementListParent;
    
    void Start()
    {
        RefreshAchievements();
    }

    void RefreshAchievements()
    {
        foreach (Transform child in achievementListParent)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var achievement in GameManager.instance.achievementsData.achievements)
        {
            CreateAchievementItem(achievement);
        }
    }

    void CreateAchievementItem(AchievementData achievement)
    {
        GameObject itemObj = Instantiate(achievementItemPrefab, achievementListParent);
        Text[] texts = itemObj.GetComponentsInChildren<Text>();
        
        bool isCompleted = IsAchievementCompleted(achievement);
        
        if (texts.Length > 0)
            texts[0].text = achievement.name;
        if (texts.Length > 1)
            texts[1].text = achievement.description;
        if (texts.Length > 2)
            texts[2].text = isCompleted ? "已完成" : "未完成";
        
        Image[] images = itemObj.GetComponentsInChildren<Image>();
        if (images.Length > 1)
        {
            images[1].color = isCompleted ? Color.green : Color.gray;
        }
    }

    bool IsAchievementCompleted(AchievementData achievement)
    {
        int currentValue = 0;
        
        if (GameManager.instance.currentSave.achievements.TryGetValue(achievement.id, out currentValue))
        {
            return currentValue >= achievement.condition.value;
        }
        
        return false;
    }

    public void OnBackButton()
    {
        UIManager.instance.OnBackButton();
    }
}
