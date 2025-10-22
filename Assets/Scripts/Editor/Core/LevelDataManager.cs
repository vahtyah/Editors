using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Service class để quản lý LevelData - Single Responsibility Principle
/// </summary>
public class LevelDataManager
{
    private const string LEVEL_DATA_PATH = "Assets/ScriptableObjects/Levels";
    
    /// <summary>
    /// Load tất cả LevelData từ project
    /// </summary>
    public List<LevelData> LoadAllLevelData()
    {
        string[] guids = AssetDatabase.FindAssets("t:LevelData");
        List<LevelData> levelDataList = new List<LevelData>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            LevelData levelData = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            if (levelData != null)
            {
                levelDataList.Add(levelData);
            }
        }

        // Sort by level number
        return levelDataList.OrderBy(x => x.levelNumber).ToList();
    }

    /// <summary>
    /// Tạo LevelData mới
    /// </summary>
    public LevelData CreateNewLevelData(int levelNumber, int width, int height)
    {
        // Tạo folder nếu chưa tồn tại
        if (!AssetDatabase.IsValidFolder(LEVEL_DATA_PATH))
        {
            string[] folders = LEVEL_DATA_PATH.Split('/');
            string currentPath = folders[0];
            
            for (int i = 1; i < folders.Length; i++)
            {
                string newPath = currentPath + "/" + folders[i];
                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    AssetDatabase.CreateFolder(currentPath, folders[i]);
                }
                currentPath = newPath;
            }
        }

        // Tạo LevelData
        LevelData newLevelData = ScriptableObject.CreateInstance<LevelData>();
        newLevelData.levelNumber = levelNumber;
        newLevelData.width = width;
        newLevelData.height = height;
        newLevelData.backgroundColor = Color.white;
        newLevelData.layout = new string[height];
        
        // Initialize empty layout
        for (int i = 0; i < height; i++)
        {
            newLevelData.layout[i] = new string('.', width);
        }

        // Save asset
        string assetPath = $"{LEVEL_DATA_PATH}/Level_{levelNumber}.asset";
        assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
        
        AssetDatabase.CreateAsset(newLevelData, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return newLevelData;
    }

    /// <summary>
    /// Xóa LevelData
    /// </summary>
    public bool DeleteLevelData(LevelData levelData)
    {
        if (levelData == null)
            return false;

        string path = AssetDatabase.GetAssetPath(levelData);
        if (string.IsNullOrEmpty(path))
            return false;

        bool result = AssetDatabase.DeleteAsset(path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return result;
    }

    /// <summary>
    /// Duplicate LevelData
    /// </summary>
    public LevelData DuplicateLevelData(LevelData source)
    {
        if (source == null)
            return null;

        LevelData duplicate = ScriptableObject.CreateInstance<LevelData>();
        duplicate.levelNumber = source.levelNumber;
        duplicate.width = source.width;
        duplicate.height = source.height;
        duplicate.backgroundColor = source.backgroundColor;
        
        // Copy layout
        duplicate.layout = new string[source.layout.Length];
        System.Array.Copy(source.layout, duplicate.layout, source.layout.Length);

        // Save asset
        string sourcePath = AssetDatabase.GetAssetPath(source);
        string directory = System.IO.Path.GetDirectoryName(sourcePath);
        string fileName = System.IO.Path.GetFileNameWithoutExtension(sourcePath);
        string newPath = $"{directory}/{fileName}_Copy.asset";
        newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);

        AssetDatabase.CreateAsset(duplicate, newPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return duplicate;
    }

    /// <summary>
    /// Apply changes to LevelData
    /// </summary>
    public void ApplyChanges(LevelData levelData)
    {
        if (levelData == null)
            return;

        EditorUtility.SetDirty(levelData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// Get next available level number
    /// </summary>
    public int GetNextLevelNumber()
    {
        List<LevelData> allLevels = LoadAllLevelData();
        if (allLevels.Count == 0)
            return 1;

        return allLevels.Max(x => x.levelNumber) + 1;
    }

    /// <summary>
    /// Validate LevelData
    /// </summary>
    public bool ValidateLevelData(LevelData levelData, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (levelData == null)
        {
            errorMessage = "LevelData is null";
            return false;
        }

        if (levelData.width <= 0 || levelData.height <= 0)
        {
            errorMessage = "Width and Height must be greater than 0";
            return false;
        }

        if (levelData.layout == null || levelData.layout.Length != levelData.height)
        {
            errorMessage = "Layout array length doesn't match height";
            return false;
        }

        for (int i = 0; i < levelData.layout.Length; i++)
        {
            if (levelData.layout[i] == null || levelData.layout[i].Length != levelData.width)
            {
                errorMessage = $"Layout row {i} length doesn't match width";
                return false;
            }
        }

        return true;
    }
}

