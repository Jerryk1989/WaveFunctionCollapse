using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Wave_Function_Collapse.Scripts
{
    public class CreateRotatedScriptableObjects : MonoBehaviour
    {
        public string FilePathToRotatedPrefabs;

        private string scriptableObjectOutputFolder = "Assets/Wave Function Collapse/ScriptableObjects/CreateRotationDataOutput";
        
        private readonly string forward = "forward";
        private readonly string back = "back";
        private readonly string left = "left";
        private readonly string right = "right";

        private List<GameObject> allRotatedPrefabs;
        private Dictionary<string, List<GameObject>> prefabGroupingDictionary;
        private List<Node> allNodes;

        public void Start()
        {
            //Check if file path has been entered is not provided or invalid.  Log an error and quit out.
            if (string.IsNullOrEmpty(FilePathToRotatedPrefabs) || !AssetDatabase.IsValidFolder(FilePathToRotatedPrefabs))
            {
                Debug.LogError("A valid folder path to your rotated objects is required.");
                return;
            }
            
            //Gets all the rotated prefabs from this folder.
            allRotatedPrefabs = GetAllRotatedPrefabsFromFolder(FilePathToRotatedPrefabs);
            
            //Log error and quit out if no prefabs were returned.
            if (allRotatedPrefabs == null || !allRotatedPrefabs.Any())
            {
                Debug.LogError($"There were no rotated prefabs found in {FilePathToRotatedPrefabs}.");
                return;
            }
            
            if (!AssetDatabase.IsValidFolder(scriptableObjectOutputFolder))
            {
                //TODO:  This should be changed to use the scriptableObjectOutputFolder variable.  Just get the path and folder from that instead of this.
                //Once that is done, you can make that scriptableObjectOutputFolder variable public so a user can type it in via the unity editor
                AssetDatabase.CreateFolder("Assets/Wave Function Collapse/ScriptableObjects", "CreateRotationDataOutput");
            }
            
            
            //Later in the code we will need to find all road groupings for instance
            //Instead of calling get component on every object every time we need to do this
            //I am going through them all once here and adding their data to a dictionary where the key is the groupingName 
            //This lets me look at all the groupings later without having to call GetComponent again.
            prefabGroupingDictionary = GetAllPrefabGroupings(allRotatedPrefabs);
            
            //We need to do this here because all the scriptableObjects need to exist first before we can add their neighbor nodes.
            allNodes = GetScriptableObjects(allRotatedPrefabs, scriptableObjectOutputFolder);
            
            //Pass the dictionary into this method where we will actually start looking at the rules and populating valid neighbors
            PopulateEachRotationsAllowedNeighborNodes(prefabGroupingDictionary);
        }

        private List<Node> GetScriptableObjects(List<GameObject> prefabs, string outputFolder)
        {
            List<Node> nodes = new List<Node>();
            
            foreach (var prefab in prefabs)
            {
                Node nodeScriptableObject = ScriptableObject.CreateInstance<Node>();

                nodeScriptableObject.Prefab = prefab;
                nodeScriptableObject.Name = $"{prefab.name}";
            
                AssetDatabase.CreateAsset(nodeScriptableObject, outputFolder + $"/{prefab.name}.asset");
                
                nodes.Add(nodeScriptableObject);
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return nodes;
        }

        private Dictionary<string, List<GameObject>> GetAllPrefabGroupings(List<GameObject> prefabs)
        {
            Dictionary<string, List<GameObject>> prefabDictionary = new Dictionary<string, List<GameObject>>();
            
            prefabs.ForEach(prefab =>
            {
                if (prefab == null)
                {
                    Debug.LogError($"A null prefab was found while trying to add to the prefabDictionary.");
                    return;
                }
                
                var groupingName = prefab.GetComponent<PrefabGroupingInfo>().grouping.groupingName;

                if (!prefabDictionary.ContainsKey(groupingName))
                    prefabDictionary[groupingName] = new List<GameObject>();
                
                prefabDictionary[groupingName].Add(prefab);
            });

            return prefabDictionary;
        }

        private void PopulateEachRotationsAllowedNeighborNodes(Dictionary<string, List<GameObject>> prefabGroupings)
        {
            //We need to iterate through the dictionary.
            foreach (var dictionaryEntry in prefabGroupings)
            {
                //Get the key for the List of gameobjects we are about to look at.
                var grouping = dictionaryEntry.Key;
                
                //Now we need to iterate through the list of Game objects associated to the key.
                foreach (var prefab in dictionaryEntry.Value)
                {
                    //Get the direction as a string.  We use this for the rules in the switch statement.
                    string prefabDirection = GetPrefabDirection(prefab.name);

                    if (string.IsNullOrEmpty(prefabDirection))
                    {
                        Debug.LogError($"prefabDirection could not be found for prefab {prefab.name}");
                        return;
                    }

                    var currentNode = allNodes.Where(x => x.Prefab.name == prefab.name).FirstOrDefault();

                    if (currentNode == null)
                    {
                        Debug.LogError($"Node could not be found for prefab {prefab.name}");
                        return;
                    }
                    
                    //Probably room for improvement, but the rules may vary a little depending on the if it's a corner, road, intersection etc.
                    //I noticed front facing buildings pretty much all have the same rules though, which is how I came up with these 4 groupings.
                    //I went into the editor and placed a bunch of pieces and looked at what the groupings looked like.  I then came up with 4 different types of groups that each
                    //rotation could fall into which is what you are seeing here.
                    switch (grouping.ToLower())
                    {
                        case "building":
                            PopulateValidBuildingNeigbhors(prefab, prefabDirection.ToLower(), currentNode);
                            break;
                        case "corner":
                            PopulateValidCornerNeighbors(prefab, prefabDirection.ToLower(), currentNode);
                            break;
                        case "road":
                            PopulateValidRoadNeighrbors(prefab, prefabDirection.ToLower(), currentNode);
                            break;
                        case "intersection":
                            PopulateValidIntersectionNeighbors(prefab, prefabDirection.ToLower(), currentNode);
                            break;
                        default:
                            Debug.LogError($"Could not find a suitable case to switch to for {grouping} on prefabe {prefab.name}.");
                            break;
                    }
                }
            }
        }

        private void PopulateValidIntersectionNeighbors(GameObject prefab, string prefabDirection, Node currentNode)
        {

        }

        private void PopulateValidRoadNeighrbors(GameObject prefab, string prefabDirection, Node currentNode)
        {

        }

        private void PopulateValidCornerNeighbors(GameObject prefab, string prefabDirection, Node currentNode)
        {

        }

        private void PopulateValidBuildingNeigbhors(GameObject prefab, string prefabDirection, Node currentNode)
        {
            List<GameObject> forwardObjects = new List<GameObject>();
            List<GameObject> backObjects = new List<GameObject>();
            List<GameObject> rightObjects = new List<GameObject>();
            List<GameObject> leftObjects = new List<GameObject>();
            
            //Get all the objects by their different grouping
            var roads = prefabGroupingDictionary.Where(x => x.Key.ToLower() == "road").Select(x => x.Value).FirstOrDefault();
            var buildings = prefabGroupingDictionary.Where(x => x.Key.ToLower() == "building").Select(x => x.Value).FirstOrDefault();
            var corners = prefabGroupingDictionary.Where(x => x.Key.ToLower() == "corner").Select(x => x.Value).FirstOrDefault();
            var intersections = prefabGroupingDictionary.Where(x => x.Key.ToLower() == "intersections").Select(x => x.Value).FirstOrDefault();
            
            if (prefabDirection == forward)
            {
                //Get each valid object for every neighbor by the grouping.
                forwardObjects.AddRange(roads.Where(x => x.name.StartsWith(forward)).ToList());
                rightObjects.AddRange(buildings.Where(x => x.name.StartsWith(forward)).ToList());
                leftObjects.AddRange(buildings.Where(x => x.name.StartsWith(forward)).ToList());
                leftObjects.AddRange(corners.Where(x => x.name.StartsWith(left)).ToList());
                backObjects.AddRange(buildings.Where(x => x.name.StartsWith(back)).ToList());

                //This just updates the currentNode we are looking at with the new Node lists
                UpdateNode(forwardObjects, backObjects, rightObjects, leftObjects, currentNode);
            }

            //if left
            //Valid tiles = [buildings facing forward, corner facing left]
            if (prefabDirection == left)
            {
                
            }
            
            //if right
            //Valid tiles = [buildings facing forward, corner facing forward]
            
            
            //if back
            //Valid tiles = [buildings facing back]
            
            if (prefabDirection == forward)
            {
                
            }
                
        }

        private void UpdateNode(List<GameObject> forwardObjects, List<GameObject> backObjects, List<GameObject> rightObjects, List<GameObject> leftObjects, Node currentNode)
        {
            //The current node we are looking at has 4 directions.
            //For each of these directions I need to give it the list of valid nodes, not game objects.
            //This chunk just takes the game objects we have found as valid and finds the corresponding nodes.
            List<Node> forwardNodes = GetNodes(forwardObjects);
            List<Node> backNodes = GetNodes(backObjects);
            List<Node> rightNodes = GetNodes(rightObjects);
            List<Node> leftNodes = GetNodes(leftObjects);
            
            currentNode.Forward = new WFC_Connection()
            {
                CompatibleNodes = forwardNodes
            };
            
            currentNode.Back = new WFC_Connection()
            {
                CompatibleNodes = backNodes
            };
            
            currentNode.Left = new WFC_Connection()
            {
                CompatibleNodes = leftNodes
            };
            
            currentNode.Right = new WFC_Connection()
            {
                CompatibleNodes = rightNodes
            };
            
            //Now we just need to save our changes for our current node (scriptable object)
            EditorUtility.SetDirty(currentNode);
            AssetDatabase.SaveAssets();
        }

        private List<Node> GetNodes(List<GameObject> validNeighbors)
        {
            List<Node> nodes = new List<Node>();
            
            foreach (var prefab in validNeighbors)
            {
                var curNode = allNodes.Where(x => x.Prefab == prefab).FirstOrDefault();
                
                if(curNode != null)
                    nodes.Add(curNode);
                else
                {
                    Debug.LogError($"Could not find Node in list of all nodes for prefab {prefab.name}");
                }
            }

            return nodes;
        }
        
        private string GetPrefabDirection(string prefabName)
        {
            if (string.IsNullOrEmpty(prefabName) || string.IsNullOrWhiteSpace(prefabName))
            {
                Debug.LogError($"Prefab name cannot be null, empty, or white space.");
                return null;
            }
            
            //All my rotated prefabs start with "direction-".  Because of this, I can split on - and take the first part.
            return prefabName.Split('-')[0];
        }

        private List<GameObject> GetAllRotatedPrefabsFromFolder(string filePathToRotatedPrefabs)
        {
            List<GameObject> prefabs = new List<GameObject>();
            
            //Get all the paths for each asset found within our folder
            string[] prefabPathsAsGUIDs = AssetDatabase.FindAssets("t:GameObject", new[] {filePathToRotatedPrefabs});
            
            //Iterate through each path we found
            foreach (var guid  in prefabPathsAsGUIDs)
            {
                //Get the prefab for the given prefabPath
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                
                //If the prefab is not null, add it to the prefabs list.
                if(prefab != null)
                    prefabs.Add(prefab);
            }
            
            //Return the populated prefabs list.
            return prefabs;
        }
    }
}