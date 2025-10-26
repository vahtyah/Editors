using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using CustomLayerDrawing;

namespace CustomLayerDrawing.Examples
{
    /// <summary>
    /// Demo: Tạo một Custom List hoàn chỉnh sử dụng Layer Drawing System
    /// Mở bằng: Window > Custom List Demo
    /// </summary>
    public class CustomListDemo : EditorWindow
    {
        // Data
        [System.Serializable]
        public class Item
        {
            public string name = "New Item";
            public int level = 1;
            public float health = 100f;
            public Color color = Color.white;
            public bool isActive = true;
        }

        private List<Item> items = new List<Item>();
        private int selectedIndex = -1;
        private Vector2 scrollPosition;

        // Layer Configurations
        private LayerConfiguration globalBackground;
        private LayerConfiguration headerBackground;
        private LayerConfiguration listBackground;
        private LayerConfiguration elementNormalBackground;
        private LayerConfiguration elementSelectedBackground;
        private LayerConfiguration footerBackground;
        private LayerConfiguration buttonBackground;

        // UI Settings
        private const float HEADER_HEIGHT = 50f;
        private const float ELEMENT_HEIGHT = 30f;
        private const float ELEMENT_EXPANDED_BASE = 150f;
        private const float FOOTER_HEIGHT = 40f;
        private const float PADDING = 5f;

        [MenuItem("Window/Custom List Demo")]
        public static void ShowWindow()
        {
            var window = GetWindow<CustomListDemo>("Custom List Demo");
            window.minSize = new Vector2(450, 600);
        }

        private void OnEnable()
        {
            InitializeData();
            InitializeLayerConfigs();
        }

        private void InitializeData()
        {
            if (items.Count == 0)
            {
                // Tạo dữ liệu mẫu
                items.Add(new Item { name = "Warrior", level = 10, health = 150f, color = new Color(0.8f, 0.2f, 0.2f), isActive = true });
                items.Add(new Item { name = "Mage", level = 8, health = 80f, color = new Color(0.2f, 0.4f, 0.9f), isActive = true });
                items.Add(new Item { name = "Archer", level = 7, health = 100f, color = new Color(0.2f, 0.8f, 0.3f), isActive = false });
                items.Add(new Item { name = "Healer", level = 5, health = 90f, color = new Color(0.9f, 0.9f, 0.2f), isActive = true });
                items.Add(new Item { name = "Tank", level = 12, health = 200f, color = new Color(0.5f, 0.5f, 0.5f), isActive = true });
            }
        }

        private void InitializeLayerConfigs()
        {
            // Global Background - Gradient tổng thể
            globalBackground = new LayerConfiguration(2);
            globalBackground.layers[0] = Layer.CreateGradient(
                new Color(0.15f, 0.15f, 0.2f),
                new Color(0.1f, 0.1f, 0.15f),
                GradientDirection.Vertical
            );
            globalBackground.layers[1] = Layer.CreateBorder(
                new Color(0.3f, 0.3f, 0.35f),
                1f,
                8f
            );

            // Header Background - Gradient với viền
            headerBackground = new LayerConfiguration(2);
            headerBackground.layers[0] = Layer.CreateGradient(
                new Color(0.25f, 0.35f, 0.55f),
                new Color(0.2f, 0.3f, 0.5f),
                GradientDirection.Vertical
            );
            headerBackground.layers[1] = Layer.CreateBorder(
                new Color(0.4f, 0.5f, 0.7f),
                2f,
                0f
            );
            headerBackground.layers[1].borderWidth = new Vector4(0, 0, 0, 2);

            // List Background
            listBackground = LayerConfiguration.CreateSimpleBackground(new Color(0.18f, 0.18f, 0.22f));

            // Element Normal Background
            elementNormalBackground = new LayerConfiguration(1);
            elementNormalBackground.layers[0] = Layer.CreateSolidColor(
                new Color(0.2f, 0.2f, 0.25f, 0.5f),
                new Padding(2f)
            );

            // Element Selected Background
            elementSelectedBackground = new LayerConfiguration(2);
            elementSelectedBackground.layers[0] = Layer.CreateRoundedRect(
                new Color(0.3f, 0.5f, 0.7f),
                4f,
                new Padding(2f)
            );
            elementSelectedBackground.layers[1] = Layer.CreateBorder(
                new Color(0.5f, 0.7f, 0.9f),
                1f,
                4f,
                new Padding(2f)
            );

            // Footer Background
            footerBackground = new LayerConfiguration(2);
            footerBackground.layers[0] = Layer.CreateRoundedRect(
                new Color(0.22f, 0.22f, 0.27f),
                6f
            );
            footerBackground.layers[1] = Layer.CreateBorder(
                new Color(0.4f, 0.4f, 0.45f),
                1f,
                6f
            );

            // Button Background
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
            if (items == null) InitializeData();
            if (globalBackground == null) InitializeLayerConfigs();

            DrawCustomList();
        }

