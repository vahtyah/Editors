﻿using UnityEngine;
using UnityEditor;

public class PropertiesPanel : EditorPanel
{
    public override string Name => "Properties";

    private GameObject selectedObject;
    private Editor gameObjectEditor;
    private bool showTransform = true;
    private bool showComponents = true;

    public override void OnEnable()
    {
        Selection.selectionChanged += OnSelectionChanged;
        OnSelectionChanged();
    }

    public override void OnDisable()
    {
        Selection.selectionChanged -= OnSelectionChanged;
        
        if (gameObjectEditor != null)
        {
            Object.DestroyImmediate(gameObjectEditor);
        }
    }

    public override void Draw()
    {
        EditorGUIHelper.DrawHeader("Properties Panel");
        EditorGUIHelper.DrawHelpBox("Xem và chỉnh sửa properties của object đã chọn.", MessageType.Info);
        
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Medium);

        if (selectedObject == null)
        {
            DrawNoSelection();
        }
        else
        {
            DrawObjectInfo();
            EditorGUILayout.Space(LevelEditorStyles.Spacing.Medium);
            
            DrawTransformProperties();
            EditorGUILayout.Space(LevelEditorStyles.Spacing.Medium);
            
            DrawComponentsList();
        }
    }

    private void OnSelectionChanged()
    {
        selectedObject = Selection.activeGameObject;
        
        if (gameObjectEditor != null)
        {
            Object.DestroyImmediate(gameObjectEditor);
        }
        
        if (selectedObject != null)
        {
            gameObjectEditor = Editor.CreateEditor(selectedObject);
        }
    }

    private void DrawNoSelection()
    {
        EditorGUIHelper.DrawBox("", () =>
        {
            EditorGUIHelper.DrawCenteredLabel("Chưa chọn object nào");
            EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);
            EditorGUIHelper.DrawCenteredLabel("Vui lòng chọn một object trong Scene");
        });
    }

    private void DrawObjectInfo()
    {
        EditorGUIHelper.DrawBox("Object Info", () =>
        {
            selectedObject.name = EditorGUILayout.TextField("Name:", selectedObject.name);
            EditorGUILayout.LabelField("Tag:", selectedObject.tag);
            EditorGUILayout.LabelField("Layer:", LayerMask.LayerToName(selectedObject.layer));
            
            EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);
            
            EditorGUILayout.BeginHorizontal();
            selectedObject.SetActive(EditorGUILayout.Toggle("Active", selectedObject.activeSelf));
            EditorGUILayout.EndHorizontal();
        });
    }

    private void DrawTransformProperties()
    {
        showTransform = EditorGUIHelper.DrawFoldout(showTransform, "Transform");
        
        if (showTransform)
        {
            EditorGUIHelper.DrawBox("", () =>
            {
                Transform t = selectedObject.transform;
                
                EditorGUI.BeginChangeCheck();
                
                Vector3 position = EditorGUILayout.Vector3Field("Position:", t.localPosition);
                Vector3 rotation = EditorGUILayout.Vector3Field("Rotation:", t.localEulerAngles);
                Vector3 scale = EditorGUILayout.Vector3Field("Scale:", t.localScale);
                
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(t, "Transform Change");
                    t.localPosition = position;
                    t.localEulerAngles = rotation;
                    t.localScale = scale;
                }
                
                EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);
                
                EditorGUILayout.BeginHorizontal();
                if (EditorGUIHelper.DrawButton("Reset Position", 120))
                {
                    Undo.RecordObject(t, "Reset Position");
                    t.localPosition = Vector3.zero;
                }
                if (EditorGUIHelper.DrawButton("Reset Rotation", 120))
                {
                    Undo.RecordObject(t, "Reset Rotation");
                    t.localEulerAngles = Vector3.zero;
                }
                if (EditorGUIHelper.DrawButton("Reset Scale", 120))
                {
                    Undo.RecordObject(t, "Reset Scale");
                    t.localScale = Vector3.one;
                }
                EditorGUILayout.EndHorizontal();
            });
        }
    }

    private void DrawComponentsList()
    {
        showComponents = EditorGUIHelper.DrawFoldout(showComponents, "Components");
        
        if (showComponents)
        {
            EditorGUIHelper.DrawBox("", () =>
            {
                Component[] components = selectedObject.GetComponents<Component>();
                
                foreach (Component component in components)
                {
                    if (component == null)
                        continue;
                    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(component.GetType().Name, EditorStyles.boldLabel);
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUIHelper.DrawSeparator();
                }
                
                EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);
                
                if (EditorGUIHelper.DrawColoredButton("Add Component", LevelEditorStyles.Colors.Success, 0, LevelEditorStyles.Sizes.ButtonHeight))
                {
                    // Mở menu add component
                    EditorApplication.ExecuteMenuItem("Component");
                }
            });
        }
    }
}

