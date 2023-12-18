using System.Collections.Generic;
using UnityEngine;

namespace Wave_Function_Collapse.Scripts
{
    public class Cell : MonoBehaviour
    {
        public bool isCollapsed;
        public List<Tile> tileOptions;

        public List<Tile> northTileOptions;
        public List<Tile> southTileOptions;
        public List<Tile> eastTileOptions;
        public List<Tile> westTileOptions;
        
        public void CreateCell(bool collapedStatus, List<Tile> options)
        {
            isCollapsed = collapedStatus;
            tileOptions = options;
        }
    }
}