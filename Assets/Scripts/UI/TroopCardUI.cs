using UnityEngine;
using UnityEngine.UI;

public class TroopCardUI : MonoBehaviour
{
    public Image backgroundImage;
    public Image borderImage;
    public Image rarityGlowImage;
    public Image troopIconImage;
    public Text nameText;
    public Text dynastyText;
    public Text roleText;
    public Text sizeText;
    public Text starText;
    public Text countText;
    public Image skillIconImage;
    public Text skillNameText;
    
    public Button cardButton;
    public string troopId;
    public bool isSelected;
    
    private Color normalColor;
    private Color selectedColor;
    
    void Start()
    {
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(OnCardClick);
        }
    }
    
    public void SetTroopData(string troopId, int star = 1, int count = 0)
    {
        this.troopId = troopId;
        
        TroopData data = GameManager.instance.GetTroopData(troopId);
        if (data == null) return;
        
        if (nameText != null)
            nameText.text = data.name;
        
        if (dynastyText != null)
            dynastyText.text = data.dynasty;
        
        if (roleText != null)
            roleText.text = data.role;
        
        if (sizeText != null)
            sizeText.text = $"{data.size}格";
        
        if (starText != null)
        {
            starText.text = "";
            for (int i = 0; i < star; i++)
            {
                starText.text += "★";
            }
        }
        
        if (countText != null)
            countText.text = count > 0 ? $"×{count}" : "";
        
        if (skillNameText != null)
            skillNameText.text = data.skillName;
        
        Color rarityColor = UIStyle.GetRarityColor(data.rarity);
        
        if (rarityGlowImage != null)
        {
            rarityGlowImage.color = rarityColor;
            rarityGlowImage.canvasRenderer.SetAlpha(0.2f);
        }
        
        if (borderImage != null)
        {
            borderImage.color = rarityColor;
        }
        
        if (backgroundImage != null)
        {
            backgroundImage.color = UIStyle.Cream;
        }
    }
    
    void OnCardClick()
    {
        isSelected = !isSelected;
        UpdateSelectionState();
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateSelectionState();
    }
    
    void UpdateSelectionState()
    {
        if (isSelected)
        {
            if (backgroundImage != null)
                backgroundImage.color = UIStyle.LightBeige;
            
            if (rarityGlowImage != null)
                rarityGlowImage.canvasRenderer.SetAlpha(0.5f);
        }
        else
        {
            if (backgroundImage != null)
                backgroundImage.color = UIStyle.Cream;
            
            if (rarityGlowImage != null)
                rarityGlowImage.canvasRenderer.SetAlpha(0.2f);
        }
    }
    
    public void PlayGetAnimation()
    {
        StartCoroutine(GetAnimationRoutine());
    }
    
    System.Collections.IEnumerator GetAnimationRoutine()
    {
        transform.localScale = Vector3.zero;
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float scale = Mathf.Lerp(0f, 1.2f, t);
            if (t > 0.7f)
            {
                scale = Mathf.Lerp(1.2f, 1f, (t - 0.7f) / 0.3f);
            }
            transform.localScale = Vector3.one * scale;
            yield return null;
        }
        
        transform.localScale = Vector3.one;
    }
}
