using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Wave_Function_Collapse.Scripts
{
    public class WaveFunctionCollapse : MonoBehaviour
    {
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private int offsetSize;

        //2d array that will store collapsed tiles we can reference them later.
        private Node[,] grid;

        //All of our possible nodes
        public List<Node> allPossibleNodes = new List<Node>();
        
        //List to store tile positions that need collapsing
        private List<Vector3Int> nodesToCollapse = new List<Vector3Int>();
        
        private Vector3Int[] nodeOffsets = new Vector3Int[]
        {
            new Vector3Int(0,0,10),
            new Vector3Int(0,0,-10),
            new Vector3Int(10,0,0),
            new Vector3Int(-10,0,0),
        };

        private void Start()
        {
            width = width * offsetSize;
            height = height * offsetSize;
            
            grid = new Node[width, height];

            CollapseWorld();
        }

        private void CollapseWorld()
        {
            nodesToCollapse.Clear();

            nodesToCollapse.Add(new Vector3Int(width / 2,0, height / 2));

            while (nodesToCollapse.Count > 0)
            {
                int x = nodesToCollapse[0].x;
                int z = nodesToCollapse[0].z;

                List<Node> potentialNodes = new List<Node>(allPossibleNodes);

                CycleThroughEachNeighborNode(potentialNodes, x, z);
                
                Instantiate(grid[x, z].Prefab, new Vector3(x, 0f, z), Quaternion.identity);
                    
                nodesToCollapse.RemoveAt(0);
            }
        }

        private void CycleThroughEachNeighborNode(List<Node> potentialNodes, int x, int z)
        {
            for (int i = 0; i < nodeOffsets.Length; i++)
            {
                Vector3Int neighborGridSpot = new Vector3Int(x + nodeOffsets[i].x, 0, z + nodeOffsets[i].z);

                if (IsNeighorNodeInsideGrid(neighborGridSpot))
                {
                    Node neighborNode = grid[neighborGridSpot.x, neighborGridSpot.z];

                    if (neighborNode != null)
                        ReducePossibleNodeOptionsForThisNeighborNode(potentialNodes, neighborNode, i);
                    else
                        if(!nodesToCollapse.Contains(neighborGridSpot))
                            nodesToCollapse.Add(neighborGridSpot);

                    GetNodeForPlacement(potentialNodes, x, z);
                }
            }
        }

        private void GetNodeForPlacement(List<Node> potentialNodes, int x, int z)
        {
            if (potentialNodes.Count < 1)
            {
                grid[x, z] = allPossibleNodes[0];
                Debug.LogWarning(("Attempted to collapse wave, but found no compatibae nodes"));
            }
            else
            {
                //I believe this gets a random node that is allowed for this neighbor.
                grid[x, z] = potentialNodes[Random.Range(0, potentialNodes.Count)];
            }
        }

        private void ReducePossibleNodeOptionsForThisNeighborNode(List<Node> potentialNodes, Node neighborNode, int i)
        {
            switch (i)
            {
                case 0:
                    ReduceNodeOptions(potentialNodes, neighborNode.Back.CompatibleNodes);
                    break;
                case 1:
                    ReduceNodeOptions(potentialNodes, neighborNode.Forward.CompatibleNodes);
                    break;
                case 2:
                    ReduceNodeOptions(potentialNodes, neighborNode.Left.CompatibleNodes);
                    break;
                case 3:
                    ReduceNodeOptions(potentialNodes, neighborNode.Right.CompatibleNodes);
                    break;
            }
        }
        
        private void ReduceNodeOptions(List<Node> potentialNodes, List<Node> validNodes)
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
            if (neighborNode.x > -1 && neighborNode.x < width && neighborNode.z > -1 && neighborNode.z < height)
                return true;

            return false;
        }
    }
}