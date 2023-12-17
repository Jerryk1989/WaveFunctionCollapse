using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "WFC/Node")]
[System.Serializable]
public class Tile : ScriptableObject
{
    public string Name;
    public GameObject Prefab;
    public WFC_Connection Forward;
    public WFC_Connection Back;
    public WFC_Connection Left;
    public WFC_Connection Right;
}

[System.Serializable]
public class WFC_Connection
{
    public List<Tile> CompatibleNodes = new List<Tile>();
}