        private void DrawCustomList()
        {
            // Global Background
            Rect globalRect = new Rect(10, 10, position.width - 20, position.height - 20);
            LayerDrawingSystem.DrawLayers(globalRect, globalBackground);

            GUILayout.BeginArea(new Rect(globalRect.x + PADDING, globalRect.y + PADDING, 
                                         globalRect.width - PADDING * 2, globalRect.height - PADDING * 2));

            // Header
            DrawHeader();
            GUILayout.Space(PADDING);

            // List
            DrawList();
            GUILayout.Space(PADDING);

            // Footer
            DrawFooter();

            GUILayout.EndArea();
        }

        private void DrawHeader()
        {
            Rect headerRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, 
                                                       GUILayout.Height(HEADER_HEIGHT), 
                                                       GUILayout.ExpandWidth(true));
            
            LayerDrawingSystem.DrawLayers(headerRect, headerBackground);

            // Header Title
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 20;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.normal.textColor = Color.white;

            GUI.Label(headerRect, "CHARACTER LIST", titleStyle);

            // Header Info
            GUIStyle infoStyle = new GUIStyle(GUI.skin.label);
            infoStyle.fontSize = 11;
            infoStyle.alignment = TextAnchor.UpperRight;
            infoStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
            infoStyle.padding = new RectOffset(0, 10, 5, 0);

            GUI.Label(headerRect, $"Total: {items.Count} items", infoStyle);
        }

        private void DrawList()
        {
            float availableHeight = position.height - HEADER_HEIGHT - FOOTER_HEIGHT - PADDING * 6 - 20;
            
            Rect listRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none,
                                                     GUILayout.Height(availableHeight),
                                                     GUILayout.ExpandWidth(true));

            LayerDrawingSystem.DrawLayers(listRect, listBackground);

            // Scroll view for list items
            Rect scrollViewRect = new Rect(listRect.x + PADDING, listRect.y + PADDING,
                                           listRect.width - PADDING * 2, listRect.height - PADDING * 2);

