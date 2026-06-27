using System;
using System.Collections.Generic;

[Serializable]
public class CurrencyInfo
{
    public string name;
    public string icon;
    public string color;
}

[Serializable]
public class GachaGuarantee
{
    public int interval;
    public string minimumRarity;
}

[Serializable]
public class GachaCost
{
    public int coin;
    public int token;
    public int jade;
}

[Serializable]
public class GachaPool
{
    public string name;
    public GachaCost cost;
    public Dictionary<string, int> rarityWeights;
    public GachaGuarantee guarantee;
}

[Serializable]
public class ShopData
{
    public Dictionary<string, CurrencyInfo> currencies;
    public Dictionary<string, GachaPool> gachaPools;
    public Dictionary<string, string> rarityColors;
}
