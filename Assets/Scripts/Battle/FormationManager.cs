using UnityEngine;
using System.Collections.Generic;

public enum FormationType
{
    NONE,
    CRANE,
    FISH_SCALE,
    GOOSE,
    CIRCLE,
    SQUARE,
    LONG_SNAKE,
    HEAVEN_AND_EARTH
}

public class FormationManager : MonoBehaviour
{
    public static FormationManager instance;

    public BattleGrid battleGrid;
    public Transform playerUnitsParent;
    public Transform enemyUnitsParent;
    
    public List<PlacedUnit> playerPlacedUnits;
    public List<PlacedUnit> enemyPlacedUnits;
    
    public FormationType playerFormation;
    public FormationType enemyFormation;
    
    public bool useReserveTroops;
    
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
        
        playerPlacedUnits = new List<PlacedUnit>();
        enemyPlacedUnits = new List<PlacedUnit>();
        playerFormation = FormationType.NONE;
        enemyFormation = FormationType.NONE;
    }

    public bool PlacePlayerUnit(string troopId, int col, int row, bool isHorizontal)
    {
        TroopData troopData = GameManager.instance.GetTroopData(troopId);
        if (troopData == null)
            return false;

        if (!battleGrid.CanPlaceUnit(col, row, troopData.size, isHorizontal, true))
            return false;

        InventoryItem item;
        if (!GameManager.instance.currentSave.inventory.TryGetValue(troopId, out item))
            return false;

        if (item.count <= 0)
            return false;

        string instanceId = $"{troopId}_{Time.time}_{Random.value}";
        
        bool placed = battleGrid.PlaceUnit(instanceId, col, row, troopData.size, isHorizontal, true);
        
        if (placed)
        {
            item.count--;
            
            PlacedUnit placedUnit = new PlacedUnit
            {
                instanceId = instanceId,
                troopId = troopId,
                col = col,
                row = row,
                size = troopData.size,
                isHorizontal = isHorizontal,
                star = item.star,
                isReserve = false
            };
            
            playerPlacedUnits.Add(placedUnit);
            UIManager.instance.UpdateFormationUI();
            
            GameManager.instance.SaveGame();
        }
        
        return placed;
    }

    public void PlaceReserveTroops()
    {
        int needed = 5 - playerPlacedUnits.Count;
        if (needed <= 0) return;
        
        string[] reserveTroops = { "troop_knife", "troop_spear", "troop_archer", "troop_shield", "troop_light_cavalry" };
        
        for (int i = 0; i < needed; i++)
        {
            string reserveTroopId = reserveTroops[i % reserveTroops.Length];
            PlaceReserveTroop(reserveTroopId);
        }
    }

    void PlaceReserveTroop(string troopId)
    {
        TroopData troopData = GameManager.instance.GetTroopData(troopId);
        if (troopData == null) return;
        
        Vector2Int position = FindRandomEmptyPosition(true, troopData.size);
        if (position.x < 0) return;
        
        bool isHorizontal = Random.value > 0.5f;
        string instanceId = $"reserve_{troopId}_{Time.time}_{Random.value}";
        
        bool placed = battleGrid.PlaceUnit(instanceId, position.x, position.y, troopData.size, isHorizontal, true);
        
        if (placed)
        {
            PlacedUnit placedUnit = new PlacedUnit
            {
                instanceId = instanceId,
                troopId = troopId,
                col = position.x,
                row = position.y,
                size = troopData.size,
                isHorizontal = isHorizontal,
                star = 1,
                isReserve = true
            };
            
            playerPlacedUnits.Add(placedUnit);
        }
    }

    Vector2Int FindRandomEmptyPosition(bool isPlayer, int size)
    {
        List<Vector2Int> possiblePositions = new List<Vector2Int>();
        
        int startRow = isPlayer ? battleGrid.riverRow + 1 : 0;
        int endRow = isPlayer ? battleGrid.rows : battleGrid.riverRow;
        
        for (int row = startRow; row < endRow; row++)
        {
            for (int col = 0; col < battleGrid.columns; col++)
            {
                if (battleGrid.CanPlaceUnit(col, row, size, true, isPlayer))
                {
                    possiblePositions.Add(new Vector2Int(col, row));
                }
                if (battleGrid.CanPlaceUnit(col, row, size, false, isPlayer))
                {
                    possiblePositions.Add(new Vector2Int(col, row));
                }
            }
        }
        
        if (possiblePositions.Count == 0)
            return new Vector2Int(-1, -1);
        
        return possiblePositions[Random.Range(0, possiblePositions.Count)];
    }

    public void RemovePlayerUnit(string instanceId)
    {
        PlacedUnit unit = playerPlacedUnits.Find(u => u.instanceId == instanceId);
        if (unit == null)
            return;

        battleGrid.RemoveUnit(instanceId);
        
        if (!unit.isReserve)
        {
            InventoryItem item;
            if (GameManager.instance.currentSave.inventory.TryGetValue(unit.troopId, out item))
            {
                item.count++;
            }
        }
        
        playerPlacedUnits.Remove(unit);
        UIManager.instance.UpdateFormationUI();
        
        if (!unit.isReserve)
        {
            GameManager.instance.SaveGame();
        }
    }

    public void SpawnPlayerUnits()
    {
        if (playerPlacedUnits.Count < 5 && useReserveTroops)
        {
            PlaceReserveTroops();
        }
        
        foreach (var placedUnit in playerPlacedUnits)
        {
            SpawnUnit(placedUnit, UnitSide.PLAYER);
        }
    }

    public void SpawnEnemyUnits()
    {
        foreach (var placedUnit in enemyPlacedUnits)
        {
            SpawnUnit(placedUnit, UnitSide.ENEMY);
        }
    }

    void SpawnUnit(PlacedUnit placedUnit, UnitSide side)
    {
        TroopData troopData = GameManager.instance.GetTroopData(placedUnit.troopId);
        if (troopData == null)
            return;

        float bonus = GameManager.instance.GetStarBonus(placedUnit.star);
        float difficultyBonus = side == UnitSide.ENEMY ? BattleManager.instance.difficultyMultiplier : 1.0f;
        float formationBonus = GetFormationBonus(side, troopData);
        
        GameObject unitObj = new GameObject(troopData.name);
        unitObj.transform.SetParent(side == UnitSide.PLAYER ? playerUnitsParent : enemyUnitsParent);
        unitObj.transform.position = new Vector3(placedUnit.col * 1.0f, placedUnit.row * 1.0f, 0);
        
        UnitController controller = unitObj.AddComponent<UnitController>();
        controller.unitId = placedUnit.instanceId;
        controller.troopDataId = placedUnit.troopId;
        controller.side = side;
        controller.size = troopData.size;
        controller.isHorizontal = placedUnit.isHorizontal;
        controller.position = new Vector2Int(placedUnit.col, placedUnit.row);
        controller.star = placedUnit.star;
        
        controller.maxHp = Mathf.RoundToInt(troopData.hp * bonus * difficultyBonus * formationBonus);
        controller.currentHp = controller.maxHp;
        controller.atk = Mathf.RoundToInt(troopData.atk * bonus * difficultyBonus * formationBonus);
        controller.range = troopData.range;
        controller.speed = troopData.speed;
        controller.armor = Mathf.RoundToInt(troopData.armor * bonus);
        
        controller.skillName = troopData.skillName;
        controller.skillDescription = troopData.skillDescription;
        controller.skillCooldown = troopData.skillCooldown;
        
        if (!string.IsNullOrEmpty(troopData.skillEffect))
        {
            try
            {
                // 将小写的skillEffect转换为UPPER_CASE以匹配枚举
                string effectName = troopData.skillEffect.ToUpper();
                controller.skillEffectType = (SkillEffectType)System.Enum.Parse(typeof(SkillEffectType), effectName);
            }
            catch
            {
                controller.skillEffectType = SkillEffectType.NONE;
            }
        }
        else
        {
            controller.skillEffectType = SkillEffectType.NONE;
        }
        
        if (side == UnitSide.PLAYER)
        {
            BattleManager.instance.playerUnits.Add(controller);
        }
        else
        {
            BattleManager.instance.enemyUnits.Add(controller);
        }
    }

    float GetFormationBonus(UnitSide side, TroopData troopData)
    {
        FormationType formation = side == UnitSide.PLAYER ? playerFormation : enemyFormation;
        
        switch (formation)
        {
            case FormationType.CRANE:
                return 1.05f;
            case FormationType.FISH_SCALE:
                if (troopData.role.Contains("防御") || troopData.role.Contains("重装"))
                    return 1.1f;
                break;
            case FormationType.GOOSE:
                if (troopData.role.Contains("弓") || troopData.role.Contains("弩") || troopData.role.Contains("炮"))
                    return 1.1f;
                break;
            case FormationType.CIRCLE:
                return 1.05f;
            case FormationType.SQUARE:
                if (troopData.role.Contains("防御") || troopData.role.Contains("重装"))
                    return 1.15f;
                break;
            case FormationType.LONG_SNAKE:
                if (troopData.role.Contains("骑") || troopData.speed >= 5)
                    return 1.1f;
                break;
            case FormationType.HEAVEN_AND_EARTH:
                return 1.1f;
        }
        
        return 1.0f;
    }

    public void ClearAllUnits()
    {
        foreach (Transform child in playerUnitsParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in enemyUnitsParent)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var unit in playerPlacedUnits)
        {
            if (!unit.isReserve)
            {
                InventoryItem item;
                if (GameManager.instance.currentSave.inventory.TryGetValue(unit.troopId, out item))
                {
                    item.count++;
                }
            }
        }
        
        playerPlacedUnits.Clear();
        enemyPlacedUnits.Clear();
        BattleManager.instance.playerUnits.Clear();
        BattleManager.instance.enemyUnits.Clear();
        
        battleGrid.InitializeGrid();
        
        GameManager.instance.SaveGame();
    }

    public int GetPlayerUnitCount()
    {
        return playerPlacedUnits.Count;
    }

    public int GetPlayerOccupiedCells()
    {
        int count = 0;
        foreach (var unit in playerPlacedUnits)
        {
            count += unit.size;
        }
        return count;
    }

    public bool CanStartBattle()
    {
        return playerPlacedUnits.Count >= 5;
    }

    public void SetPlayerFormation(FormationType formation)
    {
        playerFormation = formation;
    }

    public FormationInfo GetFormationInfo(FormationType type)
    {
        return formationInfoMap[type];
    }

    static Dictionary<FormationType, FormationInfo> formationInfoMap = new Dictionary<FormationType, FormationInfo>
    {
        { FormationType.NONE, new FormationInfo { name = "无阵型", description = "无特殊加成", icon = "none" } },
        { FormationType.CRANE, new FormationInfo { name = "鹤翼阵", description = "全属性+5%", icon = "crane" } },
        { FormationType.FISH_SCALE, new FormationInfo { name = "鱼鳞阵", description = "防御部队+10%属性", icon = "fish_scale" } },
        { FormationType.GOOSE, new FormationInfo { name = "雁行阵", description = "远程部队+10%伤害", icon = "goose" } },
        { FormationType.CIRCLE, new FormationInfo { name = "方圆阵", description = "全防御+5%", icon = "circle" } },
        { FormationType.SQUARE, new FormationInfo { name = "方阵", description = "重装部队+15%属性", icon = "square" } },
        { FormationType.LONG_SNAKE, new FormationInfo { name = "长蛇阵", description = "骑兵部队+10%速度和伤害", icon = "long_snake" } },
        { FormationType.HEAVEN_AND_EARTH, new FormationInfo { name = "天地阵", description = "全军+10%属性", icon = "heaven_earth" } }
    };
}

public class PlacedUnit
{
    public string instanceId;
    public string troopId;
    public int col;
    public int row;
    public int size;
    public bool isHorizontal;
    public int star;
    public bool isReserve;
}

public class FormationInfo
{
    public string name;
    public string description;
    public string icon;
}
