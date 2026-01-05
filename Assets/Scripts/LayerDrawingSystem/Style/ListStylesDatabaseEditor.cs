using UnityEngine;
using UnityEditor;

namespace VahTyah.List
{
    [CustomEditor(typeof(ListStylesDatabase))]
    public class ListStylesDatabaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ListStylesDatabase database = (ListStylesDatabase)target;
            DrawDefaultInspector();
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Add Default Style", GUILayout.Height(25)))
            {
                Undo.RecordObject(database, "Add Default Style");
                database.AddDefaultStyle();
                EditorUtility.SetDirty(database);
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
}