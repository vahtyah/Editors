#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using CustomLayerDrawing;
using System.Collections. Generic;
using System. Linq;
using System.Reflection;

namespace CustomInspector. Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class CustomBoxGroupDrawer :  UnityEditor.Editor
    {
        private const float HEADER_HEIGHT = 28f;
        private const float CONTENT_PADDING_TOP = 8f;
        private const float CONTENT_PADDING_BOTTOM = 8f;
        private const float CONTENT_PADDING_HORIZONTAL = 12f;
        private const float HEADER_PADDING_LEFT = 12f;
        private const float GROUP_SPACING = 6f;
        private const float ICON_SIZE = 16f;
        private const float ICON_SPACING = 6f;

        private Dictionary<string, GroupData> groups = new Dictionary<string, GroupData>();
        private List<SerializedProperty> propertiesWithoutGroup = new List<SerializedProperty>();
        private bool isInitialized = false;

        private class GroupData
        {
            public CustomBoxGroupAttribute attribute;
            public List<SerializedProperty> properties = new List<SerializedProperty>();
            public LayerConfiguration backgroundLayerConfig;
            public LayerConfiguration headerLayerConfig;
            public GUIStyle labelStyle;
        }

        private void OnEnable()
        {
            isInitialized = false;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!isInitialized)
            {
                InitializeGroups();
                isInitialized = true;
            }

            DrawScriptField();

            foreach (var property in propertiesWithoutGroup)
            {
                EditorGUILayout.PropertyField(property, true);
            }

            foreach (var groupData in groups.Values)
            {
                DrawGroup(groupData);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void InitializeGroups()
        {
            groups.Clear();
            propertiesWithoutGroup.Clear();

            FieldInfo[] fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                if (field.IsPrivate && field.GetCustomAttribute<SerializeField>() == null)
                    continue;

                CustomBoxGroupAttribute groupAttribute = field.GetCustomAttribute<CustomBoxGroupAttribute>();
                SerializedProperty property = serializedObject.FindProperty(field.Name);
                
                if (property == null)
                    continue;

                if (groupAttribute != null)
                {
                    if (! groups.ContainsKey(groupAttribute.GroupID))
                    {
                        groups[groupAttribute.GroupID] = new GroupData
                        {
                            attribute = groupAttribute,
                            backgroundLayerConfig = CreateBackgroundLayerConfiguration(groupAttribute.Style),
                            headerLayerConfig = CreateHeaderLayerConfiguration(groupAttribute.Style),
                            labelStyle = CreateLabelStyle(groupAttribute.Style)
                        };
                    }

                    groups[groupAttribute.GroupID].properties.Add(property);
                }
                else
                {
                    propertiesWithoutGroup. Add(property);
                }
            }
        }

        private void DrawScriptField()
        {
            GUI.enabled = false;
            SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");
            if (scriptProperty != null)
            {
                EditorGUILayout.PropertyField(scriptProperty);
            }
            GUI.enabled = true;
        }

        private void DrawGroup(GroupData groupData)
        {
            if (groupData.properties.Count == 0)
                return;

            float totalHeight = CalculateTotalGroupHeight(groupData);
            Rect groupRect = GUILayoutUtility.GetRect(0, totalHeight, GUILayout. ExpandWidth(true));

            if (Event.current.type == EventType.Repaint)
            {
                LayerDrawingSystem.DrawLayers(groupRect, groupData.backgroundLayerConfig);

                Rect headerLayerRect = new Rect(
                    groupRect.x,
                    groupRect.y,
                    groupRect.width,
                    HEADER_HEIGHT
                );
                LayerDrawingSystem. DrawLayers(headerLayerRect, groupData.headerLayerConfig);
            }

            Rect headerContentRect = new Rect(
                groupRect.x + HEADER_PADDING_LEFT,
                groupRect.y,
                groupRect.width - HEADER_PADDING_LEFT * 2,
                HEADER_HEIGHT
            );
            DrawHeaderContent(headerContentRect, groupData);

            float currentY = groupRect. y + HEADER_HEIGHT + CONTENT_PADDING_TOP;
            float contentX = groupRect. x + CONTENT_PADDING_HORIZONTAL;
            float contentWidth = groupRect.width - CONTENT_PADDING_HORIZONTAL * 2;

            EditorGUI.indentLevel++;
            
            foreach (var property in groupData.properties)
            {
                float propertyHeight = EditorGUI.GetPropertyHeight(property, true);
                Rect propertyRect = new Rect(contentX, currentY, contentWidth, propertyHeight);
                
                EditorGUI.PropertyField(propertyRect, property, true);
                
                currentY += propertyHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            
            EditorGUI.indentLevel--;

            GUILayout.Space(GROUP_SPACING);
        }

        private float CalculateTotalGroupHeight(GroupData groupData)
        {
            float totalHeight = HEADER_HEIGHT + CONTENT_PADDING_TOP + CONTENT_PADDING_BOTTOM;

            foreach (var property in groupData.properties)
            {
                totalHeight += EditorGUI. GetPropertyHeight(property, true);
                totalHeight += EditorGUIUtility.standardVerticalSpacing;
            }

            return totalHeight;
        }

        private void DrawHeaderContent(Rect rect, GroupData groupData)
        {
            Rect labelRect = rect;

            if (groupData.attribute. ShowIcon && ! string.IsNullOrEmpty(groupData.attribute.IconName))
            {
                Texture2D icon = GetIcon(groupData.attribute.IconName);
                if (icon != null)
                {
                    Rect iconRect = new Rect(
                        rect.x,
                        rect.y + (rect.height - ICON_SIZE) * 0.5f,
                        ICON_SIZE,
                        ICON_SIZE
                    );

                    GUI.DrawTexture(iconRect, icon);
                    labelRect. x += ICON_SIZE + ICON_SPACING;
                    labelRect.width -= ICON_SIZE + ICON_SPACING;
                }
            }

            GUI.Label(labelRect, groupData.attribute.Label, groupData.labelStyle);
        }

        // ...  rest of the configuration methods remain the same ...
        
        private LayerConfiguration CreateBackgroundLayerConfiguration(BoxStyle style)
        {
            switch (style)
            {
                case BoxStyle.Dark:  return CreateDarkBackgroundStyle();
                case BoxStyle.Light: return CreateLightBackgroundStyle();
                case BoxStyle.Accent: return CreateAccentBackgroundStyle();
                default: return CreateDarkBackgroundStyle();
            }
        }

        private LayerConfiguration CreateDarkBackgroundStyle()
        {
            LayerConfiguration config = new LayerConfiguration(3);
            config.layers[0] = Layer. CreateRoundedRect(new Color(0.15f, 0.15f, 0.15f, 1f), 4f, new Padding(0, 1, 1, 0));
            config.layers[1] = Layer. CreateRoundedRect(new Color(0.22f, 0.22f, 0.22f, 1f), 4f);
            config.layers[2] = Layer.CreateBorder(new Color(0.35f, 0.35f, 0.35f, 1f), 1f, 4f);
            return config;
        }

        private LayerConfiguration CreateLightBackgroundStyle()
        {
            LayerConfiguration config = new LayerConfiguration(2);
            config.layers[0] = Layer.CreateRoundedRect(new Color(0.95f, 0.95f, 0.95f, 1f), 4f);
            config.layers[1] = Layer.CreateBorder(new Color(0.7f, 0.7f, 0.7f, 1f), 1f, 4f);
            return config;
        }

        private LayerConfiguration CreateAccentBackgroundStyle()
        {
            LayerConfiguration config = new LayerConfiguration(2);
            config.layers[0] = Layer.CreateGradient(new Color(0.2f, 0.4f, 0.65f, 1f), new Color(0.15f, 0.3f, 0.5f, 1f), GradientDirection.Vertical);
            config.layers[1] = Layer.CreateBorder(new Color(0.3f, 0.6f, 0.9f, 1f), 1f, 4f);
            return config;
        }

        private LayerConfiguration CreateHeaderLayerConfiguration(BoxStyle style)
        {
            switch (style)
            {
                case BoxStyle.Dark: return CreateDarkHeaderStyle();
                case BoxStyle.Light: return CreateLightHeaderStyle();
                case BoxStyle.Accent: return CreateAccentHeaderStyle();
                default:  return CreateDarkHeaderStyle();
            }
        }

        private LayerConfiguration CreateDarkHeaderStyle()
        {
            LayerConfiguration config = new LayerConfiguration(3);
            config.layers[0] = Layer. CreateRoundedRect(new Color(0f, 0f, 0f, 0.15f), 4f, new Padding(0, 1, 1, 0));
            config.layers[1] = Layer. CreateRoundedRect(new Color(0f, 0f, 0f, 0.15f), 4f);
            config.layers[2] = Layer.CreateBorder(new Color(0.35f, 0.35f, 0.35f, 1f), 1f, 4f);
            return config;
        }

        private LayerConfiguration CreateLightHeaderStyle()
        {
            LayerConfiguration config = new LayerConfiguration(1);
            config.layers[0] = Layer.CreateSolidColor(new Color(0f, 0f, 0f, 0.05f));
            return config;
        }

        private LayerConfiguration CreateAccentHeaderStyle()
        {
            LayerConfiguration config = new LayerConfiguration(1);
            config.layers[0] = Layer.CreateSolidColor(new Color(1f, 1f, 1f, 0.1f));
            return config;
        }

        private GUIStyle CreateLabelStyle(BoxStyle style)
        {
            GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
            labelStyle.fontSize = 12;
            labelStyle.alignment = TextAnchor.MiddleLeft;
            labelStyle.normal. textColor = GetLabelColor(style);
            labelStyle.padding = new RectOffset(0, 0, 0, 0);
            return labelStyle;
        }

        private Color GetLabelColor(BoxStyle style)
        {
            switch (style)
            {
                case BoxStyle.Dark: return new Color(0.85f, 0.85f, 0.85f, 1f);
                case BoxStyle. Light: return new Color(0.2f, 0.2f, 0.2f, 1f);
                case BoxStyle. Accent: return Color.white;
                default: return Color.white;
            }
        }

        private Texture2D GetIcon(string iconName)
        {
            return EditorGUIUtility.IconContent(iconName)?.image as Texture2D;
        }
    }
}
#endif