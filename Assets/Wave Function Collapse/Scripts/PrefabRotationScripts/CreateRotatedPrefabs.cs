using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Wave_Function_Collapse.Scripts
{
    public class CreateRotatedPrefabs : MonoBehaviour
    {
        //I want to create the rotated variants for many prefabs at once so I make this a list
        public List<GameObject> prefabsToRotate;

        public void CreateAndSavePrefab()
        {
            //If the prefab list is not null and has any items then we step in.
            if (prefabsToRotate != null && prefabsToRotate.Any())
            {
                //I've hard coded the path here, change this if you don't want your rotated prefabs going here.
                //All we are doing is checking in the if statement if the folder is valid and if it's not (doesn't exist) we are creating it.
                if (!AssetDatabase.IsValidFolder("Assets/Wave Function Collapse/Prefabs/RotatedPrefabs"))
                {
                    AssetDatabase.CreateFolder("Assets/Wave Function Collapse/Prefabs", "RotatedPrefabs");
                }
                
                //Loop through each prefab I added to the list in the unity editor
                //You can see it calls CreatePrefab 4 times.  Once for each direction.
                //I'm sure there is a better way to do this, but this is a script that won't change much and won't run much.
                //So I'm fine with this for now.  Once I have my prefabs, I run it once and never again for the most part.
                foreach (var prefab in prefabsToRotate)
                {
                    CreatePrefab(Vector3.forward, prefab, "forward");
                    CreatePrefab(Vector3.left, prefab, "left");
                    CreatePrefab(Vector3.right, prefab, "right");
                    CreatePrefab(Vector3.back, prefab, "back");
                }
            }
            else
            {
                Debug.LogError("Please assign a GameObject to 'objectToCreatePrefabsFor' before creating the prefab.");
            }
        }

        private void CreatePrefab(Vector3 directionVector, GameObject prefab, string direction)
        {
            var path = $"Assets/Wave Function Collapse/Prefabs/RotatedPrefabs/{direction}-{prefab.name}.prefab";
            
            //This is just checking to see if the prefab exists already at this path.  If it does we return instead of saving a new one.
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path))
                return;
            
            //rotate the asset to face the correct direction.
            prefab.transform.LookAt(directionVector);
            
            //Save the rotated variant of the prefab.
            PrefabUtility.SaveAsPrefabAsset(prefab, path);
        }
    }
}