            GUILayout.BeginArea(scrollViewRect);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            if (items.Count == 0)
            {
                DrawEmptyList();
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    DrawElement(items[i], i);
                    GUILayout.Space(3);
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawEmptyList()
        {
            GUIStyle emptyStyle = new GUIStyle(GUI.skin.label);
            emptyStyle.alignment = TextAnchor.MiddleCenter;
            emptyStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
            emptyStyle.fontSize = 14;
            emptyStyle.fontStyle = FontStyle.Italic;

            GUILayout.FlexibleSpace();
            GUILayout.Label("List is empty. Click 'Add Item' to create new items.", emptyStyle);
            GUILayout.FlexibleSpace();
        }

        private void DrawElement(Item item, int index)
        {
            bool isSelected = (index == selectedIndex);
            float elementHeight = isSelected ? ELEMENT_EXPANDED_BASE : ELEMENT_HEIGHT;

            Rect elementRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none,
                                                        GUILayout.Height(elementHeight),
                                                        GUILayout.ExpandWidth(true));

            // Draw background
            LayerConfiguration bgConfig = isSelected ? elementSelectedBackground : elementNormalBackground;
            LayerDrawingSystem.DrawLayers(elementRect, bgConfig);

            // Draw content
            GUILayout.BeginArea(new Rect(elementRect.x + 5, elementRect.y + 2, elementRect.width - 10, elementRect.height - 4));

            // Header
            GUILayout.BeginHorizontal();

            // Drag Handle Icon
            GUIStyle dragStyle = new GUIStyle(GUI.skin.label);
            dragStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            dragStyle.fontSize = 14;
            GUILayout.Label("☰", dragStyle, GUILayout.Width(20));

            // Name
            GUIStyle nameStyle = new GUIStyle(GUI.skin.label);
            nameStyle.fontStyle = FontStyle.Bold;
            nameStyle.normal.textColor = isSelected ? Color.white : new Color(0.9f, 0.9f, 0.9f);
            nameStyle.fontSize = isSelected ? 14 : 12;
            GUILayout.Label(item.name, nameStyle);

            GUILayout.FlexibleSpace();

            // Level badge
            DrawBadge($"Lv.{item.level}", new Color(0.9f, 0.7f, 0.2f));

            // Active status
            if (item.isActive)
            {
                DrawBadge("Active", new Color(0.2f, 0.8f, 0.3f));
            }

            // Remove button
            if (GUILayout.Button("✕", GUILayout.Width(25), GUILayout.Height(20)))
            {
                items.RemoveAt(index);
                if (selectedIndex == index) selectedIndex = -1;
                else if (selectedIndex > index) selectedIndex--;
                return;
            }

            GUILayout.EndHorizontal();

            // Expanded content
            if (isSelected)
            {
                GUILayout.Space(5);
                DrawElementExpanded(item);
            }

            GUILayout.EndArea();

            // Click detection
            if (Event.current.type == EventType.MouseDown && elementRect.Contains(Event.current.mousePosition))
            {
                selectedIndex = (selectedIndex == index) ? -1 : index;
                Event.current.Use();
                Repaint();
            }
        }

        private void DrawBadge(string text, Color color)
        {
            Rect badgeRect = GUILayoutUtility.GetRect(new GUIContent(text), GUIStyle.none,
                                                      GUILayout.Height(18), GUILayout.Width(60));

            LayerConfiguration badgeBg = new LayerConfiguration(1);
            badgeBg.layers[0] = Layer.CreateRoundedRect(color, 3f);
            LayerDrawingSystem.DrawLayers(badgeRect, badgeBg);

            GUIStyle badgeStyle = new GUIStyle(GUI.skin.label);
            badgeStyle.alignment = TextAnchor.MiddleCenter;
            badgeStyle.normal.textColor = Color.white;
            badgeStyle.fontSize = 10;
            badgeStyle.fontStyle = FontStyle.Bold;

            GUI.Label(badgeRect, text, badgeStyle);
        }

        private void DrawElementExpanded(Item item)
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
            labelStyle.fontSize = 11;

            // Name field
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", labelStyle, GUILayout.Width(80));
            item.name = EditorGUILayout.TextField(item.name);
            GUILayout.EndHorizontal();

            GUILayout.Space(3);

            // Level slider
            GUILayout.BeginHorizontal();
            GUILayout.Label("Level:", labelStyle, GUILayout.Width(80));
            item.level = EditorGUILayout.IntSlider(item.level, 1, 100);
            GUILayout.EndHorizontal();

            GUILayout.Space(3);

            // Health bar
            GUILayout.BeginHorizontal();
            GUILayout.Label("Health:", labelStyle, GUILayout.Width(80));
            
