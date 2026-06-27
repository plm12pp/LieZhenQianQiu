using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AnimatedButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Button button;
    public RectTransform buttonRect;
    public Image backgroundImage;
    public Text buttonText;
    
    public Color normalColor = UIStyle.PrimaryBlue;
    public Color hoverColor = new Color32(0x25, 0x63, 0xEB, 0xFF);
    public Color pressedColor = new Color32(0x1D, 0x4E, 0xD8, 0xFF);
    public Color disabledColor = UIStyle.Gray;
    
    public float normalScale = 1f;
    public float hoverScale = 1.05f;
    public float pressedScale = 0.95f;
    
    public float animationDuration = 0.15f;
    
    private bool isInteractable = true;
    private Coroutine currentAnimation;
    
    void Start()
    {
        if (button == null)
            button = GetComponent<Button>();
        
        if (buttonRect == null)
            buttonRect = GetComponent<RectTransform>();
        
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
        
        if (buttonText == null)
            buttonText = GetComponentInChildren<Text>();
        
        UpdateVisualState();
    }
    
    void UpdateVisualState()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isInteractable ? normalColor : disabledColor;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        AnimateToState(hoverScale, hoverColor);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        AnimateToState(normalScale, normalColor);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        AnimateToState(pressedScale, pressedColor);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        AnimateToState(hoverScale, hoverColor);
    }
    
    void AnimateToState(float targetScale, Color targetColor)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
        
        currentAnimation = StartCoroutine(AnimateRoutine(targetScale, targetColor));
    }
    
    System.Collections.IEnumerator AnimateRoutine(float targetScale, Color targetColor)
    {
        float elapsed = 0f;
        float startScale = buttonRect.localScale.x;
        Color startColor = backgroundImage != null ? backgroundImage.color : Color.white;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            t = Mathf.SmoothStep(0f, 1f, t);
            
            buttonRect.localScale = Vector3.one * Mathf.Lerp(startScale, targetScale, t);
            
            if (backgroundImage != null)
            {
                backgroundImage.color = Color.Lerp(startColor, targetColor, t);
            }
            
            yield return null;
        }
        
        buttonRect.localScale = Vector3.one * targetScale;
        
        if (backgroundImage != null)
        {
            backgroundImage.color = targetColor;
        }
    }
    
    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
        if (button != null)
            button.interactable = interactable;
        
        UpdateVisualState();
    }
    
    public void PlayClickAnimation()
    {
        StartCoroutine(ClickAnimationRoutine());
    }
    
    System.Collections.IEnumerator ClickAnimationRoutine()
    {
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            float scale = normalScale;
            if (t < 0.3f)
            {
                scale = Mathf.Lerp(normalScale, pressedScale, t / 0.3f);
            }
            else
            {
                scale = Mathf.Lerp(pressedScale, normalScale, (t - 0.3f) / 0.7f);
            }
            
            buttonRect.localScale = Vector3.one * scale;
            yield return null;
        }
        
        buttonRect.localScale = Vector3.one * normalScale;
    }
}
