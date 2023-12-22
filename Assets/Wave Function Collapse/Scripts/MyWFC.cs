using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Wave_Function_Collapse.Scripts;
using Random = UnityEngine.Random;

public class MyWFC : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;
    public Cell cellObject;
    public int tileXAndZSize = 10;
    public List<Tile> tiles;

    private List<Cell> cells;
    private List<GameObject> objectsInstantiated = new List<GameObject>();
    private List<Cell> insantiatedCells = new List<Cell>();
    
    public void TestingWaveFunctionCollapse()
    {
        foreach (var item in objectsInstantiated)
        {
            DestroyImmediate(item);
        }
        
        foreach (var item in insantiatedCells)
        {
            DestroyImmediate(item);
        }
            
        objectsInstantiated.Clear();
        insantiatedCells.Clear();
            
        Start();
    }
    
    public void Start()
    {
        cells = new List<Cell>();
        
        for (int x = 0; x <= gridWidth; x+=tileXAndZSize)
        {
            for (int z = 0; z <= gridHeight; z+=tileXAndZSize)
            {
                Cell newCell = Instantiate(cellObject, new Vector3(x, 0f, z), Quaternion.identity);
                newCell.CreateCell(false, tiles);
                cells.Add(newCell);
                insantiatedCells.Add(newCell);
            }
        }
        
        Cell currentCell = GetNextCellWithLowestOptions();
        Tile currentTile = PickTileAndCollapseCell(currentCell);
        ReduceTileOptions(currentCell, currentTile);
        
        while (cells.Any(x => !x.isCollapsed))
        {
            currentCell = GetNextCellWithLowestOptions();
            currentTile = PickTileAndCollapseCell(currentCell);
            ReduceTileOptions(currentCell, currentTile);
        }
        
        CleanUpCells();
    }

    private void CleanUpCells()
    {
        foreach (var cell in cells)
        {
            Destroy(cell);
        }
    }

    private Tile PickTileAndCollapseCell(Cell currentCell)
    {
        Tile chosenTile = currentCell.tileOptions[Random.Range(0, currentCell.tileOptions.Count)];
        objectsInstantiated.Add(Instantiate(chosenTile.Prefab, currentCell.transform.position, chosenTile.Prefab.transform.rotation));

        currentCell.tileOptions = new List<Tile>() {chosenTile};
        currentCell.isCollapsed = true;

        return chosenTile;
    }
    
    private void ReduceTileOptions(Cell cell, Tile currentTile)
    {
        List<Cell> neighborCells = GetDirectNeighborCells(cell);
        Debug.LogWarning($"ParentCell: {cell.name}.  ParentCell Location: X: {cell.transform.position.x}, Z: {cell.transform.position.z}.  Parent Cell choice: {currentTile.name}.");
        
        foreach (var neighborCell in neighborCells)
        {
            List<Tile> validOptions = new List<Tile>();
            Vector3 direction = (neighborCell.transform.position - cell.transform.position).normalized;
            
            validOptions = CheckTileCompatability(validOptions, neighborCell, currentTile, direction);
            
            neighborCell.tileOptions = validOptions;
        }
    }

    private List<Tile> CheckTileCompatability(List<Tile> validOptions, Cell neighborCell, Tile currentTile, Vector3 direction)
    {
        foreach (Tile option in neighborCell.tileOptions)
        {
            if (IsOptionCompatibleWithTile(option, currentTile, direction))
            {
                validOptions.Add(option);
            }
        }
        
        if(validOptions.Any() == false)
            Debug.LogError($"No valid options.  Current Tile: {currentTile}.  Neighbor cell options: {string.Join(", ", neighborCell.tileOptions)}.  Neighbor direction: {direction}");

        return validOptions;
    }

    private bool IsOptionCompatibleWithTile(Tile option, Tile currentTile, Vector3 direction)
    {
        if (direction == Vector3.forward)
        {
            if (currentTile.North.Contains(option))
                return true;

            return false;
        }

        if (direction == Vector3.back)
        {
            if (currentTile.South.Contains(option))
                return true;

            return false;
        }

        if (direction == Vector3.right)
        {
            if (currentTile.East.Contains(option))
                return true;

            return false;
        }

        if (direction == Vector3.left)
        {
            if (currentTile.West.Contains(option))
                return true;

            return false;
        }
        
        Debug.LogError("There was no directional option found for the tile.");
        return false;
    }

    private List<Cell> GetDirectNeighborCells(Cell cell)
    {
        List<Cell> neighborCells = new List<Cell>();

        var currentCellPosition = cell.transform.position;
        
        neighborCells.Add(GetCellAtPosition(currentCellPosition.x, currentCellPosition.z + tileXAndZSize));
        neighborCells.Add(GetCellAtPosition(currentCellPosition.x, currentCellPosition.z - tileXAndZSize));
        neighborCells.Add(GetCellAtPosition(currentCellPosition.x + tileXAndZSize, currentCellPosition.z));
        neighborCells.Add(GetCellAtPosition(currentCellPosition.x - tileXAndZSize, currentCellPosition.z));

        neighborCells = neighborCells.Where(c => c != null).ToList();

        return neighborCells;
    }

    private Cell GetCellAtPosition(float xAxis, float zAxis, bool? isCollapsed = null)
    {
        Cell cell = null;
        
        if(isCollapsed == null)
            cell = cells.Where(x => x.transform.position.x == xAxis && x.transform.position.z == zAxis).FirstOrDefault();
        else
            cell = cells.Where(x => x.transform.position.x == xAxis && x.transform.position.z == zAxis && x.isCollapsed == isCollapsed).FirstOrDefault();

        if (!CheckIfCellIsWithinGrid(xAxis, zAxis))
            cell = null;

        return cell;;
    }

    private bool CheckIfCellIsWithinGrid(float xAxis, float zAxis)
    {
        if (xAxis > gridWidth || zAxis > gridHeight)
            return false;

        return true;
    }

    private Cell GetNextCellWithLowestOptions()
    {
        var cellsThatAreNotCollapsed = cells.Where(x => !x.isCollapsed).ToList();
        var cellsSortedByLowestTileOptions = cellsThatAreNotCollapsed.OrderBy(x => x.tileOptions.Count).ToList();
        var cellsWithLowestTileOptions = cellsSortedByLowestTileOptions.Where(x => x.tileOptions.Count == cellsSortedByLowestTileOptions[0].tileOptions.Count).ToList();

        return cellsWithLowestTileOptions[Random.Range(0, cellsWithLowestTileOptions.Count)];
    }
}