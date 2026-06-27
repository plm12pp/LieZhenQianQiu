using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleResultUI : MonoBehaviour
{
    public GameObject resultPanel;
    public Image backgroundImage;
    public Text resultTitleText;
    public Text resultDescText;
    
    public Image victoryIcon;
    public Image defeatIcon;
    
    public Transform rewardsGrid;
    public GameObject rewardItemPrefab;
    
    public Text expText;
    public Text currencyText;
    
    public Button confirmButton;
    public Button retryButton;
    
    private bool isVictory;
    
    void Awake()
    {
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmClick);
        
        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryClick);
    }
    
    public void ShowVictory(int exp, int currency, List<RewardItem> rewards)
    {
        isVictory = true;
        ShowResult(true, exp, currency, rewards);
    }
    
    public void ShowDefeat(int exp, int currency)
    {
        isVictory = false;
        ShowResult(false, exp, currency, null);
    }
    
    void ShowResult(bool victory, int exp, int currency, List<RewardItem> rewards)
    {
        if (resultPanel != null)
            resultPanel.SetActive(true);
        
        if (resultTitleText != null)
            resultTitleText.text = victory ? "战斗胜利！" : "战斗失败";
        
        if (resultDescText != null)
            resultDescText.text = victory ? "恭喜获得胜利！" : "再接再厉，下次一定能赢！";
        
        if (victoryIcon != null)
            victoryIcon.gameObject.SetActive(victory);
        
        if (defeatIcon != null)
            defeatIcon.gameObject.SetActive(!victory);
        
        if (expText != null)
            expText.text = $"经验 +{exp}";
        
        if (currencyText != null)
            currencyText.text = $"铜钱 +{currency}";
        
        if (retryButton != null)
            retryButton.gameObject.SetActive(!victory);
        
        ShowRewards(rewards);
        StartCoroutine(ShowAnimation());
    }
    
    void ShowRewards(List<RewardItem> rewards)
    {
        if (rewardsGrid == null || rewardItemPrefab == null) return;
        
        foreach (Transform child in rewardsGrid)
        {
            Destroy(child.gameObject);
        }
        
        if (rewards == null || rewards.Count == 0) return;
        
        foreach (var reward in rewards)
        {
            GameObject itemObj = Instantiate(rewardItemPrefab, rewardsGrid);
            RewardItemUI itemUI = itemObj.GetComponent<RewardItemUI>();
            if (itemUI == null)
                itemUI = itemObj.AddComponent<RewardItemUI>();
            
            itemUI.SetReward(reward);
        }
    }
    
    System.Collections.IEnumerator ShowAnimation()
    {
        resultPanel.transform.localScale = Vector3.zero;
        
        float duration = 0.4f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float scale = Mathf.Lerp(0f, 1.1f, t);
            if (t > 0.7f)
            {
                scale = Mathf.Lerp(1.1f, 1f, (t - 0.7f) / 0.3f);
            }
            resultPanel.transform.localScale = Vector3.one * scale;
            yield return null;
        }
        
        resultPanel.transform.localScale = Vector3.one;
    }
    
    void OnConfirmClick()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);
        
        if (BattleManager.instance != null)
        {
            BattleManager.instance.ExitBattle();
        }
    }
    
    void OnRetryClick()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);
        
        if (BattleManager.instance != null)
        {
            BattleManager.instance.RetryBattle();
        }
    }
}

public class RewardItem
{
    public string icon;
    public string name;
    public int count;
    public string rarity;
}

public class RewardItemUI : MonoBehaviour
{
    public Image iconImage;
    public Text nameText;
    public Text countText;
    public Image rarityBorder;
    
    public void SetReward(RewardItem reward)
    {
        if (nameText != null)
            nameText.text = reward.name;
        
        if (countText != null)
            countText.text = $"×{reward.count}";
        
        if (rarityBorder != null)
            rarityBorder.color = UIStyle.GetRarityColor(reward.rarity);
    }
}
