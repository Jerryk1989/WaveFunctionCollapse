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
    
    public void Start()
    {
        cells = new List<Cell>();
        
        //Create the grid and populate each spot with a cell.
        for (int x = 0; x <= gridWidth; x+=tileXAndZSize)
        {
            for (int z = 0; z <= gridHeight; z+=tileXAndZSize)
            {
                Cell newCell = Instantiate(cellObject, new Vector3(x, 0f, z), Quaternion.identity);
                newCell.CreateCell(false, tiles);
                cells.Add(newCell);
            }
        }
        
        //ChooseAndCreateTheFirstTile();
        
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

    private void ChooseAndCreateTheFirstTile()
    {
        //On the first cell it should just get a random starting cell since they all have the same tileoption count right now.
        Cell currentCell = GetNextCellWithLowestOptions();
        
        //Pick a random valid tile, instantiate it, collapse the cell.
        Tile currentTile = PickTileAndCollapseCell(currentCell);
        
        //GetNeighbor cells
        ReduceNeighborCellTileOptions(currentCell, currentTile);
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
        Instantiate(chosenTile.Prefab, currentCell.transform.position, chosenTile.Prefab.transform.rotation);

        currentCell.isCollapsed = true;

        return chosenTile;
    }

    private void ReduceNeighborCellTileOptions(Cell currentCell, Tile currentTile)
    {
        List<Cell> neighborCellsForCurrentCell = new List<Cell>();
        var currentCellPosition = currentCell.transform.position;
        
        //North
        Cell northCell = GetUncollapsedCellAtPosition(currentCellPosition.x, currentCellPosition.z + tileXAndZSize);
        
        if (northCell != null)
            ReduceTileOptions(northCell, currentTile.North);

        //South
        Cell southCell = GetUncollapsedCellAtPosition(currentCellPosition.x, currentCellPosition.z - tileXAndZSize);
        
        if (southCell != null)
            ReduceTileOptions(southCell, currentTile.South);
        
        //East
        Cell eastCell = GetUncollapsedCellAtPosition(currentCellPosition.x + tileXAndZSize, currentCellPosition.z);
        
        if (eastCell != null)
            ReduceTileOptions(eastCell, currentTile.East);
        
        //West
        Cell westCell = GetUncollapsedCellAtPosition(currentCellPosition.x - tileXAndZSize, currentCellPosition.z);
        
        if (westCell != null)
            ReduceTileOptions(westCell, currentTile.West);
    }

    private void ReduceTileOptions(Cell cell, List<Tile> tileOptions)
    {
        if (cell.tileOptions.Count < tiles.Count)
        {
            //Given a 2x2 grid.  The bottom left is the starting point.
            //right is placed first and in this reduction steps tell the top right corner it can only be 1 thing so that it fits with bottom right.
            //Up next is top left, which tells top right it can be a few options.  Since bottom right was already collapsed
            //This needs to find a common object between bottom right and top left to place in the top right.
            List<Tile> commonTiles = tileOptions.Intersect(cell.tileOptions).ToList();
            
            if(!commonTiles.Any())
                Debug.LogError("There were no common tiles found.  This probably means your scriptable objects aren't setup correctly.");

            cell.tileOptions = commonTiles;
        }
        else
        {
            cell.tileOptions = tileOptions;
        }
    }

    private Cell GetUncollapsedCellAtPosition(float xAxis, float zAxis)
    {
        var cell = cells.Where(x => x.transform.position.x == xAxis && x.transform.position.z == zAxis).FirstOrDefault();
        
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