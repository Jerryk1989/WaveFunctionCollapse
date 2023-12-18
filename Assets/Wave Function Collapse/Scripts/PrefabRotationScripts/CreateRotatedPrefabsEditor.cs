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

            if (script is WaveFunctionCollapse)
            {
                CreateWaveFunctionCollapseTesterButton((WaveFunctionCollapse) script);
            }
            
            if (script is MyWFC)
            {
                CreateMyWFCTesterButton((MyWFC) script);
            }
        }

        private void CreateMyWFCTesterButton(MyWFC script)
        {
            if (GUILayout.Button(("Create Grid")))
            {
                script.Start();
            }
        }

        private void CreateWaveFunctionCollapseTesterButton(WaveFunctionCollapse script)
        {
            if (GUILayout.Button(("Create Grid")))
            {
                script.TestingWaveFunctionCollapse();
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