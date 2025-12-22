using UnityEngine;
using UnityEditor;
using CustomLayerDrawing;

namespace Watermelon.List
{
    /// <summary>
    /// Render pagination controls
    /// </summary>
    public class PaginationRenderer
    {
        private ListRectCalculator rectCalculator;
        private ListStyleManager styleManager;
        private ListPaginationManager paginationManager;
        private ListSearchHandler searchHandler;

        public void Initialize(ListRectCalculator rectCalculator, ListStyleManager styleManager,
            ListPaginationManager paginationManager, ListSearchHandler searchHandler)
        {
            this.rectCalculator = rectCalculator;
            this.styleManager = styleManager;
            this.paginationManager = paginationManager;
            this.searchHandler = searchHandler;
        }

        public void Draw()
        {
            // Draw background
            LayerDrawingSystem.DrawLayers(rectCalculator.FooterPaginationRect, styleManager.PaginationBackgroundConfig);

            GUIStyle buttonStyle = new GUIStyle("RL FooterButton");
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.alignment = TextAnchor.MiddleCenter;

            DrawButtons(buttonStyle);
            DrawLabel(labelStyle);
        }

        private void DrawButtons(GUIStyle buttonStyle)
        {
            // First page
            using (new EditorGUI.DisabledScope(! paginationManager. CanGoToPreviousPage()))
            {
                if (GUI.Button(rectCalculator.FirstPageButtonRect, "<<", buttonStyle))
                {
                    paginationManager.FirstPage();
                }
            }

            // Previous page
            using (new EditorGUI.DisabledScope(! paginationManager.CanGoToPreviousPage()))
            {
                if (GUI.Button(rectCalculator.PreviousPageButtonRect, "<", buttonStyle))
                {
                    paginationManager.PreviousPage();
                }
            }

            // Next page
            using (new EditorGUI.DisabledScope(!paginationManager. CanGoToNextPage()))
            {
                if (GUI. Button(rectCalculator.NextPageButtonRect, ">", buttonStyle))
                {
                    paginationManager.NextPage();
                }
            }

            // Last page
            using (new EditorGUI.DisabledScope(!paginationManager.CanGoToNextPage()))
            {
                if (GUI.Button(rectCalculator.LastPageButtonRect, ">>", buttonStyle))
                {
                    paginationManager.LastPage();
                }
            }
        }

        private void DrawLabel(GUIStyle labelStyle)
        {
            string paginationText;
            if (searchHandler.IsActive)
            {
                paginationText = $"{paginationManager.CurrentPage + 1} / {paginationManager.PagesCount} ({searchHandler.FilteredIndices. Count} results)";
            }
            else
            {
                paginationText = $"{paginationManager.CurrentPage + 1} / {paginationManager.PagesCount}";
            }

            GUI.Label(rectCalculator.PaginationLabelRect, paginationText, labelStyle);
        }
    }
}