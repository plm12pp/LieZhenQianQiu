using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;
    
    public int currency  // 提供便捷访问金币属性
    {
        get { return GetCurrency("coin"); }
    }

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

    public void AddCurrency(string type, int amount)
    {
        switch (type)
        {
            case "coin":
                GameManager.instance.currentSave.currencies.coin += amount;
                break;
            case "token":
                GameManager.instance.currentSave.currencies.token += amount;
                break;
            case "jade":
                GameManager.instance.currentSave.currencies.jade += amount;
                break;
        }
        GameManager.instance.SaveGame();
    }

    public bool SpendCurrency(string type, int amount)
    {
        bool success = false;
        
        switch (type)
        {
            case "coin":
                if (GameManager.instance.currentSave.currencies.coin >= amount)
                {
                    GameManager.instance.currentSave.currencies.coin -= amount;
                    success = true;
                }
                break;
            case "token":
                if (GameManager.instance.currentSave.currencies.token >= amount)
                {
                    GameManager.instance.currentSave.currencies.token -= amount;
                    success = true;
                }
                break;
            case "jade":
                if (GameManager.instance.currentSave.currencies.jade >= amount)
                {
                    GameManager.instance.currentSave.currencies.jade -= amount;
                    success = true;
                }
                break;
        }
        
        if (success)
        {
            GameManager.instance.SaveGame();
        }
        
        return success;
    }

    public int GetCurrency(string type)
    {
        switch (type)
        {
            case "coin":
                return GameManager.instance.currentSave.currencies.coin;
            case "token":
                return GameManager.instance.currentSave.currencies.token;
            case "jade":
                return GameManager.instance.currentSave.currencies.jade;
            default:
                return 0;
        }
    }
}
