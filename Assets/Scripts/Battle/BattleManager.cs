using UnityEngine;
using System.Collections.Generic;

public enum BattlePhase
{
    FORMATION,
    BATTLE,
    REDEPLOY,
    END
}

public enum BattleResult
{
    NONE,
    VICTORY,
    DEFEAT
}

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    public BattlePhase currentPhase;
    public BattleResult battleResult;
    
    public int playerLives;
    public int enemyLives;
    public int currentLevel;
    public float difficultyMultiplier;
    
    public List<UnitController> playerUnits;
    public List<UnitController> enemyUnits;
    public List<string> deadPlayerUnits;
    public List<string> deadEnemyUnits;
    
    public bool isPlayerTurn;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartBattle(int level)
    {
        currentLevel = level;
        difficultyMultiplier = GameManager.instance.GetDifficultyMultiplier(GameManager.instance.currentSave.difficultyIndex);
        
        playerLives = GameManager.instance.GetMaxLives();
        enemyLives = GameManager.instance.GetMaxLives();
        
        playerUnits = new List<UnitController>();
        enemyUnits = new List<UnitController>();
        deadPlayerUnits = new List<string>();
        deadEnemyUnits = new List<string>();
        
        currentPhase = BattlePhase.FORMATION;
        battleResult = BattleResult.NONE;
        
        UIManager.instance.ShowFormationScreen();
    }

    public void StartCombat()
    {
        if (!ValidateFormation(playerUnits))
        {
            UIManager.instance.ShowMessage("布阵不合法！");
            return;
        }

        AIBattleController.instance.GenerateEnemyFormation();
        
        currentPhase = BattlePhase.BATTLE;
        isPlayerTurn = true;
        
        UIManager.instance.HideFormationScreen();
        UIManager.instance.ShowBattleScreen();
    }

    bool ValidateFormation(List<UnitController> units)
    {
        if (units.Count < 5)
            return false;
        if (units.Count > 10)
            return false;
        
        int totalCells = 0;
        foreach (var unit in units)
        {
            totalCells += unit.size;
        }
        
        return totalCells <= 35;
    }

    public void OnUnitDeath(UnitController unit)
    {
        if (unit.side == UnitSide.PLAYER)
        {
            playerUnits.Remove(unit);
            deadPlayerUnits.Add(unit.unitId);
            
            if (playerUnits.Count == 0)
            {
                HandleSideDefeat(UnitSide.PLAYER);
            }
        }
        else
        {
            enemyUnits.Remove(unit);
            deadEnemyUnits.Add(unit.unitId);
            
            if (enemyUnits.Count == 0)
            {
                HandleSideDefeat(UnitSide.ENEMY);
            }
        }
    }

    void HandleSideDefeat(UnitSide side)
    {
        if (side == UnitSide.PLAYER)
        {
            playerLives--;
            UIManager.instance.UpdatePlayerLives(playerLives);
        }
        else
        {
            enemyLives--;
            UIManager.instance.UpdateEnemyLives(enemyLives);
        }

        if (CheckBattleEnd())
            return;

        currentPhase = BattlePhase.REDEPLOY;
        
        if (side == UnitSide.PLAYER)
        {
            UIManager.instance.ShowRedeployScreen();
        }
        else
        {
            UIManager.instance.ShowRedeployScreen();
            AIBattleController.instance.GenerateEnemyFormation();
        }
    }

    bool CheckBattleEnd()
    {
        if (playerLives <= 0)
        {
            battleResult = BattleResult.DEFEAT;
            EndBattle();
            return true;
        }
        
        if (enemyLives <= 0)
        {
            battleResult = BattleResult.VICTORY;
            EndBattle();
            return true;
        }
        
        return false;
    }

    public void EndBattle()
    {
        currentPhase = BattlePhase.END;
        
        if (battleResult == BattleResult.VICTORY)
        {
            GameManager.instance.currentSave.totalCleared++;
            GameManager.instance.currentSave.winStreak++;
            GameManager.instance.currentSave.difficultyIndex++;
            
            int nextLevel = GameManager.instance.CalculateNextLevel(currentLevel);
            GameManager.instance.currentSave.currentDisplayLevel = nextLevel;
            
            GiveVictoryRewards();
            
            UIManager.instance.ShowVictoryScreen();
        }
        else
        {
            GameManager.instance.currentSave.winStreak = 0;
            GameManager.instance.currentSave.perfectWinStreak = 0;
            
            GiveDefeatRewards();
            
            UIManager.instance.ShowDefeatScreen();
        }
        
        GameManager.instance.SaveGame();
    }

    void GiveVictoryRewards()
    {
        LevelRewards rewards = GameManager.instance.levelData.levelRewards;
        int coinReward = rewards.baseCoinReward + rewards.coinIncreasePerLevel * currentLevel;
        
        CurrencyManager.instance.AddCurrency("coin", coinReward);
        
        if (Random.value < rewards.tokenRewardChance)
        {
            CurrencyManager.instance.AddCurrency("token", rewards.tokenRewardAmount);
        }
        
        if (Random.value < rewards.jadeRewardChance)
        {
            CurrencyManager.instance.AddCurrency("jade", rewards.jadeRewardAmount);
        }
    }

    void GiveDefeatRewards()
    {
        LevelRewards rewards = GameManager.instance.levelData.levelRewards;
        int coinReward = Mathf.RoundToInt((rewards.baseCoinReward + rewards.coinIncreasePerLevel * currentLevel) * 0.5f);
        
        CurrencyManager.instance.AddCurrency("coin", coinReward);
    }

    public void StartNextLevel()
    {
        int nextLevel = GameManager.instance.currentSave.currentDisplayLevel;
        StartBattle(nextLevel);
    }

    public void ReturnToMainMenu()
    {
        FormationManager.instance.ClearAllUnits();
        UIManager.instance.ShowMainMenu();
    }

    public void RetryBattle()
    {
        FormationManager.instance.ClearAllUnits();
        StartBattle(currentLevel);
    }

    public void ExitBattle()
    {
        FormationManager.instance.ClearAllUnits();
        currentPhase = BattlePhase.END;
        battleResult = BattleResult.NONE;
        ReturnToMainMenu();
    }

    public int GetRemainingPlayerUnits()
    {
        return playerUnits.Count;
    }

    public int GetRemainingEnemyUnits()
    {
        return enemyUnits.Count;
    }
}
