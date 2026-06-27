using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    public GameObject gachaButtonPrefab;
    public Transform gachaButtonParent;
    public GameObject gachaResultPopup;
    public Text resultTroopName;
    public Text resultRarity;
    public Image resultRarityBackground;
    
    private Dictionary<string, int> gachaCounts;
    
    void Start()
    {
        gachaCounts = new Dictionary<string, int>
        {
            { "normal", 0 },
            { "elite", 0 },
            { "legendary", 0 }
        };
        
        InitializeGachaButtons();
    }

    void InitializeGachaButtons()
    {
        foreach (var pool in GameManager.instance.shopData.gachaPools)
        {
            GameObject buttonObj = Instantiate(gachaButtonPrefab, gachaButtonParent);
            Button button = buttonObj.GetComponent<Button>();
            Text[] texts = buttonObj.GetComponentsInChildren<Text>();
            
            if (texts.Length > 0)
                texts[0].text = pool.Value.name;
            
            string poolKey = pool.Key;
            button.onClick.AddListener(() => OnGachaButtonClick(poolKey));
        }
    }

    void OnGachaButtonClick(string poolKey)
    {
        GachaPool pool = GameManager.instance.shopData.gachaPools[poolKey];
        
        if (!CanAfford(pool))
        {
            UIManager.instance.ShowMessage("货币不足！");
            return;
        }
        
        SpendCurrency(pool);
        
        gachaCounts[poolKey]++;
        bool isGuaranteed = gachaCounts[poolKey] >= pool.guarantee.interval;
        
        string result = PerformGacha(poolKey, isGuaranteed);
        
        if (isGuaranteed)
            gachaCounts[poolKey] = 0;
        
        ShowGachaResult(result);
    }

    bool CanAfford(GachaPool pool)
    {
        if (pool.cost.coin > 0 && CurrencyManager.instance.GetCurrency("coin") < pool.cost.coin)
            return false;
        if (pool.cost.token > 0 && CurrencyManager.instance.GetCurrency("token") < pool.cost.token)
            return false;
        if (pool.cost.jade > 0 && CurrencyManager.instance.GetCurrency("jade") < pool.cost.jade)
            return false;
        return true;
    }

    void SpendCurrency(GachaPool pool)
    {
        if (pool.cost.coin > 0)
            CurrencyManager.instance.SpendCurrency("coin", pool.cost.coin);
        if (pool.cost.token > 0)
            CurrencyManager.instance.SpendCurrency("token", pool.cost.token);
        if (pool.cost.jade > 0)
            CurrencyManager.instance.SpendCurrency("jade", pool.cost.jade);
        
        UIManager.instance.UpdateCurrencyUI();
    }

    string PerformGacha(string poolKey, bool guaranteed)
    {
        GachaPool pool = GameManager.instance.shopData.gachaPools[poolKey];
        
        string targetRarity = guaranteed ? pool.guarantee.minimumRarity : SelectRarity(pool);
        
        List<TroopData> troops = GameManager.instance.troopsData.troops.FindAll(t => t.rarity == targetRarity);
        
        if (troops.Count == 0)
        {
            troops = GameManager.instance.troopsData.troops;
        }
        
        TroopData resultTroop = troops[Random.Range(0, troops.Count)];
        
        AddToInventory(resultTroop.id);
        
        return resultTroop.id;
    }

    string SelectRarity(GachaPool pool)
    {
        int totalWeight = 0;
        foreach (var weight in pool.rarityWeights.Values)
        {
            totalWeight += weight;
        }
        
        int random = Random.Range(0, totalWeight);
        int current = 0;
        
        foreach (var kvp in pool.rarityWeights)
        {
            current += kvp.Value;
            if (random < current)
            {
                return kvp.Key;
            }
        }
        
        return "普通";
    }

    void AddToInventory(string troopId)
    {
        InventoryItem item;
        if (GameManager.instance.currentSave.inventory.TryGetValue(troopId, out item))
        {
            item.count++;
        }
        else
        {
            GameManager.instance.currentSave.inventory[troopId] = new InventoryItem
            {
                count = 1,
                star = 1,
                level = 1,
                fragments = 0
            };
        }
        
        GameManager.instance.SaveGame();
    }

    void ShowGachaResult(string troopId)
    {
        TroopData troop = GameManager.instance.GetTroopData(troopId);
        
        if (resultTroopName != null)
            resultTroopName.text = troop.name;
        
        if (resultRarity != null)
            resultRarity.text = troop.rarity;
        
        if (resultRarityBackground != null)
        {
            Color color = Color.gray;
            if (GameManager.instance.shopData.rarityColors.TryGetValue(troop.rarity, out string colorString))
            {
                ColorUtility.TryParseHtmlString(colorString, out color);
            }
            resultRarityBackground.color = color;
        }
        
        gachaResultPopup.SetActive(true);
        
        Invoke("HideGachaResult", 3.0f);
    }

    void HideGachaResult()
    {
        gachaResultPopup.SetActive(false);
    }

    public void OnBackButton()
    {
        UIManager.instance.OnBackButton();
    }
}
