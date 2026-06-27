using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Canvas mainCanvas;
    
    public GameObject mainMenuScreen;
    public GameObject battleScreen;
    public GameObject formationScreen;
    public GameObject shopScreen;
    public GameObject bagScreen;
    public GameObject settingsScreen;
    public GameObject achievementScreen;
    public GameObject victoryScreen;
    public GameObject defeatScreen;
    public GameObject messagePopup;
    
    public Text playerLivesText;
    public Text enemyLivesText;
    public Text levelText;
    public Text formationUnitCountText;
    public Text formationCellCountText;
    
    public Text coinText;
    public Text tokenText;
    public Text jadeText;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateCurrencyUI();
    }

    public void ShowMainMenu()
    {
        HideAllScreens();
        mainMenuScreen.SetActive(true);
        UpdateCurrencyUI();
    }

    public void ShowBattleScreen()
    {
        HideAllScreens();
        battleScreen.SetActive(true);
        UpdateBattleUI();
    }

    public void ShowFormationScreen()
    {
        HideAllScreens();
        formationScreen.SetActive(true);
        UpdateFormationUI();
    }

    public void ShowRedeployScreen()
    {
        HideAllScreens();
        formationScreen.SetActive(true);
        ShowMessage("请重新布阵");
    }

    public void ShowShopScreen()
    {
        HideAllScreens();
        shopScreen.SetActive(true);
        UpdateCurrencyUI();
    }

    public void ShowBagScreen()
    {
        HideAllScreens();
        bagScreen.SetActive(true);
    }

    public void ShowSettingsScreen()
    {
        HideAllScreens();
        settingsScreen.SetActive(true);
    }

    public void ShowAchievementScreen()
    {
        HideAllScreens();
        achievementScreen.SetActive(true);
    }

    public void ShowVictoryScreen()
    {
        HideAllScreens();
        victoryScreen.SetActive(true);
    }

    public void ShowDefeatScreen()
    {
        HideAllScreens();
        defeatScreen.SetActive(true);
    }

    void HideAllScreens()
    {
        mainMenuScreen.SetActive(false);
        battleScreen.SetActive(false);
        formationScreen.SetActive(false);
        shopScreen.SetActive(false);
        bagScreen.SetActive(false);
        settingsScreen.SetActive(false);
        achievementScreen.SetActive(false);
        victoryScreen.SetActive(false);
        defeatScreen.SetActive(false);
    }

    public void UpdateCurrencyUI()
    {
        coinText.text = CurrencyManager.instance.GetCurrency("coin").ToString();
        tokenText.text = CurrencyManager.instance.GetCurrency("token").ToString();
        jadeText.text = CurrencyManager.instance.GetCurrency("jade").ToString();
    }

    public void UpdateBattleUI()
    {
        if (playerLivesText != null)
            playerLivesText.text = BattleManager.instance.playerLives.ToString();
        if (enemyLivesText != null)
            enemyLivesText.text = BattleManager.instance.enemyLives.ToString();
        if (levelText != null)
            levelText.text = $"第 {BattleManager.instance.currentLevel} 关";
    }

    public void UpdateFormationUI()
    {
        if (formationUnitCountText != null)
            formationUnitCountText.text = $"{FormationManager.instance.GetPlayerUnitCount()}/10";
        if (formationCellCountText != null)
            formationCellCountText.text = $"{FormationManager.instance.GetPlayerOccupiedCells()}/35";
    }

    public void UpdatePlayerLives(int lives)
    {
        if (playerLivesText != null)
            playerLivesText.text = lives.ToString();
    }

    public void UpdateEnemyLives(int lives)
    {
        if (enemyLivesText != null)
            enemyLivesText.text = lives.ToString();
    }

    public void ShowMessage(string message)
    {
        messagePopup.SetActive(true);
        Text messageText = messagePopup.GetComponentInChildren<Text>();
        if (messageText != null)
            messageText.text = message;
        
        Invoke("HideMessage", 2.0f);
    }

    void HideMessage()
    {
        messagePopup.SetActive(false);
    }

    public void OnStartBattleButton()
    {
        int level = GameManager.instance.currentSave.currentDisplayLevel;
        BattleManager.instance.StartBattle(level);
    }

    public void OnShopButton()
    {
        ShowShopScreen();
    }

    public void OnBagButton()
    {
        ShowBagScreen();
    }

    public void OnSettingsButton()
    {
        ShowSettingsScreen();
    }

    public void OnAchievementButton()
    {
        ShowAchievementScreen();
    }

    public void OnBackButton()
    {
        ShowMainMenu();
    }

    public void OnStartCombatButton()
    {
        if (FormationManager.instance.GetPlayerUnitCount() >= 5)
        {
            BattleManager.instance.StartCombat();
        }
        else
        {
            ShowMessage("至少需要放置5个兵种！");
        }
    }

    public void OnNextLevelButton()
    {
        FormationManager.instance.ClearAllUnits();
        BattleManager.instance.StartNextLevel();
    }

    public void OnRetryButton()
    {
        FormationManager.instance.ClearAllUnits();
        BattleManager.instance.StartBattle(BattleManager.instance.currentLevel);
    }
}
