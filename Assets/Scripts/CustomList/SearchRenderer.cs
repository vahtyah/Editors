using UnityEngine;
using UnityEditor;
using VahTyah;

namespace VahTyah.List
{
    /// <summary>
    /// Render search field
    /// </summary>
    public class SearchRenderer
    {
        private ListRectCalculator rectCalculator;
        private ListStyleManager styleManager;
        private ListSearchHandler searchHandler;

        public void Initialize(ListRectCalculator rectCalculator, ListStyleManager styleManager, ListSearchHandler searchHandler)
        {
            this.rectCalculator = rectCalculator;
            this.styleManager = styleManager;
            this.searchHandler = searchHandler;
        }

        public void Draw()
        {
            // Draw background
            LayerConfiguration searchBgConfig = GetSearchBackgroundConfig();
            LayerDrawingSystem.DrawLayers(rectCalculator.SearchRect, searchBgConfig);

            // Draw search field
            Rect adjustedSearchFieldRect = rectCalculator.SearchFieldRect;
            if (! string.IsNullOrEmpty(searchHandler.SearchQuery))
            {
                adjustedSearchFieldRect. width -= 20; // Space for clear button
            }

            EditorGUI.BeginChangeCheck();
            GUI.SetNextControlName("SearchField");
            string newSearch = EditorGUI.TextField(adjustedSearchFieldRect, searchHandler.SearchQuery, UnityEditor.EditorStyles.toolbarSearchField);

            if (EditorGUI. EndChangeCheck())
            {
                searchHandler.SearchQuery = newSearch;
            }

            // Draw clear button
            if (! string.IsNullOrEmpty(searchHandler.SearchQuery))
            {
                DrawClearButton();
            }
        }

        private LayerConfiguration GetSearchBackgroundConfig()
        {
            if (styleManager.CurrentStyle?.searchField?. backgroundConfig != null)
            {
                return styleManager.CurrentStyle.searchField.backgroundConfig;
            }
            return styleManager.HeaderBackgroundConfig; // Fallback
        }

        private void DrawClearButton()
        {
            GUIStyle clearButtonStyle = new GUIStyle(GUI.skin.label);
            clearButtonStyle.alignment = TextAnchor.MiddleCenter;
            clearButtonStyle.fontSize = 14;
            clearButtonStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f, 1f);
            clearButtonStyle.hover.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            clearButtonStyle. active.textColor = Color.white;

            if (GUI.Button(rectCalculator.SearchClearButtonRect, "×", clearButtonStyle))
            {
                searchHandler.Clear();
                GUI.FocusControl(null);
            }
        }
    }
}