using UnityEngine;
using UnityEditor;

/// <summary>
/// Creator class để tạo LevelData mới - Single Responsibility Principle
/// Chỉ chịu trách nhiệm UI và logic tạo level
/// </summary>
public class LevelDataCreator
{
    private int newLevelNumber;
    private int newWidth = 10;
    private int newHeight = 10;
    private bool useAutoLevelNumber = true;

    public LevelDataCreator()
    {
        newLevelNumber = 1;
    }

    /// <summary>
    /// Vẽ UI để tạo level mới
    /// </summary>
    public void Draw()
    {
        useAutoLevelNumber = EditorGUILayout.Toggle("Auto Level Number", useAutoLevelNumber);

        EditorGUI.BeginDisabledGroup(useAutoLevelNumber);
        newLevelNumber = EditorGUILayout.IntField("Level Number:", newLevelNumber);
        EditorGUI.EndDisabledGroup();

        if (useAutoLevelNumber)
        {
            EditorGUILayout.HelpBox("Level number will be set automatically to the next available number.", MessageType.Info);
        }

        EditorGUILayout.Space(5);

        newWidth = EditorGUILayout.IntSlider("Width:", newWidth, 5, 50);
        newHeight = EditorGUILayout.IntSlider("Height:", newHeight, 5, 50);

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField($"Grid Size: {newWidth} x {newHeight} = {newWidth * newHeight} cells");
    }

    /// <summary>
    /// Tạo level mới sử dụng LevelDataManager
    /// </summary>
    public LevelData CreateLevel(LevelDataManager manager)
    {
        if (manager == null)
        {
            Debug.LogError("LevelDataManager is null!");
            return null;
        }

        // Validate input
        if (newWidth <= 0 || newHeight <= 0)
        {
            EditorUtility.DisplayDialog("Invalid Input", "Width and Height must be greater than 0!", "OK");
            return null;
        }

        // Get level number
        int levelNumber = useAutoLevelNumber ? manager.GetNextLevelNumber() : newLevelNumber;

        // Check if level already exists
        var existingLevels = manager.LoadAllLevelData();
        foreach (var level in existingLevels)
        {
            if (level.levelNumber == levelNumber)
            {
                bool overwrite = EditorUtility.DisplayDialog("Level Exists", 
                    $"Level {levelNumber} already exists. Do you want to create it anyway?", 
                    "Yes", "No");
                
                if (!overwrite)
                    return null;
                break;
            }
        }

        // Create level
        LevelData newLevel = manager.CreateNewLevelData(levelNumber, newWidth, newHeight);

        if (newLevel != null)
        {
            Debug.Log($"Created new level: Level {levelNumber} ({newWidth}x{newHeight})");
        }

        return newLevel;
    }

    /// <summary>
    /// Reset form về mặc định
    /// </summary>
    public void Reset()
    {
        newLevelNumber = 1;
        newWidth = 10;
        newHeight = 10;
        useAutoLevelNumber = true;
    }
}

