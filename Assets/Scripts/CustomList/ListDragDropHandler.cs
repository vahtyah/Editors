using UnityEngine;
using UnityEditor;

namespace VahTyah.List
{
    /// <summary>
    /// Xử lý drag & drop operations
    /// </summary>
    public class ListDragDropHandler
    {
        private ListDataManager dataManager;
        private ListRectCalculator rectCalculator;
        private ListStyleManager styleManager;
        private ListSearchHandler searchHandler;

        private bool dragging = false;
        private int startDragIndex = -1;
        private int currentDragIndex = -1;
        private float dragOffset = 0;
        private float draggedElementY = 0;
        private float draggedElementHeight = 20;
        private Vector2 lastMouseDownPosition;
        private int lastMouseDownIndex = -1;
        private float lastMouseDownHeight = 20;

        public bool IsDragging => dragging;
        public int StartDragIndex => startDragIndex;
        public int CurrentDragIndex => currentDragIndex;
        public float DraggedElementY => draggedElementY;

        public SimpleCustomList.ListReorderedCallbackDelegate ListReorderedCallback { get; set; }
        public SimpleCustomList.ListReorderedCallbackWithDetailsDelegate ListReorderedCallbackWithDetails { get; set; }

        public void Initialize(ListDataManager dataManager, ListRectCalculator rectCalculator,
            ListStyleManager styleManager, ListSearchHandler searchHandler)
        {
            this.dataManager = dataManager;
            this.rectCalculator = rectCalculator;
            this.styleManager = styleManager;
            this.searchHandler = searchHandler;
        }

        public void HandleDragDrop()
        {
            Event currentEvent = Event.current;

            if (! dragging)
            {
                DetectDragStart(currentEvent);
            }
            else
            {
                if (currentEvent.type == EventType.MouseDrag)
                {
                    UpdateDrag(currentEvent);
                }
                else if (currentEvent.type == EventType.MouseUp)
                {
                    FinishDrag(currentEvent);
                }
            }
        }

        public void TrackMouseDown(Vector2 position, int index, float height)
        {
            lastMouseDownPosition = position;
            lastMouseDownIndex = index;
            lastMouseDownHeight = height;
        }

        private void DetectDragStart(Event currentEvent)
        {
            if (currentEvent.type == EventType.MouseDrag &&
                rectCalculator.ListContentRect.Contains(currentEvent.mousePosition) &&
                lastMouseDownIndex >= 0 &&
                (currentEvent.delta.magnitude < 5f) &&
                ((lastMouseDownPosition - currentEvent.mousePosition).magnitude <= 1f + Mathf.Epsilon))
            {
                StartDrag(currentEvent);
            }
        }

        private void StartDrag(Event currentEvent)
        {
            dragging = true;
            startDragIndex = lastMouseDownIndex;
            currentDragIndex = startDragIndex;
            draggedElementHeight = lastMouseDownHeight;

            float elementY = rectCalculator.ListContentRect.y + (startDragIndex - 0) * styleManager.CollapsedElementHeight;
            dragOffset = lastMouseDownPosition.y - elementY;

            draggedElementY = Mathf.Clamp(
                currentEvent.mousePosition.y - dragOffset,
                rectCalculator. FilledElementsRect.yMin,
                rectCalculator. FilledElementsRect.yMax - draggedElementHeight
            );

            Debug.Log($"Drag Started: startIndex={startDragIndex}");
            currentEvent.Use();
        }

        private void UpdateDrag(Event currentEvent)
        {
            draggedElementY = Mathf. Clamp(
                currentEvent.mousePosition.y - dragOffset,
                rectCalculator.FilledElementsRect.yMin,
                rectCalculator.FilledElementsRect.yMax - draggedElementHeight
            );

            float draggedElementCenter = (draggedElementY - rectCalculator.ListContentRect.y) + (draggedElementHeight * 0.5f);
            int relativeIndex = Mathf.RoundToInt(draggedElementCenter / styleManager.CollapsedElementHeight);
            currentDragIndex = relativeIndex;
            currentDragIndex = Mathf. Clamp(currentDragIndex, 0, dataManager.Count - 1);

            GUI.changed = true;
            currentEvent.Use();
        }

        private void FinishDrag(Event currentEvent)
        {
            Debug.Log($"Drag Finished: from {startDragIndex} to {currentDragIndex}");

            dragging = false;

            if (startDragIndex >= 0 && startDragIndex < dataManager.Count &&
                currentDragIndex >= 0 && currentDragIndex < dataManager.Count &&
                startDragIndex != currentDragIndex)
            {
                dataManager.MoveElement(startDragIndex, currentDragIndex);

                ListReorderedCallback?.Invoke();
                ListReorderedCallbackWithDetails?.Invoke(startDragIndex, currentDragIndex);
            }

            currentEvent.Use();
        }
    }
}