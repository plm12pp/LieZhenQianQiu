using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FormationSelectUI : MonoBehaviour
{
    public GameObject formationPanel;
    public Transform formationGrid;
    public GameObject formationItemPrefab;
    public Text formationNameText;
    public Text formationDescText;
    public Button confirmButton;
    public Button closeButton;
    
    private FormationType selectedFormation;
    private List<FormationItemUI> formationItems;
    
    void Awake()
    {
        formationItems = new List<FormationItemUI>();
        InitFormationItems();
        
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmClick);
        
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClick);
    }
    
    void InitFormationItems()
    {
        if (formationGrid == null || formationItemPrefab == null) return;
        
        foreach (Transform child in formationGrid)
        {
            Destroy(child.gameObject);
        }
        
        formationItems.Clear();
        
        FormationType[] formations = {
            FormationType.NONE,
            FormationType.CRANE,
            FormationType.FISH_SCALE,
            FormationType.GOOSE,
            FormationType.CIRCLE,
            FormationType.SQUARE,
            FormationType.LONG_SNAKE,
            FormationType.HEAVEN_AND_EARTH
        };
        
        foreach (FormationType formation in formations)
        {
            GameObject itemObj = Instantiate(formationItemPrefab, formationGrid);
            FormationItemUI itemUI = itemObj.GetComponent<FormationItemUI>();
            if (itemUI == null)
                itemUI = itemObj.AddComponent<FormationItemUI>();
            
            itemUI.SetFormation(formation);
            itemUI.selectUI = this;
            formationItems.Add(itemUI);
        }
    }
    
    public void Show()
    {
        if (formationPanel != null)
            formationPanel.SetActive(true);
        
        selectedFormation = FormationManager.instance != null ? 
            FormationManager.instance.playerFormation : FormationType.NONE;
        
        UpdateSelection();
    }
    
    public void Hide()
    {
        if (formationPanel != null)
            formationPanel.SetActive(false);
    }
    
    public void SelectFormation(FormationType formation)
    {
        selectedFormation = formation;
        UpdateSelection();
        UpdateFormationInfo();
    }
    
    void UpdateSelection()
    {
        foreach (var item in formationItems)
        {
            if (item != null)
            {
                item.SetSelected(item.formationType == selectedFormation);
            }
        }
    }
    
    void UpdateFormationInfo()
    {
        if (FormationManager.instance == null) return;
        
        FormationInfo info = FormationManager.instance.GetFormationInfo(selectedFormation);
        
        if (formationNameText != null)
            formationNameText.text = info.name;
        
        if (formationDescText != null)
            formationDescText.text = info.description;
    }
    
    void OnConfirmClick()
    {
        if (FormationManager.instance != null)
        {
            FormationManager.instance.SetPlayerFormation(selectedFormation);
        }
        
        Hide();
        
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateFormationUI();
        }
    }
    
    void OnCloseClick()
    {
        Hide();
    }
}

public class FormationItemUI : MonoBehaviour
{
    public Image backgroundImage;
    public Image iconImage;
    public Text nameText;
    public Image selectedBorder;
    public Button itemButton;
    
    public FormationType formationType;
    public FormationSelectUI selectUI;
    
    private Color normalColor = Color.white;
    private Color selectedColor = new Color32(0xF1, 0xC4, 0x0F, 0xFF);
    
    void Start()
    {
        if (itemButton != null)
            itemButton.onClick.AddListener(OnItemClick);
    }
    
    public void SetFormation(FormationType type)
    {
        formationType = type;
        
        if (FormationManager.instance != null)
        {
            FormationInfo info = FormationManager.instance.GetFormationInfo(type);
            if (nameText != null)
                nameText.text = info.name;
        }
    }
    
    public void SetSelected(bool selected)
    {
        if (selectedBorder != null)
            selectedBorder.gameObject.SetActive(selected);
        
        if (backgroundImage != null)
            backgroundImage.color = selected ? new Color(1f, 1f, 0.9f) : Color.white;
    }
    
    void OnItemClick()
    {
        if (selectUI != null)
        {
            selectUI.SelectFormation(formationType);
        }
    }
}
