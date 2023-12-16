using System.ComponentModel.Design;
using UnityEditor;
using UnityEngine;

namespace Wave_Function_Collapse.Scripts
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class CreateRotatedPrefabsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MonoBehaviour script = (MonoBehaviour) target;

            if (script is CreateRotatedPrefabs)
            {
                CreatePrefabButton((CreateRotatedPrefabs) script);
            }

            if (script is CreateRotatedScriptableObjects)
            {
                CreateScriptableObjectsButton((CreateRotatedScriptableObjects) script);
            }
        }

        private void CreateScriptableObjectsButton(CreateRotatedScriptableObjects script)
        {
            if (GUILayout.Button("Create rotation data for each rotated prefab"))
            {
                script.Start();
            }
        }

        private void CreatePrefabButton(CreateRotatedPrefabs script)
        {
            if (GUILayout.Button("Create rotation prefabs"))
            {
                script.CreateAndSavePrefab();
            }
        }
    }
}