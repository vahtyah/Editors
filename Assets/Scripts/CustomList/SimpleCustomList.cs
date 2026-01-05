using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VahTyah.List
{
    /// <summary>
    /// Main coordinator class - delegates to specialized components
    /// Clean, modular, and maintainable
    /// </summary>
    public class SimpleCustomList
    {
        // Delegates
        public delegate string GetLabelDelegate(SerializedProperty elementProperty, int elementIndex);
        public delegate string GetHeaderLabelCallbackDelegate();
        public delegate void SelectionChangedCallbackDelegate();
        public delegate void ListChangedCallbackDelegate();
        public delegate void AddElementCallbackDelegate();
        public delegate void RemoveElementCallbackDelegate();
        public delegate void ListReorderedCallbackDelegate();
        public delegate void ListReorderedCallbackWithDetailsDelegate(int srcIndex, int destIndex);
        public delegate void AddElementWithDropdownCallbackDelegate(Rect buttonRect);
        public delegate void DisplayContextMenuCallbackDelegate(int index);
        public delegate void ListUndoCallbackDelegate(string undoMessage);
        public delegate void ElementDoubleClickedDelegate(int index);
        public delegate bool SearchFilterDelegate(SerializedProperty elementProperty, int elementIndex, string searchQuery);

        // Components
        private ListDataManager dataManager;
        private ListStyleManager styleManager;
        private ListRectCalculator rectCalculator;
        private ListSearchHandler searchHandler;
        private ListPaginationManager paginationManager;
        private ListRenderer renderer;
        private ListDragDropHandler dragDropHandler;
        private ListKeyboardHandler keyboardHandler;
        private ListInputHandler inputHandler;

        // State
        private int selectedIndex = -1;
        private bool executedOnce = false;
        private EditorWindow parentWindow;

        // Callbacks
        public GetHeaderLabelCallbackDelegate getHeaderLabelCallback;
        public SelectionChangedCallbackDelegate selectionChangedCallback;
        public ListChangedCallbackDelegate listChangedCallback;
        public AddElementCallbackDelegate addElementCallback;
        public RemoveElementCallbackDelegate removeElementCallback;
        public AddElementWithDropdownCallbackDelegate addElementWithDropdownCallback;
        public ListReorderedCallbackDelegate listReorderedCallback;
        public ListReorderedCallbackWithDetailsDelegate listReorderedCallbackWithDetails;
        public DisplayContextMenuCallbackDelegate displayContextMenuCallback;
        public ListUndoCallbackDelegate listUndoCallback;
        public ElementDoubleClickedDelegate elementDoubleClickedCallback;
        public SearchFilterDelegate searchFilterCallback;

        // Properties
        public int SelectedIndex
        {
            get => selectedIndex;
            set => selectedIndex = value;
        }

        public EditorWindow ParentWindow
        {
            get => parentWindow;
            set => parentWindow = value;
        }

        public CustomListStyle CurrentCustomStyle => styleManager.CurrentStyle;

        #region Constructors

        public SimpleCustomList(SerializedObject serializedObject, SerializedProperty elements, string labelPropertyName)
        {
            InitializeComponents();
            dataManager. Initialize(serializedObject, elements, labelPropertyName);
            styleManager.LoadStyle();
        }

        public SimpleCustomList(SerializedObject serializedObject, SerializedProperty elements, GetLabelDelegate getLabelCallback)
        {
            InitializeComponents();
            dataManager.Initialize(serializedObject, elements, getLabelCallback);
            styleManager.LoadStyle();
        }

        public SimpleCustomList(SerializedObject serializedObject, List<SerializedProperty> propertyList, string labelPropertyName)
        {
            InitializeComponents();
            dataManager.Initialize(serializedObject, propertyList, labelPropertyName);
            styleManager.LoadStyle();
        }

        public SimpleCustomList(SerializedObject serializedObject, List<SerializedProperty> propertyList, GetLabelDelegate getLabelCallback)
        {
            InitializeComponents();
            dataManager.Initialize(serializedObject, propertyList, getLabelCallback);
            styleManager.LoadStyle();
        }

        public SimpleCustomList(IList elements, GetLabelDelegate getLabelCallback)
        {
            InitializeComponents();
            dataManager.Initialize(elements, getLabelCallback);
            styleManager.LoadStyle();
        }

        private void InitializeComponents()
        {
            dataManager = new ListDataManager();
            styleManager = new ListStyleManager();
            rectCalculator = new ListRectCalculator();
            searchHandler = new ListSearchHandler();
            paginationManager = new ListPaginationManager();
            renderer = new ListRenderer();
            dragDropHandler = new ListDragDropHandler();
            keyboardHandler = new ListKeyboardHandler();
            inputHandler = new ListInputHandler();
        }

        #endregion

        #region Main API

        public void Display()
        {
            ExecuteOnce();

            Event currentEvent = Event.current;
            int prevIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects and pagination
            CalculateLayout();

            // Wire up components
            WireComponents();

            // Draw everything
            renderer.DrawAll();

            // Handle drag & drop (only if not searching)
            if (currentEvent.isMouse && dataManager.Count > 0 &&
                currentEvent.type != EventType.Used && 
                !styleManager. IgnoreDragEvents && 
                !searchHandler.IsActive)
            {
                dragDropHandler.HandleDragDrop();
            }

            // Handle keyboard navigation
            if (currentEvent.type == EventType.KeyDown && dataManager.Count > 0)
            {
                keyboardHandler. HandleKeyboard();
            }

            EditorGUI.indentLevel = prevIndent;
        }

        public void LoadCustomStyle(int index = 0)
        {
            styleManager.LoadStyle(index);
            RequestRepaint();
        }

        public void RequestRepaint()
        {
            parentWindow?. Repaint();
        }

        #endregion

        #region Internal Methods

        private void ExecuteOnce()
        {
            if (executedOnce) return;
            executedOnce = true;

            rectCalculator.Initialize(
                styleManager.MinHeight,
                styleManager.MinWidth,
                styleManager. StretchHeight,
                styleManager.StretchWidth,
                styleManager.CollapsedElementHeight
            );

            searchHandler.Initialize(dataManager);
            keyboardHandler.Initialize(dataManager, paginationManager);
            dragDropHandler.Initialize(dataManager, rectCalculator, styleManager, searchHandler);
            inputHandler.Initialize(dataManager, rectCalculator, styleManager, dragDropHandler);
            renderer.Initialize(rectCalculator, styleManager, dataManager, searchHandler, paginationManager, inputHandler);
        }

        private void CalculateLayout()
        {
            // Calculate available height
            float availableHeight = CalculateAvailableHeight();

            // Calculate pagination
            paginationManager.Calculate(
                availableHeight,
                styleManager. CollapsedElementHeight,
                searchHandler.DisplayCount,
                selectedIndex
            );

            // Calculate all rects
            rectCalculator.Calculate(
                styleManager.EnableHeader,
                styleManager.EnableSearch,
                paginationManager.EnablePagination,
                searchHandler.DisplayCount,
                selectedIndex,
                paginationManager.MaxElementCount,
                paginationManager.PageElementCount,
                paginationManager.CurrentPage,
                styleManager.EnableElementRemoveButton
            );
        }

        private float CalculateAvailableHeight()
        {
            float height = rectCalculator.GlobalRect.height;

            if (styleManager.EnableHeader)
                height -= 20f; // HEADER_HEIGHT

            if (styleManager.EnableSearch)
                height -= 22f; // SEARCH_HEIGHT

            height -= 20f; // FOOTER_HEIGHT
            height -= 4f; // List padding

            return height;
        }

        private void WireComponents()
        {
            // Update search filter callback
            searchHandler.SetFilterCallback(searchFilterCallback);
            
            // Wire search changed callback to reset pagination and repaint
            searchHandler.SearchChangedCallback = () =>
            {
                paginationManager.FirstPage();
                RequestRepaint();
            };

            // Update renderer callbacks
            renderer.SetHeaderLabelCallback(getHeaderLabelCallback);
            renderer.SetSelectedIndex(selectedIndex);

            // Update drag drop callbacks
            dragDropHandler.ListReorderedCallback = () =>
            {
                listReorderedCallback?.Invoke();
                listChangedCallback?.Invoke();
            };

            dragDropHandler.ListReorderedCallbackWithDetails = (from, to) =>
            {
                listReorderedCallbackWithDetails?.Invoke(from, to);
                selectedIndex = to;
                selectionChangedCallback?.Invoke();
            };

            // Update keyboard handler callbacks
            keyboardHandler.SetSelectedIndex(selectedIndex);
            keyboardHandler.SelectionChangedCallback = () =>
            {
                selectedIndex = keyboardHandler.GetSelectedIndex();
                selectionChangedCallback?.Invoke();
                RequestRepaint();
            };
            keyboardHandler.RemoveElementCallback = RemoveElement;

            // Update input handler callbacks
            inputHandler.SetSelectedIndex(selectedIndex);
            inputHandler.SelectionChangedCallback = () =>
            {
                selectionChangedCallback?.Invoke();
                RequestRepaint();
            };
            inputHandler.DoubleClickedCallback = elementDoubleClickedCallback;
            inputHandler.DisplayContextMenuCallback = displayContextMenuCallback;

            // Update footer renderer callbacks
            var footerRenderer = renderer.GetFooterRenderer();
            if (footerRenderer != null)
            {
                footerRenderer. AddElementWithDropdownCallback = addElementWithDropdownCallback;
                footerRenderer.AddElementCallback = AddElement;
                footerRenderer. RemoveElementCallback = RemoveElement;
            }
        }

        private void AddElement()
        {
            UndoCallback("Add Element");
            addElementCallback?.Invoke();
            listChangedCallback?.Invoke();
        }

        private void RemoveElement()
        {
            string elementName = "Element";
            if (selectedIndex >= 0 && selectedIndex < dataManager.Count)
            {
                if (! dataManager.UsingListInterface)
                {
                    elementName = dataManager.GetElementLabel(selectedIndex);
                }
            }
            UndoCallback($"Remove {elementName}");

            removeElementCallback?.Invoke();
            listChangedCallback?.Invoke();
        }

        private void UndoCallback(string undoMessage)
        {
            listUndoCallback?.Invoke(undoMessage);
        }

        #endregion
    }
}