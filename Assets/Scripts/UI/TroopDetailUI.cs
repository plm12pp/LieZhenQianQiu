using UnityEngine;
using UnityEngine.UI;

public class TroopDetailUI : MonoBehaviour
{
    public GameObject detailPanel;
    public Image backgroundImage;
    public Image rarityGlowImage;
    public Image rarityBorderImage;
    
    public Image troopIconImage;
    public Text troopNameText;
    public Text dynastyText;
    public Text roleText;
    public Text rarityText;
    public Text starText;
    public Text sizeText;
    
    public Slider hpSlider;
    public Text hpText;
    public Slider atkSlider;
    public Text atkText;
    public Slider armorSlider;
    public Text armorText;
    public Slider speedSlider;
    public Text speedText;
    public Slider rangeSlider;
    public Text rangeText;
    
    public Image skillIconImage;
    public Text skillNameText;
    public Text skillTypeText;
    public Text skillDescText;
    
    public Text counterBonusText;
    
    public Button closeButton;
    public Button deployButton;
    
    private string currentTroopId;
    private int currentStar;
    
    void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);
    }
    
    public void ShowTroopDetail(string troopId, int star = 1)
    {
        currentTroopId = troopId;
        currentStar = star;
        
        TroopData data = GameManager.instance.GetTroopData(troopId);
        if (data == null) return;
        
        float starBonus = GameManager.instance.GetStarBonus(star);
        
        if (troopNameText != null)
            troopNameText.text = data.name;
        
        if (dynastyText != null)
            dynastyText.text = $"【{data.dynasty}】";
        
        if (roleText != null)
            roleText.text = data.role;
        
        if (rarityText != null)
            rarityText.text = data.rarity;
        
        if (starText != null)
        {
            starText.text = "";
            for (int i = 0; i < star; i++)
                starText.text += "★";
        }
        
        if (sizeText != null)
            sizeText.text = $"占用: {data.size}格";
        
        int hpValue = Mathf.RoundToInt(data.hp * starBonus);
        int atkValue = Mathf.RoundToInt(data.atk * starBonus);
        int armorValue = Mathf.RoundToInt(data.armor * (1.0f + 0.1f * (star - 1)));
        
        SetStatBar(hpSlider, hpText, hpValue, 300, "生命");
        SetStatBar(atkSlider, atkText, atkValue, 100, "攻击");
        SetStatBar(armorSlider, armorText, armorValue, 40, "护甲");
        SetStatBar(speedSlider, speedText, Mathf.RoundToInt(data.speed * 10), 80, "速度");
        SetStatBar(rangeSlider, rangeText, Mathf.RoundToInt(data.range * 10), 60, "射程");
        
        if (skillNameText != null)
            skillNameText.text = data.skillName;
        
        if (skillTypeText != null)
            skillTypeText.text = data.skillType == "active" ? "主动技能" : "被动技能";
        
        if (skillDescText != null)
            skillDescText.text = data.skillDescription;
        
        if (counterBonusText != null)
        {
            string counterStr = "克制: ";
            if (data.counterBonus != null && data.counterBonus.Count > 0)
            {
                for (int i = 0; i < data.counterBonus.Count; i++)
                {
                    counterStr += GetCounterTypeName(data.counterBonus[i]);
                    if (i < data.counterBonus.Count - 1)
                        counterStr += "、";
                }
                counterStr += $" (伤害+{Mathf.RoundToInt((data.counterMultiplier - 1) * 100)}%)";
            }
            else
            {
                counterStr += "无";
            }
            counterBonusText.text = counterStr;
        }
        
        Color rarityColor = UIStyle.GetRarityColor(data.rarity);
        
        if (rarityBorderImage != null)
            rarityBorderImage.color = rarityColor;
        
        if (rarityGlowImage != null)
        {
            rarityGlowImage.color = rarityColor;
            rarityGlowImage.canvasRenderer.SetAlpha(0.15f);
        }
        
        if (detailPanel != null)
            detailPanel.SetActive(true);
        
        if (detailPanel != null)
        {
            detailPanel.transform.localScale = Vector3.zero;
            StartCoroutine(ShowAnimation());
        }
    }
    
    void SetStatBar(Slider slider, Text text, int value, int maxValue, string label)
    {
        if (slider != null)
        {
            slider.maxValue = maxValue;
            slider.value = Mathf.Min(value, maxValue);
        }
        
        if (text != null)
            text.text = $"{label}: {value}";
    }
    
    string GetCounterTypeName(string counterId)
    {
        switch (counterId)
        {
            case "archer": return "弓兵";
            case "crossbow": return "弩兵";
            case "light_cavalry": return "轻骑";
            case "heavy_cavalry": return "重骑";
            case "cavalry": return "骑兵";
            case "infantry": return "步兵";
            case "shield": return "盾兵";
            case "large_shield": return "大型盾阵";
            case "heavy_armor": return "重甲单位";
            case "catapult": return "投石车";
            case "artillery": return "炮兵";
            case "ranged": return "远程单位";
            case "formation": return "军阵";
            case "dense": return "密集阵型";
            case "vine_armor": return "藤甲兵";
            case "elephant": return "象兵";
            case "scholar": return "文官";
            case "naval": return "水军";
            case "assassin": return "刺客";
            case "city_defense": return "城防";
            case "large_unit": return "大型单位";
            case "all": return "全兵种";
            default: return counterId;
        }
    }
    
    System.Collections.IEnumerator ShowAnimation()
    {
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float scale = Mathf.Lerp(0f, 1.05f, t);
            if (t > 0.7f)
            {
                scale = Mathf.Lerp(1.05f, 1f, (t - 0.7f) / 0.3f);
            }
            detailPanel.transform.localScale = Vector3.one * scale;
            yield return null;
        }
        
        detailPanel.transform.localScale = Vector3.one;
    }
    
    public void Hide()
    {
        if (detailPanel != null)
        {
            StartCoroutine(HideAnimation());
        }
    }
    
    System.Collections.IEnumerator HideAnimation()
    {
        float duration = 0.2f;
        float elapsed = 0f;
        Vector3 startScale = detailPanel.transform.localScale;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            detailPanel.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
        
        detailPanel.SetActive(false);
        detailPanel.transform.localScale = Vector3.one;
    }
}
