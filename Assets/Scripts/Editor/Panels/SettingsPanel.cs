﻿using UnityEngine;
using UnityEditor;

public class SettingsPanel : EditorPanel
{
    public override string Name => "Settings";

    private LevelEditorSettings settings;

    public override void OnEnable()
    {
        settings = LevelEditorSettings.Instance;
    }

    public override void OnDisable()
    {
        // Settings được lưu tự động thông qua EditorPrefs
    }

    public override void Draw()
    {
        EditorGUILayout.LabelField("Settings Panel", LevelEditorStyles.HeaderStyle);
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);
        
        EditorGUILayout.HelpBox("Cấu hình các settings của Level Editor.", MessageType.Info);
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Medium);

        DrawGridSettings();
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Medium);

        DrawSnapSettings();
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Medium);

        DrawBrushSettings();
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Medium);

        DrawViewSettings();
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Medium);

        DrawResetButton();
    }

    private void DrawGridSettings()
    {
        EditorGUILayout.BeginVertical(LevelEditorStyles.BoxStyle);
        EditorGUILayout.LabelField("Grid Settings", LevelEditorStyles.SubHeaderStyle);
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);

        settings.ShowGrid = EditorGUILayout.Toggle("Show Grid", settings.ShowGrid);
        settings.GridSize = EditorGUILayout.Slider("Grid Size", settings.GridSize, 0.1f, 10f);
        settings.GridColor = EditorGUILayout.ColorField("Grid Color", settings.GridColor);

        EditorGUILayout.EndVertical();
    }

    private void DrawSnapSettings()
    {
        EditorGUILayout.BeginVertical(LevelEditorStyles.BoxStyle);
        EditorGUILayout.LabelField("Snap Settings", LevelEditorStyles.SubHeaderStyle);
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);

        settings.EnableSnapping = EditorGUILayout.Toggle("Enable Snapping", settings.EnableSnapping);
        
        EditorGUI.BeginDisabledGroup(!settings.EnableSnapping);
        settings.SnapDistance = EditorGUILayout.Slider("Snap Distance", settings.SnapDistance, 0.1f, 5f);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndVertical();
    }

    private void DrawBrushSettings()
    {
        EditorGUILayout.BeginVertical(LevelEditorStyles.BoxStyle);
        EditorGUILayout.LabelField("Brush Settings", LevelEditorStyles.SubHeaderStyle);
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);

        settings.BrushSize = EditorGUILayout.IntSlider("Brush Size", settings.BrushSize, 1, 10);
        settings.EnableBrushPreview = EditorGUILayout.Toggle("Enable Brush Preview", settings.EnableBrushPreview);

        EditorGUILayout.EndVertical();
    }

    private void DrawViewSettings()
    {
        EditorGUILayout.BeginVertical(LevelEditorStyles.BoxStyle);
        EditorGUILayout.LabelField("View Settings", LevelEditorStyles.SubHeaderStyle);
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);

        settings.ShowGizmos = EditorGUILayout.Toggle("Show Gizmos", settings.ShowGizmos);
        settings.CameraSpeed = EditorGUILayout.Slider("Camera Speed", settings.CameraSpeed, 1f, 20f);

        EditorGUILayout.EndVertical();
    }

    private void DrawResetButton()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        GUI.backgroundColor = LevelEditorStyles.Colors.Warning;
        if (GUILayout.Button("Reset to Defaults", LevelEditorStyles.ButtonStyle, GUILayout.Height(LevelEditorStyles.Sizes.ButtonHeight), GUILayout.Width(150)))
        {
            if (EditorUtility.DisplayDialog("Reset Settings", "Bạn có chắc muốn reset tất cả settings về mặc định?", "Yes", "No"))
            {
                settings.ResetToDefaults();
            }
        }
        GUI.backgroundColor = Color.white;
        
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }
}

