using UnityEngine;
using UnityEditor;
using CustomLayerDrawing;

namespace CustomLayerDrawing.Examples
{
    /// <summary>
    /// Ví dụ thực tế: Custom Editor Window sử dụng Layer Drawing
    /// Mở bằng: Window > Real World Example
    /// </summary>
    public class RealWorldExample : EditorWindow
    {
        // Data
        private string playerName = "Hero";
        private int playerLevel = 1;
        private float playerHealth = 100f;
        private float playerMana = 50f;

        // Layer Configs
        private LayerConfiguration windowBackground;
        private LayerConfiguration headerBackground;
        private LayerConfiguration sectionBackground;
        private LayerConfiguration statBarBackground;
        private LayerConfiguration statBarFill;
        private LayerConfiguration buttonBackground;

        // UI State
        private Vector2 scrollPosition;

        [MenuItem("Window/Real World Example")]
        public static void ShowWindow()
        {
            var window = GetWindow<RealWorldExample>("Player Stats");
            window.minSize = new Vector2(400, 500);
        }

        private void OnEnable()
        {
            InitializeLayerConfigs();
        }

        private void InitializeLayerConfigs()
        {
            // Window Background - Gradient tổng thể
            windowBackground = new LayerConfiguration(1);
            windowBackground.layers[0] = Layer.CreateGradient(
                new Color(0.15f, 0.15f, 0.2f),
                new Color(0.1f, 0.1f, 0.15f),
                GradientDirection.Vertical
            );

            // Header - Gradient với viền dưới
            headerBackground = new LayerConfiguration(2);
            headerBackground.layers[0] = Layer.CreateGradient(
                new Color(0.3f, 0.4f, 0.6f),
                new Color(0.2f, 0.3f, 0.5f),
                GradientDirection.Vertical
            );
            headerBackground.layers[1] = Layer.CreateBorder(
                new Color(0.5f, 0.6f, 0.8f),
                2f,   // Border width
                0f    // Border radius
            );
            // Tùy chỉnh để chỉ vẽ viền dưới (left, top, right, bottom)
            headerBackground.layers[1].borderWidth = new Vector4(0, 0, 0, 2);

            // Section Background - Card style
            sectionBackground = LayerConfiguration.CreateCardStyle(
                new Color(0.2f, 0.2f, 0.25f),
                new Color(0f, 0f, 0f, 0.5f),
                6f
            );

            // Stat Bar Background - Rounded rect tối
            statBarBackground = LayerConfiguration.CreateBackgroundWithBorder(
                new Color(0.1f, 0.1f, 0.1f),
                new Color(0.3f, 0.3f, 0.3f),
                1f,
                3f
            );

            // Stat Bar Fill - Gradient fill
            statBarFill = new LayerConfiguration(1);
            statBarFill.layers[0] = Layer.CreateGradient(
                new Color(0.2f, 0.8f, 0.3f),
                new Color(0.15f, 0.6f, 0.2f),
                GradientDirection.Horizontal
            );

            // Button Background - Hover effect
            buttonBackground = new LayerConfiguration(2);
            buttonBackground.layers[0] = Layer.CreateRoundedRect(
                new Color(0.3f, 0.5f, 0.7f),
                4f
            );
            buttonBackground.layers[1] = Layer.CreateBorder(
                new Color(0.5f, 0.7f, 0.9f),
                1f,
                4f
            );
        }

        private void OnGUI()
        {
            // Background tổng thể
            Rect bgRect = new Rect(0, 0, position.width, position.height);
            LayerDrawingSystem.DrawLayers(bgRect, windowBackground);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            DrawHeader();
            EditorGUILayout.Space(10);
            
            DrawPlayerInfo();
            EditorGUILayout.Space(10);
            
            DrawStats();
            EditorGUILayout.Space(10);
            
            DrawActions();
            
            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            Rect headerRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(60), GUILayout.ExpandWidth(true));
            LayerDrawingSystem.DrawLayers(headerRect, headerBackground);

            // Title
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 24;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.normal.textColor = Color.white;

            GUI.Label(headerRect, "PLAYER STATISTICS", titleStyle);
        }

        private void DrawPlayerInfo()
        {
            Rect sectionRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(120), GUILayout.ExpandWidth(true));
            sectionRect.x += 10;
            sectionRect.width -= 20;
            
            LayerDrawingSystem.DrawLayers(sectionRect, sectionBackground);

            // Content padding
            Rect contentRect = new Rect(
                sectionRect.x + 15,
                sectionRect.y + 15,
                sectionRect.width - 30,
                sectionRect.height - 30
            );

            GUILayout.BeginArea(contentRect);
            
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = Color.white;
            labelStyle.fontSize = 12;

            GUIStyle valueStyle = new GUIStyle(GUI.skin.label);
            valueStyle.normal.textColor = new Color(0.8f, 0.9f, 1f);
            valueStyle.fontSize = 16;
            valueStyle.fontStyle = FontStyle.Bold;

