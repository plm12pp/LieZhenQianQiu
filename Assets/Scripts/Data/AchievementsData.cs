using System;
using System.Collections.Generic;

[Serializable]
public class AchievementCondition
{
    public string type;
    public int value;
    public string unitId;
}

[Serializable]
public class AchievementRewards
{
    public int coin;
    public int token;
    public int jade;
    public Dictionary<string, int> fragments;
    public string item;
}

[Serializable]
public class AchievementData
{
    public string id;
    public string name;
    public string description;
    public AchievementCondition condition;
    public AchievementRewards rewards;
    public string icon;
}

[Serializable]
public class AchievementsData
{
    public List<AchievementData> achievements;
}
