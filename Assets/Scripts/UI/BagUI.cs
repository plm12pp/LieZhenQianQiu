using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BagUI : MonoBehaviour
{
    public GameObject troopItemPrefab;
    public Transform troopListParent;
    public GameObject troopDetailPanel;
    public Text detailName;
    public Text detailDynasty;
    public Text detailRole;
    public Text detailRarity;
    public Text detailStats;
    public Text detailSkill;
    public Text starText;
    public Text countText;
    public Button upgradeButton;
    public Button decomposeButton;
    
    private string selectedTroopId;
    
    void Start()
    {
        RefreshTroopList();
    }

    void RefreshTroopList()
    {
        foreach (Transform child in troopListParent)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var kvp in GameManager.instance.currentSave.inventory)
        {
            if (kvp.Value.count > 0)
            {
                CreateTroopItem(kvp.Key, kvp.Value);
            }
        }
    }

    void CreateTroopItem(string troopId, InventoryItem item)
    {
        TroopData troopData = GameManager.instance.GetTroopData(troopId);
        if (troopData == null)
            return;
        
        GameObject itemObj = Instantiate(troopItemPrefab, troopListParent);
        Text[] texts = itemObj.GetComponentsInChildren<Text>();
        
        if (texts.Length > 0)
            texts[0].text = troopData.name;
        if (texts.Length > 1)
            texts[1].text = $"{item.count}个";
        if (texts.Length > 2)
            texts[2].text = $"{item.star}星";
        
        Button button = itemObj.GetComponent<Button>();
        button.onClick.AddListener(() => OnTroopSelected(troopId));
    }

    void OnTroopSelected(string troopId)
    {
        selectedTroopId = troopId;
        TroopData troopData = GameManager.instance.GetTroopData(troopId);
        InventoryItem item = GameManager.instance.currentSave.inventory[troopId];
        
        if (detailName != null)
            detailName.text = troopData.name;
        if (detailDynasty != null)
            detailDynasty.text = troopData.dynasty;
        if (detailRole != null)
            detailRole.text = troopData.role;
        if (detailRarity != null)
            detailRarity.text = troopData.rarity;
        
        float bonus = GameManager.instance.GetStarBonus(item.star);
        float armorBonus = 1.0f + 0.1f * (item.star - 1);
        if (detailStats != null)
            detailStats.text = $"生命: {Mathf.RoundToInt(troopData.hp * bonus)}\n攻击: {Mathf.RoundToInt(troopData.atk * bonus)}\n护甲: {Mathf.RoundToInt(troopData.armor * armorBonus)}\n射程: {troopData.range}\n速度: {troopData.speed}\n占格: {troopData.size}";
        
        if (detailSkill != null)
            detailSkill.text = $"{troopData.skill}\n{troopData.skillDescription}";
        
        if (starText != null)
            starText.text = $"{item.star}/5 星";
        
        if (countText != null)
            countText.text = $"拥有: {item.count} 个";
        
        UpdateUpgradeButton(item);
        
        troopDetailPanel.SetActive(true);
    }

    void UpdateUpgradeButton(InventoryItem item)
    {
        if (item.star >= 5)
        {
            upgradeButton.interactable = false;
            upgradeButton.GetComponentInChildren<Text>().text = "已满星";
            return;
        }
        
        StarUpgradeCost cost = GameManager.instance.GetStarUpgradeCost(item.star);
        
        if (item.count >= cost.units + 1)
        {
            upgradeButton.interactable = true;
            upgradeButton.GetComponentInChildren<Text>().text = $"升星 ({cost.units}个)";
        }
        else if (item.fragments >= cost.fragments)
        {
            upgradeButton.interactable = true;
            upgradeButton.GetComponentInChildren<Text>().text = $"升星 ({cost.fragments}碎片)";
        }
        else
        {
            upgradeButton.interactable = false;
            upgradeButton.GetComponentInChildren<Text>().text = "材料不足";
        }
    }

    public void OnUpgradeButton()
    {
        if (string.IsNullOrEmpty(selectedTroopId))
            return;
        
        InventoryItem item = GameManager.instance.currentSave.inventory[selectedTroopId];
        StarUpgradeCost cost = GameManager.instance.GetStarUpgradeCost(item.star);
        
        if (item.count >= cost.units + 1)
        {
            item.count -= cost.units;
            item.star++;
        }
        else if (item.fragments >= cost.fragments)
        {
            item.fragments -= cost.fragments;
            item.star++;
        }
        
        GameManager.instance.SaveGame();
        RefreshTroopList();
        OnTroopSelected(selectedTroopId);
        
        UIManager.instance.ShowMessage("升星成功！");
    }

    public void OnDecomposeButton()
    {
        if (string.IsNullOrEmpty(selectedTroopId))
            return;
        
        InventoryItem item = GameManager.instance.currentSave.inventory[selectedTroopId];
        
        if (item.count > 0)
        {
            item.count--;
            item.fragments += 5;
            
            GameManager.instance.SaveGame();
            RefreshTroopList();
            OnTroopSelected(selectedTroopId);
            
            UIManager.instance.ShowMessage("分解成功！");
        }
    }

    public void OnBackButton()
    {
        UIManager.instance.OnBackButton();
    }
}
