using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public string playerName;
    public int currentDisplayLevel;
    public int difficultyIndex;
    public int totalCleared;
    public CurrencyData currencies;
    public Dictionary<string, InventoryItem> inventory;
    public Dictionary<string, int> achievements;
    public GameSettings settings;
    public int winStreak;
    public int perfectWinStreak;
}

[Serializable]
public class CurrencyData
{
    public int coin;
    public int token;
    public int jade;
}

[Serializable]
public class InventoryItem
{
    public int count;
    public int star;
    public int level;
    public int fragments;
}

[Serializable]
public class GameSettings
{
    public bool music;
    public bool sfx;
    public bool lowPerformanceMode;
    public float musicVolume;
    public float sfxVolume;
}
