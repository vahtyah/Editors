using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Menu item spawn nhanh GameObject demo cho VahTyah Feel. Cube được dựng runtime khi Play.
/// </summary>
public static class FeelDemoMenu
{
    private const string MENU_PATH = "Tools/VahTyah/Feel/Spawn Demo";

    [MenuItem(MENU_PATH, false, 1)]
    public static void SpawnDemo()
    {
        // Đã có sẵn thì chọn lại thay vì tạo trùng.
        FeelDemo existing = Object.FindObjectOfType<FeelDemo>();
        if (existing != null)
        {
            Selection.activeGameObject = existing.gameObject;
            EditorGUIUtility.PingObject(existing.gameObject);
            Debug.Log("[FeelDemo] Đã có Feel Demo trong scene — bấm Play để xem.");
            return;
        }

        GameObject go = new GameObject("Feel Demo");
        FeelDemo demo = go.AddComponent<FeelDemo>();

        Undo.RegisterCreatedObjectUndo(go, "Spawn Feel Demo");
        demo.SpawnCubes();

        Selection.activeGameObject = go;
        EditorGUIUtility.PingObject(go);
        EditorSceneManager.MarkSceneDirty(go.scene);

        Debug.Log("[FeelDemo] Đã tạo 'Feel Demo' + lưới cube (mỗi cube 1 feedback). Chỉnh trong Inspector, bấm Play để xem.");
    }
}
