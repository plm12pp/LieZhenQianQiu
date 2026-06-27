using UnityEngine;
using System.Collections.Generic;

public enum GridCellType
{
    EMPTY,
    PLAYER_UNIT,
    ENEMY_UNIT,
    RIVER,
    FERRY
}

public enum GridRegion
{
    ENEMY,
    RIVER,
    PLAYER
}

public class BattleGrid : MonoBehaviour
{
    public int columns = 7;
    public int rows = 11;
    public int playerRows = 5;
    public int enemyRows = 5;
    public int riverRow = 5;

    private GridCellType[,] grid;
    private Dictionary<string, List<Vector2Int>> unitPositions;

    void Awake()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        grid = new GridCellType[columns, rows];
        unitPositions = new Dictionary<string, List<Vector2Int>>();

        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                if (row == riverRow)
                {
                    if (col == 0 || col == columns - 1)
                    {
                        grid[col, row] = GridCellType.FERRY;
                    }
                    else
                    {
                        grid[col, row] = GridCellType.RIVER;
                    }
                }
                else
                {
                    grid[col, row] = GridCellType.EMPTY;
                }
            }
        }
    }

    public GridRegion GetRegion(int row)
    {
        if (row < riverRow)
            return GridRegion.ENEMY;
        else if (row == riverRow)
            return GridRegion.RIVER;
        else
            return GridRegion.PLAYER;
    }

    public bool IsValidPosition(int col, int row)
    {
        return col >= 0 && col < columns && row >= 0 && row < rows;
    }

    public bool CanPlaceUnit(int col, int row, int size, bool isHorizontal, bool isPlayer)
    {
        List<Vector2Int> cells = GetUnitCells(col, row, size, isHorizontal);
        
        foreach (var cell in cells)
        {
            if (!IsValidPosition(cell.x, cell.y))
                return false;
            
            if (grid[cell.x, cell.y] != GridCellType.EMPTY)
                return false;
            
            GridRegion region = GetRegion(cell.y);
            if (isPlayer && region != GridRegion.PLAYER)
                return false;
            if (!isPlayer && region != GridRegion.ENEMY)
                return false;
        }
        
        return true;
    }

    public List<Vector2Int> GetUnitCells(int col, int row, int size, bool isHorizontal)
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        
        if (isHorizontal)
        {
            for (int i = 0; i < size; i++)
            {
                cells.Add(new Vector2Int(col + i, row));
            }
        }
        else
        {
            for (int i = 0; i < size; i++)
            {
                cells.Add(new Vector2Int(col, row + i));
            }
        }
        
        return cells;
    }

    public bool PlaceUnit(string unitId, int col, int row, int size, bool isHorizontal, bool isPlayer)
    {
        if (!CanPlaceUnit(col, row, size, isHorizontal, isPlayer))
            return false;

        List<Vector2Int> cells = GetUnitCells(col, row, size, isHorizontal);
        
        foreach (var cell in cells)
        {
            grid[cell.x, cell.y] = isPlayer ? GridCellType.PLAYER_UNIT : GridCellType.ENEMY_UNIT;
        }
        
        unitPositions[unitId] = cells;
        return true;
    }

    public void RemoveUnit(string unitId)
    {
        if (unitPositions.TryGetValue(unitId, out List<Vector2Int> cells))
        {
            foreach (var cell in cells)
            {
                grid[cell.x, cell.y] = GridCellType.EMPTY;
            }
            unitPositions.Remove(unitId);
        }
    }

    public bool CanMoveTo(int col, int row)
    {
        if (!IsValidPosition(col, row))
            return false;
        
        GridCellType cellType = grid[col, row];
        return cellType == GridCellType.EMPTY || cellType == GridCellType.FERRY;
    }

    public int GetDistance(Vector2Int from, Vector2Int to)
    {
        int dx = Mathf.Abs(from.x - to.x);
        int dy = Mathf.Abs(from.y - to.y);
        return dx + dy;
    }

    public Vector2Int GetFerryPosition(bool left)
    {
        return new Vector2Int(left ? 0 : columns - 1, riverRow);
    }

    public int GetPlayerAreaRows()
    {
        return playerRows;
    }

    public int GetEnemyAreaRows()
    {
        return enemyRows;
    }

    public int GetTotalCells()
    {
        return columns * rows;
    }

    public int GetEmptyCellsCount(bool isPlayer)
    {
        int count = 0;
        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                if (grid[col, row] == GridCellType.EMPTY)
                {
                    GridRegion region = GetRegion(row);
                    if ((isPlayer && region == GridRegion.PLAYER) ||
                        (!isPlayer && region == GridRegion.ENEMY))
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }
}
