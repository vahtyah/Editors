﻿using UnityEngine;
using UnityEditor;
using VahTyah;

namespace VahTyah.List
{
    /// <summary>
    /// Render individual elements
    /// </summary>
    public class ElementRenderer
    {
        private const float DRAG_HANDLE_WIDTH = 10f;
        private const float DRAG_HANDLE_ALLOCATED_SPACE = 20f;

        private ListRectCalculator rectCalculator;
        private ListStyleManager styleManager;
        private ListDataManager dataManager;
        private ListSearchHandler searchHandler;
        private ListInputHandler inputHandler;

        private int selectedIndex = -1;

        public void Initialize(ListRectCalculator rectCalculator, ListStyleManager styleManager, 
            ListDataManager dataManager, ListSearchHandler searchHandler, ListInputHandler inputHandler)
        {
            this.rectCalculator = rectCalculator;
            this.styleManager = styleManager;
            this.dataManager = dataManager;
            this.searchHandler = searchHandler;
            this.inputHandler = inputHandler;
        }

        public void SetSelectedIndex(int index)
        {
            selectedIndex = index;
        }

        public void DrawElement(Rect rect, int index, bool isSelected)
        {
            DrawBackground(rect, isSelected);
            DrawDragHandle(rect);
            DrawLabel(rect, index);
            
            // Handle mouse input (clicks, context menu)
            inputHandler?.HandleElementInput(rect, index);
        }

        private void DrawBackground(Rect rect, bool isSelected)
        {
            Rect bgRect = new Rect(rect.x, rect.y, rect.width, styleManager.CollapsedElementHeight);

            if (isSelected)
                LayerDrawingSystem.DrawLayers(bgRect, styleManager. SelectedElementConfig);
            else
                LayerDrawingSystem.DrawLayers(bgRect, styleManager.UnselectedElementConfig);
        }

        private void DrawDragHandle(Rect rect)
        {
            if (Event.current.type == EventType. Repaint)
            {
                Rect dragRect = new Rect(
                    rect.x + 5,
                    rect.yMax - 6 - 6,
                    DRAG_HANDLE_WIDTH,
                    6
                );

                GUIStyle dragStyle = new GUIStyle("RL DragHandle");
                dragStyle. Draw(dragRect, false, false, false, false);
            }
        }

        private void DrawLabel(Rect rect, int index)
        {
            Rect labelRect = new Rect(
                rect.x + DRAG_HANDLE_ALLOCATED_SPACE,
                rect.y,
                rect.width - DRAG_HANDLE_ALLOCATED_SPACE,
                styleManager.CollapsedElementHeight
            );

            if (styleManager.EnableElementRemoveButton)
            {
                const float REMOVE_BUTTON_ALLOCATED_SPACE = 26f;
                labelRect.xMax -= REMOVE_BUTTON_ALLOCATED_SPACE;
            }

            string label = dataManager.GetElementLabel(index);
            GUIStyle labelStyle = CreateLabelStyle();
            GUI.Label(labelRect, label, labelStyle);
        }

        private GUIStyle CreateLabelStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);

            if (styleManager.CurrentStyle?.element != null)
            {
                style.normal.textColor = styleManager.CurrentStyle.element.textColor;
            }

            return style;
        }
    }
}