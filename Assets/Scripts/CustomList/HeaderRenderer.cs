using UnityEngine;
using UnityEditor;
using CustomLayerDrawing;

namespace Watermelon. List
{
    /// <summary>
    /// Render header của list
    /// </summary>
    public class HeaderRenderer
    {
        private ListRectCalculator rectCalculator;
        private ListStyleManager styleManager;
        private ListDataManager dataManager;
        private SimpleCustomList.GetHeaderLabelCallbackDelegate getHeaderLabelCallback;

        public void Initialize(ListRectCalculator rectCalculator, ListStyleManager styleManager, ListDataManager dataManager)
        {
            this.rectCalculator = rectCalculator;
            this.styleManager = styleManager;
            this.dataManager = dataManager;
        }

        public void SetHeaderLabelCallback(SimpleCustomList. GetHeaderLabelCallbackDelegate callback)
        {
            getHeaderLabelCallback = callback;
        }

        public void Draw()
        {
            // Draw background
            LayerDrawingSystem.DrawLayers(rectCalculator.HeaderRect, styleManager.HeaderBackgroundConfig);

            // Draw label (left aligned)
            GUIStyle headerStyle = CreateHeaderStyle();
            headerStyle. alignment = TextAnchor. MiddleLeft;
            EditorGUI.LabelField(rectCalculator.HeaderContentRect, GetHeaderLabel(), headerStyle);

            // Draw size (right aligned)
            GUIStyle sizeStyle = CreateHeaderStyle();
            sizeStyle.alignment = TextAnchor.MiddleRight;
            EditorGUI.LabelField(rectCalculator.HeaderContentRect, $"Size: {dataManager.Count}", sizeStyle);
        }

        private GUIStyle CreateHeaderStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);

            if (styleManager.CurrentStyle?. header != null)
            {
                style. normal.textColor = styleManager. CurrentStyle.header.textColor;
            }

            return style;
        }

        private string GetHeaderLabel()
        {
            return getHeaderLabelCallback?.Invoke() ?? "List";
        }
    }
}