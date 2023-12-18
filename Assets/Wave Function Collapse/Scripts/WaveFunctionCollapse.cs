using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Wave_Function_Collapse.Scripts
{
    public class WaveFunctionCollapse : MonoBehaviour
    {
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private int offsetSize;
        
        private int changedWidth;

        private int changedHeight;

        //2d array that will store collapsed tiles we can reference them later.
        private Tile[,] grid;

        //All of our possible nodes
        public List<Tile> allPossibleNodes = new List<Tile>();
        public Tile errorNode;
        public List<GameObject> ObjectsCreated = new List<GameObject>();
        
        //List to store tile positions that need collapsing
        private List<Vector3Int> nodesToCollapse = new List<Vector3Int>();
        
        private Vector3Int[] nodeOffsets = new Vector3Int[]
        {
            new Vector3Int(0,0,10),
            new Vector3Int(0,0,-10),
            new Vector3Int(10,0,0),
            new Vector3Int(-10,0,0),
        };

        public void TestingWaveFunctionCollapse()
        {
            foreach (var item in ObjectsCreated)
            {
                DestroyImmediate(item);
            }
            
            ObjectsCreated.Clear();
            
            Start();
        }
        private void Start()
        {
            //Get all the cells for our grid
            
            //Choose a starting cell
            
            //Get a random 
            changedWidth = width * offsetSize;
            changedHeight = height * offsetSize;
            
            grid = new Tile[changedWidth, changedHeight];

            CollapseWorld();
        }

        private void CollapseWorld()
        {
            nodesToCollapse.Clear();

            nodesToCollapse.Add(new Vector3Int(changedWidth / 2,0, changedHeight / 2));

            while (nodesToCollapse.Count > 0)
            {
                int x = nodesToCollapse[0].x;
                int z = nodesToCollapse[0].z;

                List<Tile> potentialNodes = new List<Tile>(allPossibleNodes);

                CycleThroughEachNeighborNode(potentialNodes, x, z);

                var prefabYAxis = grid[x, z].Prefab.transform.rotation.eulerAngles.y;
                
                ObjectsCreated.Add(Instantiate(grid[x, z].Prefab, new Vector3(x, 0f, z), Quaternion.Euler(0f, prefabYAxis, 0f)));
                
                nodesToCollapse.RemoveAt(0);
            }
        }

        private void CycleThroughEachNeighborNode(List<Tile> potentialNodes, int x, int z)
        {
            for (int i = 0; i < nodeOffsets.Length; i++)
            {
                Vector3Int neighborGridSpot = new Vector3Int(x + nodeOffsets[i].x, 0, z + nodeOffsets[i].z);

                if (IsNeighorNodeInsideGrid(neighborGridSpot))
                {
                    Tile neighborTile = grid[neighborGridSpot.x, neighborGridSpot.z];
                    
                    if (neighborTile != null)
                        ReducePossibleNodeOptionsForThisNeighborNode(potentialNodes, neighborTile, i);
                    else
                        if(!nodesToCollapse.Contains(neighborGridSpot))
                            nodesToCollapse.Add(neighborGridSpot);
                    
                    if(potentialNodes.Count == 0)
                        Debug.LogError($"PotentialNodes just hit 0 records.  NeighborTile: {neighborTile}");
                }
            }
            
            GetNodeForPlacement(potentialNodes, x, z);
        }

        private void GetNodeForPlacement(List<Tile> potentialNodes, int x, int z)
        {
            if (potentialNodes.Count < 1)
            {
                grid[x, z] = errorNode;
                Debug.LogWarning(($"Attempted to collapse wave, but found no compatible nodes for grid position x: {x}, z: {z}."));
            }
            else
            {
                //I believe this gets a random node that is allowed for this neighbor.
                grid[x, z] = potentialNodes[Random.Range(0, potentialNodes.Count)];
            }
        }

        private void ReducePossibleNodeOptionsForThisNeighborNode(List<Tile> potentialNodes, Tile neighborTile, int i)
        {
            switch (i)
            {
                case 0:
                    ReduceNodeOptions(potentialNodes, neighborTile.South);
                    break;
                case 1:
                    ReduceNodeOptions(potentialNodes, neighborTile.North);
                    break;
                case 2:
                    ReduceNodeOptions(potentialNodes, neighborTile.East);
                    break;
                case 3:
                    ReduceNodeOptions(potentialNodes, neighborTile.West);
                    break;
            }
        }
        
        private void ReduceNodeOptions(List<Tile> potentialNodes, List<Tile> validNodes)
        {
            for (int i = potentialNodes.Count - 1; i > -1; i--)
            {
                if (!validNodes.Contains(potentialNodes[i]))
                {
                    potentialNodes.RemoveAt(i);
                }
            }
        }
        
        private bool IsNeighorNodeInsideGrid(Vector3Int neighborNode)
        {
            if (neighborNode.x > -1 && neighborNode.x < changedWidth && neighborNode.z > -1 && neighborNode.z < changedHeight)
                return true;

            return false;
        }
    }
}