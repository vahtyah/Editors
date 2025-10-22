using UnityEngine;

/// <summary>
/// Lưu trữ các settings cho Level Editor
/// </summary>
public class LevelEditorSettings
{
    private const string PREF_PREFIX = "LevelEditor_";

    // Grid Settings
    public float GridSize
    {
        get => EditorPrefsHelper.GetFloat(PREF_PREFIX + "GridSize", 1f);
        set => EditorPrefsHelper.SetFloat(PREF_PREFIX + "GridSize", value);
    }

    public bool ShowGrid
    {
        get => EditorPrefsHelper.GetBool(PREF_PREFIX + "ShowGrid", true);
        set => EditorPrefsHelper.SetBool(PREF_PREFIX + "ShowGrid", value);
    }

    public Color GridColor
    {
        get => EditorPrefsHelper.GetColor(PREF_PREFIX + "GridColor", new Color(0.5f, 0.5f, 0.5f, 0.3f));
        set => EditorPrefsHelper.SetColor(PREF_PREFIX + "GridColor", value);
    }

    // Snap Settings
    public bool EnableSnapping
    {
        get => EditorPrefsHelper.GetBool(PREF_PREFIX + "EnableSnapping", true);
        set => EditorPrefsHelper.SetBool(PREF_PREFIX + "EnableSnapping", value);
    }

    public float SnapDistance
    {
        get => EditorPrefsHelper.GetFloat(PREF_PREFIX + "SnapDistance", 0.5f);
        set => EditorPrefsHelper.SetFloat(PREF_PREFIX + "SnapDistance", value);
    }

    // Brush Settings
    public int BrushSize
    {
        get => EditorPrefsHelper.GetInt(PREF_PREFIX + "BrushSize", 1);
        set => EditorPrefsHelper.SetInt(PREF_PREFIX + "BrushSize", value);
    }

    public bool EnableBrushPreview
    {
        get => EditorPrefsHelper.GetBool(PREF_PREFIX + "EnableBrushPreview", true);
        set => EditorPrefsHelper.SetBool(PREF_PREFIX + "EnableBrushPreview", value);
    }

    // View Settings
    public bool ShowGizmos
    {
        get => EditorPrefsHelper.GetBool(PREF_PREFIX + "ShowGizmos", true);
        set => EditorPrefsHelper.SetBool(PREF_PREFIX + "ShowGizmos", value);
    }

    public float CameraSpeed
    {
        get => EditorPrefsHelper.GetFloat(PREF_PREFIX + "CameraSpeed", 5f);
        set => EditorPrefsHelper.SetFloat(PREF_PREFIX + "CameraSpeed", value);
    }

    // Singleton instance
    private static LevelEditorSettings instance;
    public static LevelEditorSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LevelEditorSettings();
            }
            return instance;
        }
    }

    // Reset về mặc định
    public void ResetToDefaults()
    {
        GridSize = 1f;
        ShowGrid = true;
        GridColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        EnableSnapping = true;
        SnapDistance = 0.5f;
        BrushSize = 1;
        EnableBrushPreview = true;
        ShowGizmos = true;
        CameraSpeed = 5f;
    }
}

