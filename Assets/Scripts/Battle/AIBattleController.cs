using UnityEngine;
using System.Collections.Generic;

public class AIBattleController : MonoBehaviour
{
    public static AIBattleController instance;

    public BattleGrid battleGrid;
    public FormationManager formationManager;
    
    private List<string> availableTroops;
    private Dictionary<string, List<string>> counterPreferences;
    
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
        
        InitializeCounterPreferences();
    }

    void InitializeCounterPreferences()
    {
        counterPreferences = new Dictionary<string, List<string>>
        {
            { "cavalry", new List<string> { "troop_spear", "troop_yuejiajun", "troop_modao_dui" } },
            { "archer", new List<string> { "troop_shield", "troop_heavy_cavalry", "troop_light_cavalry" } },
            { "shield", new List<string> { "troop_crossbow", "troop_catapult", "troop_tiger_cannon" } },
            { "ranged", new List<string> { "troop_light_cavalry", "troop_heavy_cavalry", "troop_bai_mayi" } },
            { "artillery", new List<string> { "troop_light_cavalry", "troop_xiliang_cavalry" } },
            { "large", new List<string> { "troop_crossbow", "troop_tiger_cannon", "troop_divine_cannon" } },
            { "infantry", new List<string> { "troop_axeman", "troop_qin_ruishi", "troop_trap_camp" } }
        };
    }

    public void GenerateEnemyFormation()
    {
        availableTroops = GetAvailableTroopsForAI();
        formationManager.enemyPlacedUnits.Clear();
        
        int enemyCount = GameManager.instance.GetEnemyCountForLevel(BattleManager.instance.currentLevel);
        AnalyzePlayerFormation();
        
        for (int i = 0; i < enemyCount; i++)
        {
            PlaceEnemyUnit(i, enemyCount);
        }
        
        formationManager.SpawnEnemyUnits();
    }

    List<string> GetAvailableTroopsForAI()
    {
        List<string> troops = new List<string>();
        foreach (var troop in GameManager.instance.troopsData.troops)
        {
            troops.Add(troop.id);
        }
        return troops;
    }

    void AnalyzePlayerFormation()
    {
        
    }

    void PlaceEnemyUnit(int index, int totalCount)
    {
        string troopId = SelectTroopForPosition(index, totalCount);
        TroopData troopData = GameManager.instance.GetTroopData(troopId);
        
        if (troopData == null)
            return;

        Vector2Int position = FindValidPosition(troopData.size);
        if (position.x < 0)
            return;

        bool isHorizontal = Random.value > 0.5f;
        
        string instanceId = $"enemy_{troopId}_{Time.time}_{index}";
        bool placed = battleGrid.PlaceUnit(instanceId, position.x, position.y, troopData.size, isHorizontal, false);
        
        if (placed)
        {
            int star = CalculateEnemyStar();
            
            PlacedUnit placedUnit = new PlacedUnit
            {
                instanceId = instanceId,
                troopId = troopId,
                col = position.x,
                row = position.y,
                size = troopData.size,
                isHorizontal = isHorizontal,
                star = star
            };
            
            formationManager.enemyPlacedUnits.Add(placedUnit);
        }
    }

    string SelectTroopForPosition(int index, int totalCount)
    {
        float random = Random.value;
        
        if (random < 0.4f)
        {
            return GetRandomTroopBySize(1);
        }
        else if (random < 0.7f)
        {
            return GetRandomTroopBySize(2);
        }
        else
        {
            return GetRandomTroopBySize(3);
        }
    }

    string GetRandomTroopBySize(int size)
    {
        List<string> filtered = availableTroops.FindAll(id => 
        {
            TroopData data = GameManager.instance.GetTroopData(id);
            return data != null && data.size == size;
        });
        
        if (filtered.Count == 0)
        {
            return availableTroops[Random.Range(0, availableTroops.Count)];
        }
        
        return filtered[Random.Range(0, filtered.Count)];
    }

    Vector2Int FindValidPosition(int size)
    {
        List<Vector2Int> possiblePositions = new List<Vector2Int>();
        
        for (int row = 0; row < battleGrid.GetEnemyAreaRows(); row++)
        {
            for (int col = 0; col < battleGrid.columns; col++)
            {
                if (battleGrid.CanPlaceUnit(col, row, size, true, false))
                {
                    possiblePositions.Add(new Vector2Int(col, row));
                }
                if (battleGrid.CanPlaceUnit(col, row, size, false, false))
                {
                    possiblePositions.Add(new Vector2Int(col, row));
                }
            }
        }
        
        if (possiblePositions.Count == 0)
            return new Vector2Int(-1, -1);
        
        return possiblePositions[Random.Range(0, possiblePositions.Count)];
    }

    int CalculateEnemyStar()
    {
        float difficulty = BattleManager.instance.difficultyMultiplier;
        
        if (difficulty < 1.2f)
            return 1;
        else if (difficulty < 1.5f)
            return Random.Range(1, 3);
        else if (difficulty < 2.0f)
            return Random.Range(2, 4);
        else
            return Random.Range(2, 5);
    }

    void Redeploy()
    {
        formationManager.ClearAllUnits();
        GenerateEnemyFormation();
    }
}
