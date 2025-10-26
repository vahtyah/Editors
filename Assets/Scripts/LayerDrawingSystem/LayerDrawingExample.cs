using UnityEngine;
using UnityEditor;

namespace CustomLayerDrawing.Examples
{
    /// <summary>
    /// Example: Tạo custom inspector với Layer Drawing
    /// Attach MonoBehaviour này vào GameObject để thấy custom inspector
    /// </summary>
    public class LayerDrawingExample : MonoBehaviour
    {
        [Header("Layer Configuration")]
        public LayerConfiguration headerBackground;
        public LayerConfiguration contentBackground;
        public LayerConfiguration footerBackground;

        [Header("Demo Data")]
        public string headerText = "Custom Header";
        public string contentText = "This is content area with layer background";
        public bool showFooter = true;

        private void Reset()
        {
            // Khởi tạo mặc định khi component được thêm vào
            InitializeDefaultConfigs();
        }

        private void InitializeDefaultConfigs()
        {
            // Header: Gradient background với border
            headerBackground = new LayerConfiguration(2);
            headerBackground.layers[0] = Layer.CreateGradient(
                new Color(0.2f, 0.4f, 0.6f),
                new Color(0.15f, 0.3f, 0.45f),
                GradientDirection.Vertical
            );
            headerBackground.layers[1] = Layer.CreateBorder(
                new Color(0.5f, 0.7f, 0.9f),
                1f,
                0f
            );

            // Content: Simple background với padding
            contentBackground = LayerConfiguration.CreateSimpleBackground(new Color(0.25f, 0.25f, 0.25f));

            // Footer: Card style
            footerBackground = LayerConfiguration.CreateCardStyle(
                new Color(0.3f, 0.3f, 0.3f),
                new Color(0f, 0f, 0f, 0.3f),
                5f
            );
        }
    }

    /// <summary>
    /// Custom Inspector cho LayerDrawingExample
    /// Hiển thị preview của layer backgrounds
    /// </summary>
    [CustomEditor(typeof(LayerDrawingExample))]
    public class LayerDrawingExampleEditor : Editor
    {
        private LayerDrawingExample example;

        private void OnEnable()
        {
            example = (LayerDrawingExample)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("LAYER DRAWING EXAMPLE", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // Draw preview với layer backgrounds
            DrawPreview();

            EditorGUILayout.Space(10);

            // Draw default inspector
            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPreview()
        {
            EditorGUILayout.LabelField("Preview:", EditorStyles.boldLabel);

            // Header Preview
            if (example.headerBackground != null)
            {
                Rect headerRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(40), GUILayout.ExpandWidth(true));
                LayerDrawingSystem.DrawLayers(headerRect, example.headerBackground);
                
                GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
                headerStyle.alignment = TextAnchor.MiddleCenter;
                headerStyle.fontStyle = FontStyle.Bold;
                headerStyle.normal.textColor = Color.white;
                GUI.Label(headerRect, example.headerText, headerStyle);
            }

            // Content Preview
            if (example.contentBackground != null)
            {
                Rect contentRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(60), GUILayout.ExpandWidth(true));
                LayerDrawingSystem.DrawLayers(contentRect, example.contentBackground);
                
                GUIStyle contentStyle = new GUIStyle(GUI.skin.label);
                contentStyle.alignment = TextAnchor.MiddleCenter;
                contentStyle.normal.textColor = Color.white;
                contentStyle.wordWrap = true;
                Rect textRect = new Rect(contentRect.x + 10, contentRect.y, contentRect.width - 20, contentRect.height);
                GUI.Label(textRect, example.contentText, contentStyle);
            }

            // Footer Preview
            if (example.showFooter && example.footerBackground != null)
            {
                Rect footerRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(30), GUILayout.ExpandWidth(true));
                footerRect.x += 5;
                footerRect.width -= 10;
                LayerDrawingSystem.DrawLayers(footerRect, example.footerBackground);
                
                GUIStyle footerStyle = new GUIStyle(GUI.skin.label);
                footerStyle.alignment = TextAnchor.MiddleCenter;
                footerStyle.normal.textColor = Color.gray;
                GUI.Label(footerRect, "Footer Area", footerStyle);
            }
        }
    }
}

