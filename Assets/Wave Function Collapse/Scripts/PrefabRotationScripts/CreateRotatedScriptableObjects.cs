using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Wave_Function_Collapse.Scripts
{
    public class CreateRotatedScriptableObjects : MonoBehaviour
    {
        public string FilePathToRotatedPrefabs;

        private List<GameObject> allRotatedPrefabs;
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

            PopulateEachRotationsAllowedNeighborNodes(allRotatedPrefabs);
        }

        private void PopulateEachRotationsAllowedNeighborNodes(List<GameObject> prefabs)
        {
            foreach (var prefab in prefabs)
            {
                
                //If building
                
                //If corner building
                
                //if road
                
                //if interesction
            }
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