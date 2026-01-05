using UnityEngine;
using UnityEditor;
using VahTyah;

namespace VahTyah. List
{
    /// <summary>
    /// Render footer buttons (Add/Remove)
    /// </summary>
    public class FooterRenderer
    {
        private ListRectCalculator rectCalculator;
        private ListStyleManager styleManager;

        public SimpleCustomList.AddElementWithDropdownCallbackDelegate AddElementWithDropdownCallback { get; set; }
        public SimpleCustomList.AddElementCallbackDelegate AddElementCallback { get; set; }
        public SimpleCustomList.RemoveElementCallbackDelegate RemoveElementCallback { get; set; }

        public void Initialize(ListRectCalculator rectCalculator, ListStyleManager styleManager)
        {
            this.rectCalculator = rectCalculator;
            this.styleManager = styleManager;
        }

        public void Draw(bool enableAddButton, bool enableRemoveButton, int selectedIndex, int elementCount)
        {
            rectCalculator. CalculateFooterButtonsRect(enableAddButton, enableRemoveButton);

            // Draw background
            LayerDrawingSystem.DrawLayers(rectCalculator.ButtonsRect, styleManager.FooterBackgroundConfig);

            GUIStyle buttonStyle = new GUIStyle("RL FooterButton");
            
            float buttonX = rectCalculator.ButtonsRect.x + 4;
            float buttonY = rectCalculator.ButtonsRect.y + (rectCalculator.ButtonsRect. height - 16f) / 2;

            // Add button
            if (enableAddButton)
            {
                Rect addButtonRect = new Rect(buttonX, buttonY, 25, 16);
                DrawAddButton(addButtonRect, buttonStyle);
                buttonX += 25;
            }

            // Remove button
            if (enableRemoveButton)
            {
                Rect removeButtonRect = new Rect(buttonX, buttonY, 25, 16);
                DrawRemoveButton(removeButtonRect, buttonStyle, selectedIndex, elementCount);
            }
        }

        private void DrawAddButton(Rect rect, GUIStyle buttonStyle)
        {
            GUIContent addIcon = AddElementWithDropdownCallback != null
                ? EditorGUIUtility.TrIconContent("Toolbar Plus More")
                : EditorGUIUtility.TrIconContent("Toolbar Plus");

            if (GUI.Button(rect, addIcon, buttonStyle))
            {
                if (AddElementWithDropdownCallback != null)
                {
                    AddElementWithDropdownCallback. Invoke(rect);
                }
                else
                {
                    AddElementCallback?. Invoke();
                }
            }
        }

        private void DrawRemoveButton(Rect rect, GUIStyle buttonStyle, int selectedIndex, int elementCount)
        {
            using (new EditorGUI.DisabledScope(selectedIndex < 0 || selectedIndex >= elementCount))
            {
                if (GUI. Button(rect, EditorGUIUtility.TrIconContent("Toolbar Minus"), buttonStyle))
                {
                    RemoveElementCallback?.Invoke();
                }
            }
        }
    }
}