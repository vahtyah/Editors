using UnityEngine;
using UnityEditor;

/// <summary>
/// Lưu trữ các style UI cho Level Editor để dễ dàng tái sử dụng
/// </summary>
public static class LevelEditorStyles
{
    private static GUIStyle headerStyle;
    private static GUIStyle subHeaderStyle;
    private static GUIStyle boxStyle;
    private static GUIStyle buttonStyle;
    private static GUIStyle toggleStyle;
    private static GUIStyle labelStyle;
    private static GUIStyle centeredLabelStyle;
    private static GUIStyle toolbarButtonStyle;
    private static GUIStyle helpBoxStyle;

    // Header Style - Tiêu đề lớn
    public static GUIStyle HeaderStyle
    {
        get
        {
            if (headerStyle == null)
            {
                headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 18,
                    alignment = TextAnchor.MiddleLeft,
                    fontStyle = FontStyle.Bold,
                    padding = new RectOffset(10, 10, 10, 10)
                };
            }
            return headerStyle;
        }
    }

    // Sub Header Style - Tiêu đề nhỏ
    public static GUIStyle SubHeaderStyle
    {
        get
        {
            if (subHeaderStyle == null)
            {
                subHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 14,
                    alignment = TextAnchor.MiddleLeft,
                    fontStyle = FontStyle.Bold,
                    padding = new RectOffset(5, 5, 5, 5)
                };
            }
            return subHeaderStyle;
        }
    }

    // Box Style - Hộp chứa nội dung
    public static GUIStyle BoxStyle
    {
        get
        {
            if (boxStyle == null)
            {
                boxStyle = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(10, 10, 10, 10),
                    margin = new RectOffset(5, 5, 5, 5)
                };
            }
            return boxStyle;
        }
    }

    // Button Style - Nút bấm
    public static GUIStyle ButtonStyle
    {
        get
        {
            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    padding = new RectOffset(10, 10, 5, 5)
                };
            }
            return buttonStyle;
        }
    }

    // Toggle Style - Checkbox
    public static GUIStyle ToggleStyle
    {
        get
        {
            if (toggleStyle == null)
            {
                toggleStyle = new GUIStyle(EditorStyles.toggle)
                {
                    fontSize = 11,
                    padding = new RectOffset(20, 5, 2, 2)
                };
            }
            return toggleStyle;
        }
    }

    // Label Style - Nhãn thông thường
    public static GUIStyle LabelStyle
    {
        get
        {
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(EditorStyles.label)
                {
                    fontSize = 11,
                    wordWrap = true
                };
            }
            return labelStyle;
        }
    }

    // Centered Label Style - Nhãn canh giữa
    public static GUIStyle CenteredLabelStyle
    {
        get
        {
            if (centeredLabelStyle == null)
            {
                centeredLabelStyle = new GUIStyle(EditorStyles.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 11,
                    wordWrap = true
                };
            }
            return centeredLabelStyle;
        }
    }

    // Toolbar Button Style
    public static GUIStyle ToolbarButtonStyle
    {
        get
        {
            if (toolbarButtonStyle == null)
            {
                toolbarButtonStyle = new GUIStyle(EditorStyles.toolbarButton)
                {
                    fontSize = 11,
                    fontStyle = FontStyle.Bold
                };
            }
            return toolbarButtonStyle;
        }
    }

    // Help Box Style
    public static GUIStyle HelpBoxStyle
    {
        get
        {
            if (helpBoxStyle == null)
            {
                helpBoxStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    fontSize = 11,
                    padding = new RectOffset(10, 10, 10, 10),
                    margin = new RectOffset(5, 5, 5, 5)
                };
            }
            return helpBoxStyle;
        }
    }

    // Colors
    public static class Colors
    {
        public static Color Primary = new Color(0.2f, 0.6f, 1f, 1f);
        public static Color Success = new Color(0.2f, 0.8f, 0.2f, 1f);
        public static Color Warning = new Color(1f, 0.8f, 0.2f, 1f);
        public static Color Error = new Color(1f, 0.3f, 0.3f, 1f);
        public static Color Background = new Color(0.22f, 0.22f, 0.22f, 1f);
        public static Color Border = new Color(0.15f, 0.15f, 0.15f, 1f);
    }

    // Spacing
    public static class Spacing
    {
        public const float Small = 5f;
        public const float Medium = 10f;
        public const float Large = 20f;
    }

    // Sizes
    public static class Sizes
    {
        public const float ButtonHeight = 30f;
        public const float FieldHeight = 18f;
        public const float IconSize = 20f;
        public const float ThumbnailSize = 64f;
    }
}

