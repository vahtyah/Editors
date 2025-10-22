using UnityEngine;
using UnityEditor;

/// <summary>
/// Helper class để lưu trữ và lấy dữ liệu từ EditorPrefs
/// </summary>
public static class EditorPrefsHelper
{
    public static void SetFloat(string key, float value)
    {
        EditorPrefs.SetFloat(key, value);
    }

    public static float GetFloat(string key, float defaultValue)
    {
        return EditorPrefs.GetFloat(key, defaultValue);
    }

    public static void SetInt(string key, int value)
    {
        EditorPrefs.SetInt(key, value);
    }

    public static int GetInt(string key, int defaultValue)
    {
        return EditorPrefs.GetInt(key, defaultValue);
    }

    public static void SetBool(string key, bool value)
    {
        EditorPrefs.SetBool(key, value);
    }

    public static bool GetBool(string key, bool defaultValue)
    {
        return EditorPrefs.GetBool(key, defaultValue);
    }

    public static void SetString(string key, string value)
    {
        EditorPrefs.SetString(key, value);
    }

    public static string GetString(string key, string defaultValue)
    {
        return EditorPrefs.GetString(key, defaultValue);
    }

    public static void SetColor(string key, Color color)
    {
        EditorPrefs.SetString(key, ColorUtility.ToHtmlStringRGBA(color));
    }

    public static Color GetColor(string key, Color defaultValue)
    {
        string colorString = EditorPrefs.GetString(key, "");
        if (string.IsNullOrEmpty(colorString))
            return defaultValue;

        if (ColorUtility.TryParseHtmlString("#" + colorString, out Color color))
            return color;

        return defaultValue;
    }

    public static void SetVector2(string key, Vector2 vector)
    {
        EditorPrefs.SetFloat(key + "_x", vector.x);
        EditorPrefs.SetFloat(key + "_y", vector.y);
    }

    public static Vector2 GetVector2(string key, Vector2 defaultValue)
    {
        return new Vector2(
            EditorPrefs.GetFloat(key + "_x", defaultValue.x),
            EditorPrefs.GetFloat(key + "_y", defaultValue.y)
        );
    }

    public static void SetVector3(string key, Vector3 vector)
    {
        EditorPrefs.SetFloat(key + "_x", vector.x);
        EditorPrefs.SetFloat(key + "_y", vector.y);
        EditorPrefs.SetFloat(key + "_z", vector.z);
    }

    public static Vector3 GetVector3(string key, Vector3 defaultValue)
    {
        return new Vector3(
            EditorPrefs.GetFloat(key + "_x", defaultValue.x),
            EditorPrefs.GetFloat(key + "_y", defaultValue.y),
            EditorPrefs.GetFloat(key + "_z", defaultValue.z)
        );
    }
}

