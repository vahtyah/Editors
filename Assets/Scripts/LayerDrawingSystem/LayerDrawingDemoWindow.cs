using UnityEngine;
using UnityEditor;

namespace CustomLayerDrawing.Examples
{
    /// <summary>
    /// Demo window cho LayerDrawingSystem
    /// Mở bằng: Window > Layer Drawing Demo
    /// </summary>
    public class LayerDrawingDemoWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private Rect demoRect;

        // Các config mẫu
            private LayerConfiguration simpleBackground;
            private LayerConfiguration borderedBackground;
            private LayerConfiguration cardStyle;
            private LayerConfiguration customMultiLayer;

        [MenuItem("Window/Layer Drawing Demo")]
        public static void ShowWindow()
        {
            GetWindow<LayerDrawingDemoWindow>("Layer Drawing Demo");
        }

        private void OnEnable()
        {
            InitializeDemoConfigs();
        }

        private void InitializeDemoConfigs()
        {
            // 1. Simple Background
            simpleBackground = LayerConfiguration.CreateSimpleBackground(new Color(0.2f, 0.2f, 0.2f));

            // 2. Bordered Background
            borderedBackground = LayerConfiguration.CreateBackgroundWithBorder(
                new Color(0.3f, 0.3f, 0.3f),  // Background color
                new Color(0.5f, 0.5f, 0.5f),  // Border color
                2f,                            // Border width
                5f                             // Border radius
            );

            // 3. Card Style với Shadow
            cardStyle = LayerConfiguration.CreateCardStyle(
                new Color(0.25f, 0.25f, 0.25f), // Card color
                new Color(0f, 0f, 0f, 0.3f),    // Shadow color
                8f                               // Corner radius
            );

            // 4. Custom Multi-Layer (3 layers)
            customMultiLayer = new LayerConfiguration(3);

            // Layer 1: Background gradient
            customMultiLayer.layers[0] = Layer.CreateGradient(
                new Color(0.2f, 0.3f, 0.5f),
                new Color(0.1f, 0.15f, 0.25f),
                GradientDirection.Vertical
            );

            // Layer 2: Viền ngoài
            customMultiLayer.layers[1] = Layer.CreateBorder(
                new Color(0.8f, 0.8f, 0.8f),
                1f,
                10f,
                new Padding(2)
            );

            // Layer 3: Highlight góc trên
            customMultiLayer.layers[2] = Layer.CreateSolidColor(
                new Color(1f, 1f, 1f, 0.1f),
                new Padding(5, 5, 5, 50) // Chỉ ở góc trên
            );
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.LabelField("LAYER DRAWING SYSTEM DEMO", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            // Demo 1: Simple Background
            DrawDemo("1. Simple Background (Màu đặc)", simpleBackground, 60);

            // Demo 2: Bordered Background
            DrawDemo("2. Background With Border (Có viền + bo góc)", borderedBackground, 60);

            // Demo 3: Card Style
            DrawDemo("3. Card Style (Có shadow)", cardStyle, 80);

            // Demo 4: Custom Multi-Layer
            DrawDemo("4. Custom Multi-Layer (Gradient + Border + Highlight)", customMultiLayer, 100);

            // Demo 5: Interactive Example
            DrawInteractiveDemo();

            EditorGUILayout.Space(20);
            DrawCodeExamples();

            EditorGUILayout.EndScrollView();
        }

        private void DrawDemo(string title, LayerConfiguration config, float height)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            
            demoRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(height), GUILayout.ExpandWidth(true));
            demoRect.x += 10;
            demoRect.width -= 20;

            // Vẽ layer system
            LayerDrawingSystem.DrawLayers(demoRect, config);

            // Vẽ text lên trên để thấy rõ
            GUI.Label(demoRect, "  Layer Drawing Demo", EditorStyles.whiteLargeLabel);

            EditorGUILayout.Space(10);
        }

        private Color interactiveColor = new Color(0.3f, 0.5f, 0.8f);
        private float interactiveBorderRadius = 5f;
        private float interactiveBorderWidth = 2f;

        private void DrawInteractiveDemo()
        {
            EditorGUILayout.LabelField("5. Interactive Demo (Tùy chỉnh trực tiếp)", EditorStyles.boldLabel);
            
            interactiveColor = EditorGUILayout.ColorField("Background Color", interactiveColor);
            interactiveBorderRadius = EditorGUILayout.Slider("Border Radius", interactiveBorderRadius, 0f, 20f);
            interactiveBorderWidth = EditorGUILayout.Slider("Border Width", interactiveBorderWidth, 0f, 10f);

            // Tạo config động
            LayerConfiguration dynamicConfig = new LayerConfiguration(2);
            dynamicConfig.layers[0] = Layer.CreateRoundedRect(interactiveColor, interactiveBorderRadius);
            dynamicConfig.layers[1] = Layer.CreateBorder(Color.white, interactiveBorderWidth, interactiveBorderRadius);

            demoRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(80), GUILayout.ExpandWidth(true));
            demoRect.x += 10;
            demoRect.width -= 20;

            LayerDrawingSystem.DrawLayers(demoRect, dynamicConfig);
            GUI.Label(demoRect, "  Điều chỉnh để thấy thay đổi!", EditorStyles.whiteLargeLabel);

            EditorGUILayout.Space(10);
        }

        private void DrawCodeExamples()
        {
            EditorGUILayout.LabelField("CODE EXAMPLES", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "// Cách sử dụng cơ bản:\n\n" +
                "// 1. Tạo config đơn giản\n" +
                "LayerConfiguration config = LayerConfiguration.CreateSimpleBackground(Color.gray);\n\n" +
                "// 2. Vẽ trong OnGUI\n" +
                "Rect rect = GUILayoutUtility.GetRect(100, 50);\n" +
                "LayerDrawingSystem.DrawLayers(rect, config);\n\n" +
                "// 3. Tạo custom multi-layer\n" +
                "LayerConfiguration custom = new LayerConfiguration(2);\n" +
                "custom.layers[0] = Layer.CreateRoundedRect(Color.blue, 5f);\n" +
                "custom.layers[1] = Layer.CreateBorder(Color.white, 2f, 5f);",
                MessageType.Info
            );
        }
    }
}

