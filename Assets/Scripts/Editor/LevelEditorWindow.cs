﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class LevelEditorWindow : EditorWindow
{
    private PanelType currentPanel = PanelType.Levels;
    private Vector2 scrollPosition;
    private Dictionary<PanelType, EditorPanel> panels;

    [MenuItem("Window/Level Editor")]
    public static void ShowWindow()
    {
        LevelEditorWindow window = GetWindow<LevelEditorWindow>("Level Editor");
        window.minSize = new Vector2(800, 600);
        window.Show();
    }

    private void OnEnable()
    {
        InitializePanels();
    }

    private void OnDisable()
    {
        if (panels != null)
        {
            foreach (var panel in panels.Values)
            {
                panel.OnDisable();
            }
        }
    }

    private void InitializePanels()
    {
        panels = new Dictionary<PanelType, EditorPanel>
        {
            { PanelType.Levels, new LevelsPanel() },
            { PanelType.Tiles, new TilesPanel() },
            { PanelType.Objects, new ObjectsPanel() },
            { PanelType.Settings, new SettingsPanel() },
            { PanelType.Properties, new PropertiesPanel() }
        };

        foreach (var panel in panels.Values)
        {
            panel.OnEnable();
        }
    }

    private void OnGUI()
    {
        DrawMenuBar();
        
        EditorGUILayout.Space(5);
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        if (panels != null && panels.ContainsKey(currentPanel))
        {
            panels[currentPanel].Draw();
        }
        
        EditorGUILayout.EndScrollView();
    }

    private void DrawMenuBar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        foreach (var kvp in panels)
        {
            if (GUILayout.Toggle(currentPanel == kvp.Key, kvp.Value.Name, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true)))
            {
                currentPanel = kvp.Key;
            }
        }

        EditorGUILayout.EndHorizontal();
    }
}

