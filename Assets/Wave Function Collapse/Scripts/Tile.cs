using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Node", menuName = "WFC/Node")]
[System.Serializable]
public class Tile : ScriptableObject
{
    public string Name;
    public GameObject Prefab;
    
    public List<Tile> North;
    public List<Tile> South;
    public List<Tile> East;
    public List<Tile> West;
}