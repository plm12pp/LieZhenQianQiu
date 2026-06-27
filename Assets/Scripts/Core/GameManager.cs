using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TroopsData troopsData;
    public TroopsData allTroops;  // 为GachaUI等提供访问
    public LevelData levelData;
    public ShopData shopData;
    public AchievementsData achievementsData;
    
    public SaveData currentSave;
    
    private string savePath;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
            InitializeSave();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadData()
    {
        TextAsset troopsJson = Resources.Load<TextAsset>("Data/Troops");
        troopsData = JsonUtility.FromJson<TroopsData>(troopsJson.text);
        allTroops = troopsData;  // 同一引用
        
        TextAsset levelsJson = Resources.Load<TextAsset>("Data/Levels");
        levelData = JsonUtility.FromJson<LevelData>(levelsJson.text);
        
        TextAsset shopJson = Resources.Load<TextAsset>("Data/ShopPools");
        shopData = JsonUtility.FromJson<ShopData>(shopJson.text);
        
        TextAsset achievementsJson = Resources.Load<TextAsset>("Data/Achievements");
        achievementsData = JsonUtility.FromJson<AchievementsData>(achievementsJson.text);
    }

    void InitializeSave()
    {
        savePath = Path.Combine(Application.persistentDataPath, "save.json");
        
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentSave = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            currentSave = CreateDefaultSave();
            SaveGame();
        }
    }

    SaveData CreateDefaultSave()
    {
        SaveData save = new SaveData();
        save.playerName = "玩家";
        save.currentDisplayLevel = 1;
        save.difficultyIndex = 0;
        save.totalCleared = 0;
        save.winStreak = 0;
        save.perfectWinStreak = 0;
        
        save.currencies = new CurrencyData
        {
            coin = 500,
            token = 10,
            jade = 0
        };
        
        save.inventory = new Dictionary<string, InventoryItem>();
        AddInitialTroops(save.inventory);
        
        save.achievements = new Dictionary<string, int>();
        foreach (var ach in achievementsData.achievements)
        {
            save.achievements[ach.id] = 0;
        }
        
        save.settings = new GameSettings
        {
            music = true,
            sfx = true,
            lowPerformanceMode = false,
            musicVolume = 0.5f,
            sfxVolume = 0.7f
        };
        
        return save;
    }

    void AddInitialTroops(Dictionary<string, InventoryItem> inventory)
    {
        inventory["troop_knife"] = new InventoryItem { count = 3, star = 1, level = 1, fragments = 0 };
        inventory["troop_spear"] = new InventoryItem { count = 2, star = 1, level = 1, fragments = 0 };
        inventory["troop_archer"] = new InventoryItem { count = 2, star = 1, level = 1, fragments = 0 };
        inventory["troop_shield"] = new InventoryItem { count = 1, star = 1, level = 1, fragments = 0 };
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(currentSave, true);
        File.WriteAllText(savePath, json);
    }

    public void ClearSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        currentSave = CreateDefaultSave();
        SaveGame();
    }

    public TroopData GetTroopData(string troopId)
    {
        return troopsData.troops.Find(t => t.id == troopId);
    }

    public float GetStarBonus(int star)
    {
        if (troopsData.starBonuses.TryGetValue(star.ToString(), out float bonus))
        {
            return bonus;
        }
        return 1.0f;
    }

    public StarUpgradeCost GetStarUpgradeCost(int currentStar)
    {
        string key = $"{currentStar}_to_{currentStar + 1}";
        if (troopsData.starUpgradeCost.TryGetValue(key, out StarUpgradeCost cost))
        {
            return cost;
        }
        return null;
    }

    public int GetEnemyCountForLevel(int level)
    {
        foreach (var kvp in levelData.enemyCountByLevel)
        {
            string range = kvp.Key;
            if (range.Contains("-"))
            {
                string[] parts = range.Split('-');
                int min = int.Parse(parts[0]);
                int max = int.Parse(parts[1]);
                if (level >= min && level <= max)
                {
                    return kvp.Value;
                }
            }
            else if (range.EndsWith("+"))
            {
                int min = int.Parse(range.Replace("+", ""));
                if (level >= min)
                {
                    return kvp.Value;
                }
            }
        }
        return levelData.levelSettings.baseEnemyCount;
    }

    public float GetDifficultyMultiplier(int index)
    {
        return Mathf.Pow(levelData.levelSettings.difficultyMultiplier, index);
    }

    public int CalculateNextLevel(int currentLevel)
    {
        if (currentLevel % levelData.levelSettings.resetInterval == 0)
        {
            return Mathf.RoundToInt(currentLevel * levelData.levelSettings.resetFactor);
        }
        return currentLevel + 1;
    }

    public int GetMaxLives()
    {
        return levelData.levelSettings.maxLives;
    }
}
