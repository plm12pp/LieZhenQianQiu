using UnityEngine;
using UnityEngine.UI;

public class BattleUnitUI : MonoBehaviour
{
    public Image healthBarBackground;
    public Image healthBarFill;
    public Image borderImage;
    public Text nameText;
    public Text damageText;
    
    public UnitController unitController;
    private CanvasGroup canvasGroup;
    
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }
    
    void LateUpdate()
    {
        if (unitController == null) return;
        
        UpdateHealthBar();
        UpdatePosition();
    }
    
    public void AttachToUnit(UnitController unit)
    {
        unitController = unit;
        
        if (unitController == null) return;
        
        TroopData data = GameManager.instance.GetTroopData(unit.troopDataId);
        if (data != null && nameText != null)
        {
            nameText.text = data.name;
        }
        
        if (borderImage != null)
        {
            borderImage.color = unit.side == UnitSide.PLAYER ? UIStyle.PrimaryBlue : UIStyle.PrimaryRed;
        }
        
        if (healthBarFill != null)
        {
            healthBarFill.color = unit.side == UnitSide.PLAYER ? UIStyle.JadeGreen : UIStyle.PrimaryRed;
        }
    }
    
    void UpdateHealthBar()
    {
        if (healthBarFill == null || unitController == null) return;
        
        float healthPercent = unitController.GetHealthPercentage();
        healthBarFill.fillAmount = healthPercent;
        
        if (healthPercent < 0.3f)
        {
            healthBarFill.color = Color.red;
        }
        else if (healthPercent < 0.6f)
        {
            healthBarFill.color = Color.yellow;
        }
        else
        {
            healthBarFill.color = unitController.side == UnitSide.PLAYER ? UIStyle.JadeGreen : UIStyle.PrimaryRed;
        }
    }
    
    void UpdatePosition()
    {
        if (unitController == null) return;
        
        Vector3 worldPos = new Vector3(
            unitController.position.x + (unitController.isHorizontal ? unitController.size / 2f : 0.5f),
            unitController.position.y + (unitController.isHorizontal ? 0.5f : unitController.size / 2f),
            0
        );
        
        transform.position = worldPos + Vector3.up * 1.2f;
    }
    
    public void ShowDamage(int damage)
    {
        if (damageText == null) return;
        
        damageText.text = $"-{damage}";
        damageText.gameObject.SetActive(true);
        
        StartCoroutine(DamagePopupRoutine());
    }
    
    System.Collections.IEnumerator DamagePopupRoutine()
    {
        float duration = 0.8f;
        float elapsed = 0f;
        Vector3 startPos = damageText.transform.position;
        
        damageText.color = Color.white;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            damageText.transform.position = startPos + Vector3.up * (t * 30f);
            
            Color color = damageText.color;
            color.a = 1f - t;
            damageText.color = color;
            
            yield return null;
        }
        
        damageText.gameObject.SetActive(false);
        damageText.transform.position = startPos;
    }
    
    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }
    
    System.Collections.IEnumerator FadeOutRoutine()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - (elapsed / duration);
            yield return null;
        }
        
        Destroy(gameObject);
    }
}
