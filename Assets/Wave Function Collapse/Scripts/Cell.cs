using System.Collections.Generic;
using UnityEngine;

namespace Wave_Function_Collapse.Scripts
{
    public class Cell : MonoBehaviour
    {
        public bool isCollapsed;
        public List<Tile> tileOptions;

        public void CreateCell(bool collapedStatus, List<Tile> options)
        {
            isCollapsed = collapedStatus;
            tileOptions = options;
        }
    }
}