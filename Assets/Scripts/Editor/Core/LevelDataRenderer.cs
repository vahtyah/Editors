using UnityEngine;
using UnityEditor;

/// <summary>
/// Renderer class để vẽ LevelData - Single Responsibility Principle
/// Chỉ chịu trách nhiệm render UI cho LevelData
/// </summary>
public class LevelDataRenderer
{
    private const float CELL_SIZE = 20f;
    private GUIStyle cellStyle;

    public LevelDataRenderer()
    {
        InitializeStyles();
    }

    private void InitializeStyles()
    {
        cellStyle = new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 10
        };
    }

    /// <summary>
    /// Vẽ layout preview của LevelData
    /// </summary>
    public void DrawLayoutPreview(LevelData levelData)
    {
        if (levelData == null || levelData.layout == null)
            return;

        EditorGUILayout.BeginVertical();

        // Draw background color indicator
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Background:", GUILayout.Width(80));
        Rect colorRect = EditorGUILayout.GetControlRect(GUILayout.Width(50), GUILayout.Height(20));
        EditorGUI.DrawRect(colorRect, levelData.backgroundColor);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        // Draw grid
        for (int row = 0; row < levelData.layout.Length; row++)
        {
            if (levelData.layout[row] == null)
                continue;

            EditorGUILayout.BeginHorizontal();

            for (int col = 0; col < levelData.layout[row].Length; col++)
            {
                char cell = levelData.layout[row][col];
                DrawCell(cell, row, col);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();

        // Draw legend
        EditorGUILayout.Space(5);
        DrawLegend();
    }

    private void DrawCell(char cell, int row, int col)
    {
        Color cellColor = GetCellColor(cell);
        Color originalColor = GUI.backgroundColor;
        GUI.backgroundColor = cellColor;

        GUILayout.Box(cell.ToString(), cellStyle, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));

        GUI.backgroundColor = originalColor;
    }

    private Color GetCellColor(char cell)
    {
        switch (cell)
        {
            case '.': return new Color(0.9f, 0.9f, 0.9f); // Empty
            case '#': return new Color(0.3f, 0.3f, 0.3f); // Wall
            case 'P': return new Color(0.2f, 0.8f, 0.2f); // Player
            case 'E': return new Color(0.8f, 0.2f, 0.2f); // Enemy
            case 'C': return new Color(1f, 0.8f, 0.2f);   // Collectible
            case 'S': return new Color(0.2f, 0.6f, 1f);   // Special
            default: return Color.white;
        }
    }

    private void DrawLegend()
    {
        EditorGUILayout.BeginVertical(LevelEditorStyles.BoxStyle);
        EditorGUILayout.LabelField("Legend:", EditorStyles.boldLabel);
        
        DrawLegendItem('.', "Empty", new Color(0.9f, 0.9f, 0.9f));
        DrawLegendItem('#', "Wall", new Color(0.3f, 0.3f, 0.3f));
        DrawLegendItem('P', "Player", new Color(0.2f, 0.8f, 0.2f));
        DrawLegendItem('E', "Enemy", new Color(0.8f, 0.2f, 0.2f));
        DrawLegendItem('C', "Collectible", new Color(1f, 0.8f, 0.2f));
        DrawLegendItem('S', "Special", new Color(0.2f, 0.6f, 1f));
        
        EditorGUILayout.EndVertical();
    }

    private void DrawLegendItem(char symbol, string description, Color color)
    {
        EditorGUILayout.BeginHorizontal();
        
        Color originalColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        GUILayout.Box(symbol.ToString(), cellStyle, GUILayout.Width(20), GUILayout.Height(20));
        GUI.backgroundColor = originalColor;
        
        EditorGUILayout.LabelField($"= {description}");
        
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Vẽ compact preview (thumbnail) của level
    /// </summary>
    public void DrawCompactPreview(LevelData levelData, float maxWidth, float maxHeight)
    {
        if (levelData == null || levelData.layout == null)
            return;

        float cellSize = Mathf.Min(maxWidth / levelData.width, maxHeight / levelData.height);
        cellSize = Mathf.Max(cellSize, 2f); // Minimum 2 pixels

        Rect previewRect = GUILayoutUtility.GetRect(levelData.width * cellSize, levelData.height * cellSize);
        
        // Draw background
        EditorGUI.DrawRect(previewRect, levelData.backgroundColor);

        // Draw cells
        for (int row = 0; row < levelData.layout.Length; row++)
        {
            if (levelData.layout[row] == null)
                continue;

            for (int col = 0; col < levelData.layout[row].Length; col++)
            {
                char cell = levelData.layout[row][col];
                if (cell == '.')
                    continue; // Skip empty cells

                Rect cellRect = new Rect(
                    previewRect.x + col * cellSize,
                    previewRect.y + row * cellSize,
                    cellSize,
                    cellSize
                );

                EditorGUI.DrawRect(cellRect, GetCellColor(cell));
            }
        }
    }
}

