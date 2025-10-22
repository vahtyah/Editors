using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Levels Panel - Quản lý LevelData với kiến trúc SOLID
/// Single Responsibility: Chỉ xử lý UI và tương tác người dùng
/// Open/Closed: Dễ dàng mở rộng thêm chức năng mới
/// Liskov Substitution: Kế thừa từ EditorPanel
/// Interface Segregation: Sử dụng các interface/service riêng biệt
/// Dependency Inversion: Phụ thuộc vào abstraction (LevelDataManager)
/// </summary>
public class LevelsPanel : EditorPanel
{
    public override string Name => "Levels";

    // Dependencies - Dependency Inversion Principle
    private LevelDataManager levelDataManager;
    private LevelDataRenderer levelDataRenderer;
    private LevelDataCreator levelDataCreator;

    // State
    private List<LevelData> levelDataList;
    private LevelData selectedLevelData;
    private Vector2 levelListScrollPosition;
    private Vector2 levelDetailsScrollPosition;

    // UI State
    private bool showLevelList = true;
    private bool showLevelDetails = true;
    private bool showCreateNewLevel = false;

    public override void OnEnable()
    {
        try
        {
            // Initialize dependencies
            levelDataManager = new LevelDataManager();
            levelDataRenderer = new LevelDataRenderer();
            levelDataCreator = new LevelDataCreator();

            // Load data
            RefreshLevelList();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error initializing LevelsPanel: {e.Message}\n{e.StackTrace}");
        }
    }

    public override void OnDisable()
    {
        // Cleanup if needed
    }

    public override void Draw()
    {
        EditorGUIHelper.DrawHeader("Levels Panel");
        EditorGUIHelper.DrawHelpBox("Quản lý tất cả LevelData trong project.", MessageType.Info);

        EditorGUILayout.Space(LevelEditorStyles.Spacing.Medium);

        DrawToolbar();
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);

        EditorGUILayout.BeginHorizontal();
        
