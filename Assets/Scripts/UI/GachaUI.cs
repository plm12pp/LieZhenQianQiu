using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GachaUI : MonoBehaviour
{
    public GameObject gachaPanel;
    public GameObject resultPanel;
    public Transform cardContainer;
    public GameObject cardPrefab;
    
    public Button singleDrawButton;
    public Button tenDrawButton;
    public Button closeButton;
    
    public Text singleCostText;
    public Text tenCostText;
    public Text currencyText;
    
    private List<TroopCardUI> resultCards;
    
    void Awake()
    {
        resultCards = new List<TroopCardUI>();
        
        if (singleDrawButton != null)
            singleDrawButton.onClick.AddListener(OnSingleDraw);
        
        if (tenDrawButton != null)
            tenDrawButton.onClick.AddListener(OnTenDraw);
        
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseResult);
    }
    
    public void Show()
    {
        if (gachaPanel != null)
            gachaPanel.SetActive(true);
        
        UpdateCurrencyDisplay();
    }
    
    public void Hide()
    {
        if (gachaPanel != null)
            gachaPanel.SetActive(false);
    }
    
    void UpdateCurrencyDisplay()
    {
        if (currencyText != null && CurrencyManager.instance != null)
        {
            currencyText.text = CurrencyManager.instance.currency.ToString();
        }
    }
    
    void OnSingleDraw()
    {
        DoDraw(1);
    }
    
    void OnTenDraw()
    {
        DoDraw(10);
    }
    
    void DoDraw(int count)
    {
        List<string> drawnTroops = DrawTroops(count);
        ShowDrawResult(drawnTroops);
    }
    
    List<string> DrawTroops(int count)
    {
        List<string> result = new List<string>();
        
        if (GameManager.instance == null) return result;
        
        for (int i = 0; i < count; i++)
        {
            string troopId = DrawSingleTroop();
            if (!string.IsNullOrEmpty(troopId))
                result.Add(troopId);
        }
        
        return result;
    }
    
    string DrawSingleTroop()
    {
        if (GameManager.instance == null || GameManager.instance.allTroops == null)
            return "";
        
        int totalWeight = 0;
        Dictionary<string, int> rarityWeights = new Dictionary<string, int>();
        
        foreach (var troop in GameManager.instance.allTroops.troops)
        {
            if (!rarityWeights.ContainsKey(troop.rarity))
            {
                int weight = 1;
                if (GameManager.instance.allTroops.rarityWeight != null &&
                    GameManager.instance.allTroops.rarityWeight.ContainsKey(troop.rarity))
                {
                    weight = GameManager.instance.allTroops.rarityWeight[troop.rarity];
                }
                rarityWeights[troop.rarity] = weight;
            }
        }
        
        string selectedRarity = SelectRarity(rarityWeights);
        
        List<TroopData> rarityTroops = new List<TroopData>();
        foreach (var troop in GameManager.instance.allTroops.troops)
        {
            if (troop.rarity == selectedRarity)
                rarityTroops.Add(troop);
        }
        
        if (rarityTroops.Count == 0)
            return "";
        
        TroopData selected = rarityTroops[Random.Range(0, rarityTroops.Count)];
        return selected.id;
    }
    
    string SelectRarity(Dictionary<string, int> rarityWeights)
    {
        int totalWeight = 0;
        foreach (var kvp in rarityWeights)
            totalWeight += kvp.Value;
        
        int random = Random.Range(0, totalWeight);
        int current = 0;
        
        foreach (var kvp in rarityWeights)
        {
            current += kvp.Value;
            if (random < current)
                return kvp.Key;
        }
        
        return "普通";
    }
    
    void ShowDrawResult(List<string> troopIds)
    {
        if (resultPanel != null)
            resultPanel.SetActive(true);
        
        if (cardContainer == null || cardPrefab == null) return;
        
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }
        
        resultCards.Clear();
        
        for (int i = 0; i < troopIds.Count; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardContainer);
            TroopCardUI cardUI = cardObj.GetComponent<TroopCardUI>();
            if (cardUI == null)
                cardUI = cardObj.AddComponent<TroopCardUI>();
            
            cardUI.SetTroopData(troopIds[i], 1, 1);
            cardUI.gameObject.SetActive(false);
            resultCards.Add(cardUI);
        }
        
        StartCoroutine(RevealCards());
    }
    
    System.Collections.IEnumerator RevealCards()
    {
        for (int i = 0; i < resultCards.Count; i++)
        {
            if (resultCards[i] != null)
            {
                resultCards[i].gameObject.SetActive(true);
                resultCards[i].PlayGetAnimation();
            }
            yield return new WaitForSeconds(0.15f);
        }
    }
    
    void OnCloseResult()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);
        
        UpdateCurrencyDisplay();
    }
}
