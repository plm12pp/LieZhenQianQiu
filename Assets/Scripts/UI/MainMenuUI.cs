using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Text levelText;
    public Text totalClearedText;
    public Text winStreakText;
    
    void Start()
    {
        UpdateStats();
    }

    void UpdateStats()
    {
        if (levelText != null)
            levelText.text = $"当前关卡: 第 {GameManager.instance.currentSave.currentDisplayLevel} 关";
        
        if (totalClearedText != null)
            totalClearedText.text = $"累计通关: {GameManager.instance.currentSave.totalCleared} 关";
        
        if (winStreakText != null)
            winStreakText.text = $"连胜: {GameManager.instance.currentSave.winStreak} 场";
    }

    public void OnBattleButton()
    {
        UIManager.instance.OnStartBattleButton();
    }

    public void OnShopButton()
    {
        UIManager.instance.OnShopButton();
    }

    public void OnBagButton()
    {
        UIManager.instance.OnBagButton();
    }

    public void OnAchievementButton()
    {
        UIManager.instance.OnAchievementButton();
    }

    public void OnSettingsButton()
    {
        UIManager.instance.OnSettingsButton();
    }
}