        // Left panel - Level list
        EditorGUILayout.BeginVertical(GUILayout.Width(250));
        DrawLevelList();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);

        // Right panel - Level details
        EditorGUILayout.BeginVertical();
        DrawLevelDetails();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(80)))
        {
            RefreshLevelList();
        }

        if (GUILayout.Button("Create New", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
            showCreateNewLevel = !showCreateNewLevel;
        }

        GUILayout.FlexibleSpace();

        EditorGUILayout.LabelField($"Total: {levelDataList?.Count ?? 0}", EditorStyles.toolbarButton, GUILayout.Width(80));

        EditorGUILayout.EndHorizontal();

        // Create new level section
        if (showCreateNewLevel)
        {
            DrawCreateNewLevelSection();
        }
    }

    private void DrawCreateNewLevelSection()
    {
        if (levelDataCreator == null)
        {
            levelDataCreator = new LevelDataCreator();
        }
        
        if (levelDataManager == null)
        {
            levelDataManager = new LevelDataManager();
        }

        EditorGUIHelper.DrawBox("Create New Level", () =>
        {
            levelDataCreator.Draw();

            EditorGUILayout.BeginHorizontal();

            if (EditorGUIHelper.DrawColoredButton("Create", LevelEditorStyles.Colors.Success, 0, LevelEditorStyles.Sizes.ButtonHeight))
            {
                LevelData newLevel = levelDataCreator.CreateLevel(levelDataManager);
                if (newLevel != null)
                {
                    RefreshLevelList();
                    selectedLevelData = newLevel;
                    showCreateNewLevel = false;
                    EditorUtility.DisplayDialog("Success", $"Level {newLevel.levelNumber} created successfully!", "OK");
                }
            }

            if (EditorGUIHelper.DrawButton("Cancel", 0, LevelEditorStyles.Sizes.ButtonHeight))
            {
                showCreateNewLevel = false;
            }

            EditorGUILayout.EndHorizontal();
        });

        EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);
    }

    private void DrawLevelList()
    {
        showLevelList = EditorGUIHelper.DrawFoldout(showLevelList, "Level List");

        if (!showLevelList)
            return;

        EditorGUILayout.BeginVertical(LevelEditorStyles.BoxStyle);

        if (levelDataList == null || levelDataList.Count == 0)
        {
            EditorGUIHelper.DrawCenteredLabel("No levels found");
            EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);
            
            if (EditorGUIHelper.DrawButton("Create First Level", 0, LevelEditorStyles.Sizes.ButtonHeight))
            {
                showCreateNewLevel = true;
            }
        }
        else
        {
            levelListScrollPosition = EditorGUILayout.BeginScrollView(levelListScrollPosition, GUILayout.Height(400));

            foreach (LevelData levelData in levelDataList)
            {
                if (levelData == null)
                    continue;

                DrawLevelListItem(levelData);
            }

            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawLevelListItem(LevelData levelData)
    {
        bool isSelected = selectedLevelData == levelData;
        Color originalColor = GUI.backgroundColor;

        if (isSelected)
            GUI.backgroundColor = LevelEditorStyles.Colors.Primary;

        EditorGUILayout.BeginHorizontal(LevelEditorStyles.BoxStyle);

        // Level info button
        if (GUILayout.Button($"Level {levelData.levelNumber}", GUILayout.Height(40), GUILayout.ExpandWidth(true)))
        {
            selectedLevelData = levelData;
            Selection.activeObject = levelData;
        }

        // Quick actions
        EditorGUILayout.BeginVertical(GUILayout.Width(30));

        if (GUILayout.Button("▶", GUILayout.Height(18)))
        {
            ApplyLevel(levelData);
        }

        if (GUILayout.Button("×", GUILayout.Height(18)))
        {
            DeleteLevel(levelData);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        GUI.backgroundColor = originalColor;
    }

    private void DrawLevelDetails()
    {
        showLevelDetails = EditorGUIHelper.DrawFoldout(showLevelDetails, "Level Details");

        if (!showLevelDetails)
            return;

        EditorGUILayout.BeginVertical(LevelEditorStyles.BoxStyle);

        if (selectedLevelData == null)
        {
            EditorGUIHelper.DrawCenteredLabel("Select a level to view details");
        }
        else
        {
            levelDetailsScrollPosition = EditorGUILayout.BeginScrollView(levelDetailsScrollPosition);

            DrawSelectedLevelInfo();
            EditorGUILayout.Space(LevelEditorStyles.Spacing.Medium);

            DrawSelectedLevelProperties();
            EditorGUILayout.Space(LevelEditorStyles.Spacing.Medium);

            DrawSelectedLevelLayout();
            EditorGUILayout.Space(LevelEditorStyles.Spacing.Medium);

            DrawSelectedLevelActions();

            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawSelectedLevelInfo()
    {
        EditorGUIHelper.DrawBox("Level Information", () =>
        {
            EditorGUILayout.LabelField("Asset Path:", AssetDatabase.GetAssetPath(selectedLevelData), EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("Dimensions:", $"{selectedLevelData.width} x {selectedLevelData.height}");
            EditorGUILayout.LabelField("Layout Size:", $"{selectedLevelData.layout?.Length ?? 0} rows");

            // Validation
            if (levelDataManager.ValidateLevelData(selectedLevelData, out string errorMessage))
            {
                EditorGUIHelper.DrawHelpBox("✓ Level data is valid", MessageType.Info);
            }
            else
            {
                EditorGUIHelper.DrawHelpBox($"⚠ Validation Error: {errorMessage}", MessageType.Warning);
            }
        });
    }

    private void DrawSelectedLevelProperties()
    {
        EditorGUIHelper.DrawBox("Properties", () =>
        {
            EditorGUI.BeginChangeCheck();

            selectedLevelData.levelNumber = EditorGUILayout.IntField("Level Number:", selectedLevelData.levelNumber);
            selectedLevelData.width = EditorGUILayout.IntField("Width:", selectedLevelData.width);
            selectedLevelData.height = EditorGUILayout.IntField("Height:", selectedLevelData.height);
            selectedLevelData.backgroundColor = EditorGUILayout.ColorField("Background Color:", selectedLevelData.backgroundColor);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(selectedLevelData);
            }
        });
    }

    private void DrawSelectedLevelLayout()
    {
        EditorGUIHelper.DrawBox("Layout Preview", () =>
        {
            if (selectedLevelData.layout != null && selectedLevelData.layout.Length > 0)
            {
                levelDataRenderer.DrawLayoutPreview(selectedLevelData);
            }
            else
            {
                EditorGUIHelper.DrawCenteredLabel("No layout data");
            }
        });
    }

    private void DrawSelectedLevelActions()
    {
        EditorGUIHelper.DrawBox("Actions", () =>
        {
            EditorGUILayout.BeginHorizontal();

            if (EditorGUIHelper.DrawColoredButton("Apply Level", LevelEditorStyles.Colors.Success, 0, LevelEditorStyles.Sizes.ButtonHeight))
            {
                ApplyLevel(selectedLevelData);
            }

            if (EditorGUIHelper.DrawButton("Duplicate", 0, LevelEditorStyles.Sizes.ButtonHeight))
            {
                DuplicateLevel(selectedLevelData);
            }

            if (EditorGUIHelper.DrawColoredButton("Delete", LevelEditorStyles.Colors.Error, 0, LevelEditorStyles.Sizes.ButtonHeight))
            {
                DeleteLevel(selectedLevelData);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);

            EditorGUILayout.BeginHorizontal();

            if (EditorGUIHelper.DrawButton("Save Changes", 0, LevelEditorStyles.Sizes.ButtonHeight))
            {
                SaveLevel(selectedLevelData);
            }

            if (EditorGUIHelper.DrawButton("Select in Project", 0, LevelEditorStyles.Sizes.ButtonHeight))
            {
                Selection.activeObject = selectedLevelData;
                EditorGUIUtility.PingObject(selectedLevelData);
            }

            EditorGUILayout.EndHorizontal();
        });
    }

    // Actions - Single Responsibility
    private void RefreshLevelList()
    {
        levelDataList = levelDataManager.LoadAllLevelData();
    }

    private void ApplyLevel(LevelData levelData)
    {
        if (levelData == null)
            return;

        levelDataManager.ApplyChanges(levelData);
        Debug.Log($"Applied Level {levelData.levelNumber}");
        EditorUtility.DisplayDialog("Success", $"Level {levelData.levelNumber} applied successfully!", "OK");
    }

    private void SaveLevel(LevelData levelData)
    {
        if (levelData == null)
            return;

        levelDataManager.ApplyChanges(levelData);
        Debug.Log($"Saved Level {levelData.levelNumber}");
        EditorUtility.DisplayDialog("Success", $"Level {levelData.levelNumber} saved successfully!", "OK");
    }

    private void DuplicateLevel(LevelData levelData)
    {
        if (levelData == null)
            return;

        LevelData duplicate = levelDataManager.DuplicateLevelData(levelData);
        if (duplicate != null)
        {
            RefreshLevelList();
            selectedLevelData = duplicate;
            Debug.Log($"Duplicated Level {levelData.levelNumber}");
            EditorUtility.DisplayDialog("Success", "Level duplicated successfully!", "OK");
        }
    }

    private void DeleteLevel(LevelData levelData)
    {
        if (levelData == null)
            return;

        if (EditorUtility.DisplayDialog("Delete Level", 
            $"Are you sure you want to delete Level {levelData.levelNumber}?\nThis action cannot be undone.", 
            "Delete", "Cancel"))
        {
            bool success = levelDataManager.DeleteLevelData(levelData);
            if (success)
            {
                if (selectedLevelData == levelData)
                    selectedLevelData = null;

                RefreshLevelList();
                Debug.Log($"Deleted Level {levelData.levelNumber}");
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Failed to delete level.", "OK");
            }
        }
    }
}

