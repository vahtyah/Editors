using UnityEngine;
using UnityEditor;

namespace VahTyah.List
{
    /// <summary>
    /// Xử lý mouse input (clicks, double-click, context menu)
    /// </summary>
    public class ListInputHandler
    {
        private const double DOUBLE_CLICK_TIME = 0.3;

        private ListDataManager dataManager;
        private ListRectCalculator rectCalculator;
        private ListStyleManager styleManager;
        private ListDragDropHandler dragDropHandler;

        private double lastClickTime = 0;
        private int lastClickedIndex = -1;
        private int selectedIndex = -1;

        public SimpleCustomList.SelectionChangedCallbackDelegate SelectionChangedCallback { get; set; }
        public SimpleCustomList.ElementDoubleClickedDelegate DoubleClickedCallback { get; set; }
        public SimpleCustomList.DisplayContextMenuCallbackDelegate DisplayContextMenuCallback { get; set; }

        public void Initialize(ListDataManager dataManager, ListRectCalculator rectCalculator,
            ListStyleManager styleManager, ListDragDropHandler dragDropHandler)
        {
            this.dataManager = dataManager;
            this.rectCalculator = rectCalculator;
            this.styleManager = styleManager;
            this.dragDropHandler = dragDropHandler;
        }

        public void SetSelectedIndex(int index)
        {
            selectedIndex = index;
        }

        public bool HandleElementInput(Rect elementRect, int index)
        {
            Event currentEvent = Event.current;

            if (dragDropHandler.IsDragging)
                return false;

            // Right click - context menu
            if (currentEvent.type == EventType.MouseDown && 
                elementRect.Contains(currentEvent.mousePosition) &&
                currentEvent. button == 1)
            {
                return HandleRightClick(index, currentEvent);
            }

            // Left click - selection/double-click
            if (currentEvent.type == EventType.MouseUp &&
                elementRect.Contains(currentEvent.mousePosition) &&
                currentEvent.button == 0)
            {
                return HandleLeftClick(index, currentEvent);
            }

            // Track mouse down for drag detection
            if (currentEvent.type == EventType.MouseDown &&
                elementRect.Contains(currentEvent.mousePosition) &&
                currentEvent.button == 0)
            {
                dragDropHandler.TrackMouseDown(currentEvent.mousePosition, index, elementRect.height);
            }

            return false;
        }

        private bool HandleRightClick(int index, Event currentEvent)
        {
            if (selectedIndex != index)
            {
                OnSelectionChanged(index);
            }

            if (DisplayContextMenuCallback != null)
            {
                DisplayContextMenuCallback. Invoke(index);
            }
            else
            {
                ShowDefaultContextMenu(index);
            }

            currentEvent.Use();
            return true;
        }

        private bool HandleLeftClick(int index, Event currentEvent)
        {
            double currentTime = EditorApplication.timeSinceStartup;
            bool isDoubleClick = false;

            if (lastClickedIndex == index && (currentTime - lastClickTime) < DOUBLE_CLICK_TIME)
            {
                isDoubleClick = true;
                lastClickTime = 0;
                lastClickedIndex = -1;
            }
            else
            {
                lastClickTime = currentTime;
                lastClickedIndex = index;
            }

            if (isDoubleClick)
            {
                if (DoubleClickedCallback != null)
                {
                    DoubleClickedCallback. Invoke(index);
                    currentEvent.Use();
                    return true;
                }
            }
            else
            {
                OnSelectionChanged(index);
            }

            currentEvent.Use();
            return true;
        }

        private void ShowDefaultContextMenu(int index)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Duplicate"), false, () =>
            {
                dataManager. DuplicateElement(index);
            });

            menu.AddSeparator("");

            if (index > 0)
            {
                menu.AddItem(new GUIContent("Move to Top"), false, () =>
                {
                    dataManager.MoveElement(index, 0);
                    OnSelectionChanged(0);
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Move to Top"));
            }

            if (index < dataManager.Count - 1)
            {
                menu.AddItem(new GUIContent("Move to Bottom"), false, () =>
                {
                    dataManager.MoveElement(index, dataManager.Count - 1);
                    OnSelectionChanged(dataManager.Count - 1);
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Move to Bottom"));
            }

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Delete"), false, () =>
            {
                selectedIndex = index;
                // RemoveElement would be called via callback
            });

            menu.ShowAsContext();
        }

        private void OnSelectionChanged(int index)
        {
            selectedIndex = index;
            SelectionChangedCallback?.Invoke();
        }
    }
}