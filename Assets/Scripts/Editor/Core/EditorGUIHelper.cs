using UnityEngine;
using UnityEditor;

/// <summary>
/// Helper class để vẽ các UI elements phổ biến trong Level Editor
/// </summary>
public static class EditorGUIHelper
{
    /// <summary>
    /// Vẽ header với separator line
    /// </summary>
    public static void DrawHeader(string title)
    {
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);
        EditorGUILayout.LabelField(title, LevelEditorStyles.HeaderStyle);
        DrawSeparator();
        EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);
    }

    /// <summary>
    /// Vẽ sub header
    /// </summary>
    public static void DrawSubHeader(string title)
    {
        EditorGUILayout.LabelField(title, LevelEditorStyles.SubHeaderStyle);
    }

    /// <summary>
    /// Vẽ đường phân cách
    /// </summary>
    public static void DrawSeparator(int height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, height);
        rect.height = height;
        EditorGUI.DrawRect(rect, LevelEditorStyles.Colors.Border);
    }

    /// <summary>
    /// Vẽ box với title và content
    /// </summary>
    public static void DrawBox(string title, System.Action drawContent)
    {
        try
        {
            EditorGUILayout.BeginVertical(LevelEditorStyles.BoxStyle);
            
            if (!string.IsNullOrEmpty(title))
            {
                DrawSubHeader(title);
                EditorGUILayout.Space(LevelEditorStyles.Spacing.Small);
            }
            
            if (drawContent != null)
            {
                drawContent.Invoke();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in DrawBox: {e.Message}\n{e.StackTrace}");
        }
        finally
        {
            EditorGUILayout.EndVertical();
        }
    }

    /// <summary>
    /// Vẽ button với icon
    /// </summary>
    public static bool DrawButton(string text, float width = 0, float height = 0)
    {
        GUILayoutOption[] options = width > 0 && height > 0
            ? new[] { GUILayout.Width(width), GUILayout.Height(height) }
            : width > 0
                ? new[] { GUILayout.Width(width) }
                : height > 0
                    ? new[] { GUILayout.Height(height) }
                    : new GUILayoutOption[0];

        return GUILayout.Button(text, LevelEditorStyles.ButtonStyle, options);
    }

    /// <summary>
    /// Vẽ button có màu
    /// </summary>
    public static bool DrawColoredButton(string text, Color color, float width = 0, float height = 0)
    {
        Color originalColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        
        bool result = DrawButton(text, width, height);
        
        GUI.backgroundColor = originalColor;
        return result;
    }

    /// <summary>
    /// Vẽ centered label
    /// </summary>
    public static void DrawCenteredLabel(string text)
    {
        EditorGUILayout.LabelField(text, LevelEditorStyles.CenteredLabelStyle);
    }

    /// <summary>
    /// Vẽ help box với custom style
    /// </summary>
    public static void DrawHelpBox(string message, MessageType type = MessageType.Info)
    {
        EditorGUILayout.HelpBox(message, type);
    }

    /// <summary>
    /// Vẽ field với label rộng hơn
    /// </summary>
    public static float DrawFloatField(string label, float value, float labelWidth = 150f)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
        float result = EditorGUILayout.FloatField(value);
        EditorGUILayout.EndHorizontal();
        return result;
    }

    /// <summary>
    /// Vẽ int field với label
    /// </summary>
    public static int DrawIntField(string label, int value, float labelWidth = 150f)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
        int result = EditorGUILayout.IntField(value);
        EditorGUILayout.EndHorizontal();
        return result;
    }

    /// <summary>
    /// Vẽ object field với label
    /// </summary>
    public static T DrawObjectField<T>(string label, T obj, bool allowSceneObjects = true, float labelWidth = 150f) where T : Object
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
        T result = (T)EditorGUILayout.ObjectField(obj, typeof(T), allowSceneObjects);
        EditorGUILayout.EndHorizontal();
        return result;
    }

    /// <summary>
    /// Vẽ foldout với custom style
    /// </summary>
    public static bool DrawFoldout(bool foldout, string label)
    {
        GUIStyle style = new GUIStyle(EditorStyles.foldout)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 12
        };
        return EditorGUILayout.Foldout(foldout, label, true, style);
    }

    /// <summary>
    /// Vẽ toolbar với các buttons
    /// </summary>
    public static int DrawToolbar(int selected, string[] labels)
    {
        return GUILayout.Toolbar(selected, labels, LevelEditorStyles.ToolbarButtonStyle);
    }

    /// <summary>
    /// Vẽ grid với các items
    /// </summary>
    public static int DrawGrid(int selected, Texture2D[] textures, int itemsPerRow, float itemSize)
    {
        int columns = itemsPerRow;
        int rows = Mathf.CeilToInt((float)textures.Length / columns);

        int newSelected = selected;

        for (int row = 0; row < rows; row++)
        {
            EditorGUILayout.BeginHorizontal();
            
            for (int col = 0; col < columns; col++)
            {
                int index = row * columns + col;
                if (index >= textures.Length)
                    break;

                bool isSelected = index == selected;
                Color originalColor = GUI.backgroundColor;
                
                if (isSelected)
                    GUI.backgroundColor = LevelEditorStyles.Colors.Primary;

                if (GUILayout.Button(textures[index], GUILayout.Width(itemSize), GUILayout.Height(itemSize)))
                {
                    newSelected = index;
                }

                GUI.backgroundColor = originalColor;
            }
            
            EditorGUILayout.EndHorizontal();
        }

        return newSelected;
    }

    /// <summary>
    /// Vẽ progress bar
    /// </summary>
    public static void DrawProgressBar(float progress, string label = "")
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 20);
        EditorGUI.ProgressBar(rect, progress, label);
    }

    /// <summary>
    /// Vẽ horizontal line với label ở giữa
    /// </summary>
    public static void DrawLabelWithLine(string label)
    {
        EditorGUILayout.BeginHorizontal();
        
        DrawSeparator();
        EditorGUILayout.LabelField(label, LevelEditorStyles.CenteredLabelStyle, GUILayout.Width(100));
        DrawSeparator();
        
        EditorGUILayout.EndHorizontal();
    }
}

