using UnityEngine;

public class LifeSystem : MonoBehaviour
{
    public static LifeSystem instance;

    public int playerMaxLives;
    public int enemyMaxLives;
    
    private int playerCurrentLives;
    private int enemyCurrentLives;
    
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
        
        playerMaxLives = GameManager.instance.GetMaxLives();
        enemyMaxLives = GameManager.instance.GetMaxLives();
    }

    public void InitializeLives()
    {
        playerCurrentLives = playerMaxLives;
        enemyCurrentLives = enemyMaxLives;
        
        UIManager.instance.UpdatePlayerLives(playerCurrentLives);
        UIManager.instance.UpdateEnemyLives(enemyCurrentLives);
    }

    public void RemovePlayerLife()
    {
        playerCurrentLives--;
        UIManager.instance.UpdatePlayerLives(playerCurrentLives);
        
        if (playerCurrentLives <= 0)
        {
            BattleManager.instance.EndBattle();
        }
    }

    public void RemoveEnemyLife()
    {
        enemyCurrentLives--;
        UIManager.instance.UpdateEnemyLives(enemyCurrentLives);
        
        if (enemyCurrentLives <= 0)
        {
            BattleManager.instance.EndBattle();
        }
    }

    public bool CanPlayerContinue()
    {
        return playerCurrentLives > 0;
    }

    public bool CanEnemyContinue()
    {
        return enemyCurrentLives > 0;
    }

    public int GetPlayerLives()
    {
        return playerCurrentLives;
    }

    public int GetEnemyLives()
    {
        return enemyCurrentLives;
    }

    public float GetPlayerLifePercentage()
    {
        return (float)playerCurrentLives / playerMaxLives;
    }

    public float GetEnemyLifePercentage()
    {
        return (float)enemyCurrentLives / enemyMaxLives;
    }
}
