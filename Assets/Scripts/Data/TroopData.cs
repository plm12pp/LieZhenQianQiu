using System;
using System.Collections.Generic;

[Serializable]
public class TroopData
{
    public string id;
    public string name;
    public string dynasty;
    public string role;
    public string rarity;
    public int size;
    public int hp;
    public int atk;
    public float range;
    public float speed;
    public int armor;
    public List<string> counterBonus;
    public float counterMultiplier;
    public string skillType;
    public string skillName;
    public string skill;
    public string skillDescription;
    public int skillCooldown;
    public string skillEffect;
}

[Serializable]
public class CounterRule
{
    public List<string> targets;
    public float multiplier;
}

[Serializable]
public class TroopsData
{
    public List<TroopData> troops;
    public Dictionary<string, CounterRule> counterRules;
    public Dictionary<string, float> starBonuses;
    public Dictionary<string, StarUpgradeCost> starUpgradeCost;
    public Dictionary<string, int> rarityWeight;
}

[Serializable]
public class StarUpgradeCost
{
    public int units;
    public int fragments;
}
