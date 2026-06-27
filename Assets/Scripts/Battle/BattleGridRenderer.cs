using UnityEngine;
using System.Collections.Generic;

public class BattleGridRenderer : MonoBehaviour
{
    public BattleGrid battleGrid;
    public GameObject cellPrefab;
    public Transform gridParent;
    
    public Color playerAreaColor = new Color32(0x3B, 0x82, 0xF6, 0x33);
    public Color enemyAreaColor = new Color32(0xEF, 0x44, 0x44, 0x33);
    public Color riverColor = new Color32(0x6B, 0x72, 0x80, 0x66);
    public Color ferryColor = new Color32(0x8B, 0x45, 0x13, 0x66);
    public Color gridLineColor = new Color32(0x5D, 0x40, 0x37, 0x99);
    public Color highlightColor = new Color32(0x10, 0xB9, 0x81, 0x66);
    public Color invalidColor = new Color32(0xEF, 0x44, 0x44, 0x66);
    
    private GridCellUI[,] cellUIs;
    private List<BattleUnitUI> unitUIs;
    
    void Start()
    {
        CreateGrid();
        unitUIs = new List<BattleUnitUI>();
    }
    
    void CreateGrid()
    {
        cellUIs = new GridCellUI[battleGrid.columns, battleGrid.rows];
        
        for (int row = 0; row < battleGrid.rows; row++)
        {
            for (int col = 0; col < battleGrid.columns; col++)
            {
                GameObject cellObj = Instantiate(cellPrefab, gridParent);
                cellObj.name = $"Cell_{col}_{row}";
                cellObj.transform.localPosition = new Vector3(col, row, 0);
                
                GridCellUI cellUI = cellObj.GetComponent<GridCellUI>();
                if (cellUI == null)
                    cellUI = cellObj.AddComponent<GridCellUI>();
                
                cellUI.column = col;
                cellUI.row = row;
                cellUI.gridRenderer = this;
                
                SetCellAppearance(cellUI, col, row);
                cellUIs[col, row] = cellUI;
            }
        }
    }
    
    void SetCellAppearance(GridCellUI cell, int col, int row)
    {
        GridRegion region = battleGrid.GetRegion(row);
        
        if (region == GridRegion.RIVER)
        {
            if (col == 0 || col == battleGrid.columns - 1)
            {
                cell.SetBackgroundColor(ferryColor);
                cell.SetBorderColor(gridLineColor);
            }
            else
            {
                cell.SetBackgroundColor(riverColor);
                cell.SetBorderColor(gridLineColor);
            }
        }
        else if (region == GridRegion.PLAYER)
        {
            cell.SetBackgroundColor(playerAreaColor);
            cell.SetBorderColor(gridLineColor);
        }
        else
        {
            cell.SetBackgroundColor(enemyAreaColor);
            cell.SetBorderColor(gridLineColor);
        }
    }
    
    public void HighlightPlacement(int col, int row, int size, bool isHorizontal, bool valid)
    {
        ClearHighlights();
        
        List<Vector2Int> cells = battleGrid.GetUnitCells(col, row, size, isHorizontal);
        
        foreach (var cell in cells)
        {
            if (cell.x >= 0 && cell.x < battleGrid.columns && 
                cell.y >= 0 && cell.y < battleGrid.rows)
            {
                cellUIs[cell.x, cell.y].SetHighlight(valid ? highlightColor : invalidColor);
            }
        }
    }
    
    public void ClearHighlights()
    {
        for (int row = 0; row < battleGrid.rows; row++)
        {
            for (int col = 0; col < battleGrid.columns; col++)
            {
                cellUIs[col, row].ClearHighlight();
            }
        }
    }
    
    public void UpdateUnitDisplay()
    {
        foreach (var unitUI in unitUIs)
        {
            if (unitUI != null)
                Destroy(unitUI.gameObject);
        }
        unitUIs.Clear();
    }
    
    public void AddUnitUI(UnitController unit)
    {
        GameObject unitObj = new GameObject($"UnitUI_{unit.unitId}");
        unitObj.transform.SetParent(gridParent);
        
        BattleUnitUI unitUI = unitObj.AddComponent<BattleUnitUI>();
        unitUI.AttachToUnit(unit);
        
        unitUIs.Add(unitUI);
    }
    
    public GridCellUI GetCellUI(int col, int row)
    {
        if (col >= 0 && col < battleGrid.columns && row >= 0 && row < battleGrid.rows)
        {
            return cellUIs[col, row];
        }
        return null;
    }
}

public class GridCellUI : MonoBehaviour
{
    public int column;
    public int row;
    public BattleGridRenderer gridRenderer;
    
    public Image backgroundImage;
    public Image borderImage;
    public Image highlightImage;
    public Button cellButton;
    
    private Color originalColor;
    
    public void SetBackgroundColor(Color color)
    {
        originalColor = color;
        if (backgroundImage != null)
            backgroundImage.color = color;
    }
    
    public void SetBorderColor(Color color)
    {
        if (borderImage != null)
            borderImage.color = color;
    }
    
    public void SetHighlight(Color color)
    {
        if (highlightImage != null)
        {
            highlightImage.color = color;
            highlightImage.gameObject.SetActive(true);
        }
    }
    
    public void ClearHighlight()
    {
        if (highlightImage != null)
        {
            highlightImage.gameObject.SetActive(false);
        }
    }
    
    void OnMouseEnter()
    {
        if (FormationManager.instance != null)
        {
        }
    }
    
    void OnMouseExit()
    {
        if (gridRenderer != null)
        {
            gridRenderer.ClearHighlights();
        }
    }
    
    void OnMouseDown()
    {
        if (FormationManager.instance != null)
        {
        }
    }
}