            Rect healthBarRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none,
                                                          GUILayout.Height(20), GUILayout.ExpandWidth(true));
            
            // Health bar background
            LayerConfiguration healthBg = LayerConfiguration.CreateBackgroundWithBorder(
                new Color(0.15f, 0.15f, 0.15f),
                new Color(0.4f, 0.4f, 0.4f),
                1f,
                3f
            );
            LayerDrawingSystem.DrawLayers(healthBarRect, healthBg);

            // Health bar fill
            float healthPercent = item.health / 100f;
            if (healthPercent > 0.01f)
            {
                Rect healthFillRect = new Rect(healthBarRect.x + 2, healthBarRect.y + 2,
                                               (healthBarRect.width - 4) * healthPercent, healthBarRect.height - 4);
                
                LayerConfiguration healthFill = new LayerConfiguration(1);
                Color healthColor = Color.Lerp(new Color(0.8f, 0.2f, 0.2f), new Color(0.2f, 0.8f, 0.3f), healthPercent);
                healthFill.layers[0] = Layer.CreateGradient(healthColor, healthColor * 0.8f, GradientDirection.Horizontal);
                LayerDrawingSystem.DrawLayers(healthFillRect, healthFill);
            }

            // Health text
            GUIStyle healthTextStyle = new GUIStyle(GUI.skin.label);
            healthTextStyle.alignment = TextAnchor.MiddleCenter;
            healthTextStyle.normal.textColor = Color.white;
            healthTextStyle.fontStyle = FontStyle.Bold;
            healthTextStyle.fontSize = 11;
            GUI.Label(healthBarRect, $"{item.health:F0} / 100", healthTextStyle);

            GUILayout.EndHorizontal();

            // Health slider
            item.health = GUILayout.HorizontalSlider(item.health, 0f, 200f);

            GUILayout.Space(3);

            // Color picker
            GUILayout.BeginHorizontal();
            GUILayout.Label("Color:", labelStyle, GUILayout.Width(80));
            item.color = EditorGUILayout.ColorField(item.color);
            GUILayout.EndHorizontal();

            GUILayout.Space(3);

            // Active toggle
            GUILayout.BeginHorizontal();
            GUILayout.Label("Active:", labelStyle, GUILayout.Width(80));
            item.isActive = EditorGUILayout.Toggle(item.isActive);
            GUILayout.EndHorizontal();
        }

        private void DrawFooter()
        {
            Rect footerRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none,
                                                       GUILayout.Height(FOOTER_HEIGHT),
                                                       GUILayout.ExpandWidth(true));

            LayerDrawingSystem.DrawLayers(footerRect, footerBackground);

            // Footer buttons
            GUILayout.BeginArea(new Rect(footerRect.x + PADDING, footerRect.y + PADDING,
                                        footerRect.width - PADDING * 2, footerRect.height - PADDING * 2));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Add button
            if (DrawCustomButton("+ Add Item", new Color(0.2f, 0.7f, 0.3f), 100))
            {
                items.Add(new Item());
                selectedIndex = items.Count - 1;
            }

            GUILayout.Space(5);

            // Remove selected button
            GUI.enabled = (selectedIndex >= 0 && selectedIndex < items.Count);
            if (DrawCustomButton("- Remove Selected", new Color(0.8f, 0.3f, 0.2f), 130))
            {
                items.RemoveAt(selectedIndex);
                selectedIndex = -1;
            }
            GUI.enabled = true;

            GUILayout.Space(5);

            // Clear all button
            GUI.enabled = items.Count > 0;
            if (DrawCustomButton("Clear All", new Color(0.6f, 0.4f, 0.2f), 80))
            {
                if (EditorUtility.DisplayDialog("Clear All", "Are you sure you want to clear all items?", "Yes", "No"))
                {
                    items.Clear();
                    selectedIndex = -1;
                }
            }
            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private bool DrawCustomButton(string text, Color color, float width)
        {
            Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent(text), GUIStyle.none,
                                                       GUILayout.Height(26), GUILayout.Width(width));

            // Hover effect
            bool isHover = buttonRect.Contains(Event.current.mousePosition);
            Color finalColor = isHover ? color * 1.2f : color;

            LayerConfiguration customBtnBg = new LayerConfiguration(2);
            customBtnBg.layers[0] = Layer.CreateRoundedRect(finalColor, 4f);
            customBtnBg.layers[1] = Layer.CreateBorder(finalColor * 1.3f, 1f, 4f);

            LayerDrawingSystem.DrawLayers(buttonRect, customBtnBg);

            // Text
            GUIStyle btnStyle = new GUIStyle(GUI.skin.label);
            btnStyle.alignment = TextAnchor.MiddleCenter;
            btnStyle.normal.textColor = Color.white;
            btnStyle.fontStyle = FontStyle.Bold;
            btnStyle.fontSize = 12;

            GUI.Label(buttonRect, text, btnStyle);

            // Click detection
            bool clicked = Event.current.type == EventType.MouseDown && buttonRect.Contains(Event.current.mousePosition);
            if (clicked)
            {
                Event.current.Use();
                Repaint();
            }

            return clicked;
        }
    }
}

