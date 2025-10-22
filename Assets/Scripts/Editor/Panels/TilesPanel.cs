﻿using UnityEngine;
using UnityEditor;

public class TilesPanel : EditorPanel
{
    public override string Name => "Tiles";

    private int selectedTileIndex = -1;
    private bool showPaletteSettings = true;

    public override void OnEnable()
    {
        // Khởi tạo dữ liệu cho Tiles Panel
        selectedTileIndex = -1;
    }

    public override void OnDisable()
    {
        // Dọn dẹp resources khi panel bị disable
    }

    public override void Draw()
    {
        EditorGUIHelper.DrawHeader("Tiles Panel");
        EditorGUIHelper.DrawHelpBox("Quản lý tiles trong level của bạn.", MessageType.Info);
        
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Medium);

        DrawTilePalette();
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Medium);

        DrawTileProperties();
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Medium);

        DrawTileActions();
    }

    private void DrawTilePalette()
    {
        showPaletteSettings = EditorGUIHelper.DrawFoldout(showPaletteSettings, "Tile Palette");
        
        if (showPaletteSettings)
        {
            EditorGUIHelper.DrawBox("", () =>
            {
                EditorGUIHelper.DrawCenteredLabel("Chưa có tiles nào được load");
                EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);
                
                if (EditorGUIHelper.DrawButton("Load Tile Set", 120, LevelEditorStyles.Sizes.ButtonHeight))
                {
                    // TODO: Implement load tile set
                    Debug.Log("Load Tile Set clicked");
                }
            });
        }
    }

    private void DrawTileProperties()
    {
        EditorGUIHelper.DrawBox("Tile Properties", () =>
        {
            if (selectedTileIndex >= 0)
            {
                EditorGUILayout.LabelField("Selected Tile:", selectedTileIndex.ToString());
                // TODO: Hiển thị properties của tile được chọn
            }
            else
            {
                EditorGUIHelper.DrawCenteredLabel("Chưa chọn tile nào");
            }
        });
    }

    private void DrawTileActions()
    {
        EditorGUIHelper.DrawBox("Actions", () =>
        {
            EditorGUILayout.BeginHorizontal();
            
            if (EditorGUIHelper.DrawColoredButton("Paint", LevelEditorStyles.Colors.Primary, 0, LevelEditorStyles.Sizes.ButtonHeight))
            {
                Debug.Log("Paint mode");
            }
            
            if (EditorGUIHelper.DrawColoredButton("Erase", LevelEditorStyles.Colors.Warning, 0, LevelEditorStyles.Sizes.ButtonHeight))
            {
                Debug.Log("Erase mode");
            }
            
            if (EditorGUIHelper.DrawColoredButton("Fill", LevelEditorStyles.Colors.Success, 0, LevelEditorStyles.Sizes.ButtonHeight))
            {
                Debug.Log("Fill mode");
            }
            
            EditorGUILayout.EndHorizontal();
        });
    }
}