            GUILayout.Label("Character Name", labelStyle);
            playerName = EditorGUILayout.TextField(playerName);
            
            GUILayout.Space(5);
            
            GUILayout.Label("Level", labelStyle);
            playerLevel = EditorGUILayout.IntSlider(playerLevel, 1, 100);
            
            GUILayout.EndArea();
        }

        private void DrawStats()
        {
            Rect sectionRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(150), GUILayout.ExpandWidth(true));
            sectionRect.x += 10;
            sectionRect.width -= 20;
            
            LayerDrawingSystem.DrawLayers(sectionRect, sectionBackground);

            Rect contentRect = new Rect(
                sectionRect.x + 15,
                sectionRect.y + 15,
                sectionRect.width - 30,
                sectionRect.height - 30
            );

            GUILayout.BeginArea(contentRect);
            
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = Color.white;
            labelStyle.fontSize = 12;

            // Health Bar
            GUILayout.Label("Health", labelStyle);
            DrawStatBar(ref playerHealth, 100f, new Color(0.8f, 0.2f, 0.2f), new Color(0.6f, 0.15f, 0.15f));
            
            GUILayout.Space(10);
            
            // Mana Bar
            GUILayout.Label("Mana", labelStyle);
            DrawStatBar(ref playerMana, 100f, new Color(0.2f, 0.4f, 0.8f), new Color(0.15f, 0.3f, 0.6f));
            
            GUILayout.EndArea();
        }

        private void DrawStatBar(ref float currentValue, float maxValue, Color startColor, Color endColor)
        {
            Rect barRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(25));
            
            // Background
            LayerDrawingSystem.DrawLayers(barRect, statBarBackground);
            
            // Fill
            float fillPercentage = currentValue / maxValue;
            Rect fillRect = new Rect(
                barRect.x + 2,
                barRect.y + 2,
                (barRect.width - 4) * fillPercentage,
                barRect.height - 4
            );

            if (fillPercentage > 0.01f)
            {
                LayerConfiguration fillConfig = new LayerConfiguration(1);
                fillConfig.layers[0] = Layer.CreateGradient(startColor, endColor, GradientDirection.Horizontal);
                fillConfig.layers[0].borderRadius = Vector4.one * 2f;
                
                LayerDrawingSystem.DrawLayers(fillRect, fillConfig);
            }

            // Text
            GUIStyle textStyle = new GUIStyle(GUI.skin.label);
            textStyle.alignment = TextAnchor.MiddleCenter;
            textStyle.normal.textColor = Color.white;
            textStyle.fontStyle = FontStyle.Bold;
            
            GUI.Label(barRect, $"{currentValue:F0} / {maxValue:F0}", textStyle);
            
            // Slider
            currentValue = GUI.HorizontalSlider(
                new Rect(barRect.x, barRect.yMax + 2, barRect.width, 15),
                currentValue,
                0f,
                maxValue
            );
        }

        private void DrawActions()
        {
            Rect sectionRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(100), GUILayout.ExpandWidth(true));
            sectionRect.x += 10;
            sectionRect.width -= 20;
            
            LayerDrawingSystem.DrawLayers(sectionRect, sectionBackground);

            Rect contentRect = new Rect(
                sectionRect.x + 15,
                sectionRect.y + 15,
                sectionRect.width - 30,
                sectionRect.height - 30
            );

            GUILayout.BeginArea(contentRect);
            
            GUILayout.BeginHorizontal();
            
            if (DrawCustomButton("Heal", new Color(0.2f, 0.8f, 0.3f)))
            {
                playerHealth = Mathf.Min(playerHealth + 20f, 100f);
            }
            
            if (DrawCustomButton("Rest", new Color(0.3f, 0.5f, 0.9f)))
            {
                playerMana = Mathf.Min(playerMana + 30f, 100f);
            }
            
            if (DrawCustomButton("Level Up", new Color(0.9f, 0.7f, 0.2f)))
            {
                playerLevel++;
                playerHealth = 100f;
                playerMana = 100f;
            }
            
            GUILayout.EndHorizontal();
            
            GUILayout.EndArea();
        }

        private bool DrawCustomButton(string text, Color color)
        {
            Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent(text), GUIStyle.none, GUILayout.Height(40), GUILayout.ExpandWidth(true));
            
            // Background với màu tùy chỉnh
            LayerConfiguration customButtonBg = new LayerConfiguration(2);
            customButtonBg.layers[0] = Layer.CreateRoundedRect(color, 4f);
            customButtonBg.layers[1] = Layer.CreateBorder(
                new Color(color.r + 0.2f, color.g + 0.2f, color.b + 0.2f),
                1f,
                4f
            );
            
            LayerDrawingSystem.DrawLayers(buttonRect, customButtonBg);
            
            // Text
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.label);
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.fontSize = 14;
            
            GUI.Label(buttonRect, text, buttonStyle);
            
            // Click detection
            return Event.current.type == EventType.MouseDown && buttonRect.Contains(Event.current.mousePosition);
        }
    }
}

