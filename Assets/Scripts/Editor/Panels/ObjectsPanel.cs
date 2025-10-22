using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ObjectsPanel : EditorPanel
{
    public override string Name => "Objects";

    private GameObject selectedPrefab;
    private List<GameObject> prefabLibrary = new List<GameObject>();
    private Vector2 libraryScrollPosition;
    private bool showLibrary = true;
    private bool showSpawnSettings = true;

    public override void OnEnable()
    {
        // Khởi tạo dữ liệu cho Objects Panel
        LoadPrefabLibrary();
    }

    public override void OnDisable()
    {
        // Dọn dẹp resources khi panel bị disable
    }

    public override void Draw()
    {
        EditorGUIHelper.DrawHeader("Objects Panel");
        EditorGUIHelper.DrawHelpBox("Thêm và quản lý các objects trong level.", MessageType.Info);
        
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Medium);

        DrawPrefabLibrary();
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Medium);

        DrawSelectedPrefab();
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Medium);

        DrawSpawnSettings();
    }

    private void DrawPrefabLibrary()
    {
        showLibrary = EditorGUIHelper.DrawFoldout(showLibrary, "Prefab Library");
        
        if (showLibrary)
        {
            EditorGUIHelper.DrawBox("", () =>
            {
                // Add prefab button
                EditorGUILayout.BeginHorizontal();
                GameObject newPrefab = EditorGUIHelper.DrawObjectField<GameObject>("Add Prefab:", null, false);
                if (newPrefab != null && !prefabLibrary.Contains(newPrefab))
                {
                    prefabLibrary.Add(newPrefab);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUIHelper.DrawSeparator();
                EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);

                // Prefab list
                if (prefabLibrary.Count == 0)
                {
                    EditorGUIHelper.DrawCenteredLabel("Chưa có prefab nào trong library");
                }
                else
                {
                    libraryScrollPosition = EditorGUILayout.BeginScrollView(libraryScrollPosition, GUILayout.Height(200));
                    
                    for (int i = 0; i < prefabLibrary.Count; i++)
                    {
                        if (prefabLibrary[i] == null)
                            continue;

                        EditorGUILayout.BeginHorizontal(LevelEditorStyles.BoxStyle);
                        
                        bool isSelected = selectedPrefab == prefabLibrary[i];
                        Color originalColor = GUI.backgroundColor;
                        if (isSelected)
                            GUI.backgroundColor = LevelEditorStyles.Colors.Primary;

                        if (GUILayout.Button(prefabLibrary[i].name, GUILayout.Height(30)))
                        {
                            selectedPrefab = prefabLibrary[i];
                        }

                        GUI.backgroundColor = originalColor;

                        if (EditorGUIHelper.DrawColoredButton("X", LevelEditorStyles.Colors.Error, 30, 30))
                        {
                            prefabLibrary.RemoveAt(i);
                            if (selectedPrefab == prefabLibrary[i])
                                selectedPrefab = null;
                            i--;
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                    
                    EditorGUILayout.EndScrollView();
                }
            });
        }
    }

    private void DrawSelectedPrefab()
    {
        EditorGUIHelper.DrawBox("Selected Prefab", () =>
        {
            if (selectedPrefab != null)
            {
                EditorGUILayout.LabelField("Name:", selectedPrefab.name);
                selectedPrefab = EditorGUIHelper.DrawObjectField<GameObject>("Prefab:", selectedPrefab, false);
            }
            else
            {
                EditorGUIHelper.DrawCenteredLabel("Chưa chọn prefab nào");
            }
        });
    }

    private void DrawSpawnSettings()
    {
        showSpawnSettings = EditorGUIHelper.DrawFoldout(showSpawnSettings, "Spawn Settings");
        
        if (showSpawnSettings)
        {
            EditorGUIHelper.DrawBox("", () =>
            {
                EditorGUI.BeginDisabledGroup(selectedPrefab == null);
                
                if (EditorGUIHelper.DrawColoredButton("Spawn at Scene View Center", LevelEditorStyles.Colors.Success, 0, LevelEditorStyles.Sizes.ButtonHeight))
                {
                    SpawnPrefab();
                }
                
                EditorGUI.EndDisabledGroup();
            });
        }
    }

    private void LoadPrefabLibrary()
    {
        // TODO: Load từ EditorPrefs hoặc file config
    }

    private void SpawnPrefab()
    {
        if (selectedPrefab != null)
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
            if (instance != null)
            {
                Undo.RegisterCreatedObjectUndo(instance, "Spawn " + selectedPrefab.name);
                Selection.activeGameObject = instance;
                Debug.Log("Spawned: " + selectedPrefab.name);
            }
        }
    }
}

