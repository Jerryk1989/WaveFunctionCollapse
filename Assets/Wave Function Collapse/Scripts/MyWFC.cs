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
        Debug.LogWarning("***************************************************************");
        Debug.LogWarning("STARTING NEW CREATION SEQUENCE");
        Debug.LogWarning("***************************************************************");
        cells = new List<Cell>();
        
        //Create the grid and populate each spot with a cell.
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
        ReduceNeighborCellTileOptions(currentCell, currentTile);

        while (cells.Any(x => !x.isCollapsed))
        {
            currentCell = GetNextCellWithLowestOptions();
            currentTile = PickTileAndCollapseCell(currentCell);
            ReduceNeighborCellTileOptions(currentCell, currentTile);
        }
        
        CleanUpCells();
    }

    private void LogCurrentTile(Tile tile, Cell cell)
    {
        var items = string.Join(", ", cell.tileOptions);
        Debug.LogWarning($"Current tile being placed is {tile} at x: {cell.transform.position.x} and z: {cell.transform.position.z}.  It's tile choices were {items}.");
    }
    
    private void LogTileOptionsForTile(Tile tile, List<Tile> tileOptions, string direction)
    {
        var items = string.Join(", ", tileOptions);
        Debug.LogWarning($"The Tile {direction} has tileOptions of {items}.  This decision is coming from {tile}.");
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

        LogCurrentTile(chosenTile, currentCell);
            
        return chosenTile;
    }
    
    private void ReduceNeighborCellTileOptions(Cell currentCell, Tile currentTile)
    {
        List<Cell> neighborCellsForCurrentCell = new List<Cell>();
        var currentCellPosition = currentCell.transform.position;
        
        //North
        Cell northCell = GetCellAtPosition(currentCellPosition.x, currentCellPosition.z + tileXAndZSize);
        Cell southCell = GetCellAtPosition(currentCellPosition.x, currentCellPosition.z - tileXAndZSize);
        Cell eastCell = GetCellAtPosition(currentCellPosition.x + tileXAndZSize, currentCellPosition.z);
        Cell westCell = GetCellAtPosition(currentCellPosition.x - tileXAndZSize, currentCellPosition.z);
    
        if (northCell != null && !northCell.isCollapsed && CheckIfCellIsWithinGrid(northCell))
        {
            ReduceTileOptions(northCell, currentCell.tileOptions[0].North);
            LogTileOptionsForTile(currentTile, northCell.tileOptions, "north");
        }
    
        if (southCell != null && !southCell.isCollapsed&& CheckIfCellIsWithinGrid(southCell))
        {
            ReduceTileOptions(southCell, currentCell.tileOptions[0].North);
            LogTileOptionsForTile(currentTile, southCell.tileOptions, "south");
        }
    
        if (eastCell != null && !eastCell.isCollapsed&& CheckIfCellIsWithinGrid(eastCell))
        {
            ReduceTileOptions(eastCell, currentCell.tileOptions[0].North);
            LogTileOptionsForTile(currentTile, eastCell.tileOptions, "east");
        }
        
        if (westCell != null && !westCell.isCollapsed&& CheckIfCellIsWithinGrid(westCell))
        {
            ReduceTileOptions(westCell, currentCell.tileOptions[0].North);
            LogTileOptionsForTile(currentTile, westCell.tileOptions, "west");
        }
    }

    // private void ReduceTileOptions(Cell tileBeingReduced, List<Tile> tileOptionsFromParentCell)
    // {
    //     List<Tile> validOptions = tileOptionsFromParentCell;
    //
    //     tileBeingReduced.tileOptions = validOptions;
    // }
    
    private void ReduceTileOptions(Cell cell, Tile currentTile)
    {
        // Get the direct neighbor cells of the current cell
        List<Cell> neighborCells = GetDirectNeighborCells(cell);

        // Create a list to store the valid options after reduction
        List<Tile> validOptions = new List<Tile>();

        foreach (var neighborCell in neighborCells)
        {
            foreach (Tile option in cell.tileOptions)
            {
                Vector3 direction = (neighborCell.transform.position - cell.transform.position).normalized;
                
                // Check if the option is compatible with the current tile in the specified direction
                if (IsOptionCompatibleWithTile(option, currentTile, neighborCell, direction))
                {
                    // Check if the option is compatible with the neighboring cells
                    if (AreOptionsCompatibleWithNeighbors(option, neighborCells))
                    {
                        // If both conditions are met, add the option to the validOptions list
                        validOptions.Add(option);
                    }
                }
            }
        }
        
        // Update the tile options for the current cell
        cell.tileOptions = validOptions;
    }

    private bool IsOptionCompatibleWithTile(Tile option, Tile currentTile, Cell neighborCell, Vector3 direction)
    {
        //This isn't right.
        if (direction == Vector3.forward)
        {
            neighborCell.tileOptions = currentTile.North;
        }
        else if (direction == Vector3.back)
        {
            neighborCell.tileOptions = currentTile.South;
        }
        else if (direction == Vector3.right)
        {
            neighborCell.tileOptions = currentTile.East;
        } 
        else if (direction == Vector3.left)
        {
            neighborCell.tileOptions = currentTile.West;
        }
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
        
        if (cell == null && CheckIfCellIsWithinGrid(xAxis, zAxis))
        {
            Debug.LogError($"Could not find a cell at x position {xAxis} and z position {zAxis}");
        }

        return cell;;
    }

    private bool CheckIfCellIsWithinGrid(float xAxis, float zAxis)
    {
        if (xAxis > gridWidth || zAxis > gridHeight)
            return false;

        return true;
    }
    
    private bool CheckIfCellIsWithinGrid(Cell cell)
    {
        if (cell.transform.position.x > gridWidth || cell.transform.position.z > gridHeight)
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


//Starting places a tile and updates the tile options for 1 and 2.

//1 and 2 both have a tile count of 1.

//1 is chosen as the next tile to collapse.

//1 collapses and then reduced tile 3 options to something that has 2 tile counts that are valid to touch 1, but maybe not 2.

//now we move to 2 and place the only valid option we had.  3 is then reduced again and it's tileoptions are now whatever 2 says they should be regardless of what 1 wants.

//In order for 3 to work, it needs to find a common ground between 2 and 1.