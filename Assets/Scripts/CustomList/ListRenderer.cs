﻿using UnityEngine;
using VahTyah;

namespace VahTyah.List
{
    /// <summary>
    /// Main renderer - coordinates all sub-renderers
    /// </summary>
    public class ListRenderer
    {
        private ListRectCalculator rectCalculator;
        private ListStyleManager styleManager;
        private ListDataManager dataManager;
        private ListSearchHandler searchHandler;
        private ListPaginationManager paginationManager;
        private ListInputHandler inputHandler;

        private HeaderRenderer headerRenderer;
        private SearchRenderer searchRenderer;
        private ElementRenderer elementRenderer;
        private PaginationRenderer paginationRenderer;
        private FooterRenderer footerRenderer;

        private int selectedIndex = -1;

        public void Initialize(ListRectCalculator rectCalculator, ListStyleManager styleManager,
            ListDataManager dataManager, ListSearchHandler searchHandler, ListPaginationManager paginationManager,
            ListInputHandler inputHandler)
        {
            this. rectCalculator = rectCalculator;
            this.styleManager = styleManager;
            this.dataManager = dataManager;
            this. searchHandler = searchHandler;
            this.paginationManager = paginationManager;
            this.inputHandler = inputHandler;

            // Create sub-renderers
            headerRenderer = new HeaderRenderer();
            headerRenderer.Initialize(rectCalculator, styleManager, dataManager);

            searchRenderer = new SearchRenderer();
            searchRenderer.Initialize(rectCalculator, styleManager, searchHandler);

            elementRenderer = new ElementRenderer();
            elementRenderer.Initialize(rectCalculator, styleManager, dataManager, searchHandler, inputHandler);

            paginationRenderer = new PaginationRenderer();
            paginationRenderer.Initialize(rectCalculator, styleManager, paginationManager, searchHandler);

            footerRenderer = new FooterRenderer();
            footerRenderer.Initialize(rectCalculator, styleManager);
        }

        public void SetSelectedIndex(int index)
        {
            selectedIndex = index;
            elementRenderer.SetSelectedIndex(index);
        }

        public void SetHeaderLabelCallback(SimpleCustomList.GetHeaderLabelCallbackDelegate callback)
        {
            headerRenderer.SetHeaderLabelCallback(callback);
        }

        public FooterRenderer GetFooterRenderer()
        {
            return footerRenderer;
        }

        public void DrawAll()
        {
            // Draw global background
            LayerDrawingSystem. DrawLayers(rectCalculator. GlobalRect, styleManager.GlobalBackgroundConfig);

            // Draw header
            if (styleManager.EnableHeader)
            {
                headerRenderer.Draw();
            }

            // Draw search
            if (styleManager.EnableSearch)
            {
                searchRenderer.Draw();
            }

            // Draw list content
            DrawList();

            // Draw pagination
            if (paginationManager.EnablePagination)
            {
                paginationRenderer.Draw();
            }

            // Draw footer
            if (styleManager.EnableFooterAddButton || styleManager.EnableFooterRemoveButton)
            {
                footerRenderer.Draw(styleManager.EnableFooterAddButton, styleManager.EnableFooterRemoveButton, 
                    selectedIndex, dataManager.Count);
            }
        }

        private void DrawList()
        {
            LayerDrawingSystem.DrawLayers(rectCalculator.ListRect, styleManager.ListBackgroundConfig);

            // Check for empty or no results
            if (searchHandler.IsActive && searchHandler.FilteredIndices.Count == 0)
            {
                DrawEmptyMessage(styleManager.NoResultsMessage);
                return;
            }

            if (dataManager.Count == 0)
            {
                DrawEmptyMessage(styleManager.EmptyListMessage);
                return;
            }

            // Draw elements
            DrawElements();
        }

        private void DrawEmptyMessage(string message)
        {
            Rect messageRect = new Rect(
                rectCalculator.ListContentRect.x,
                rectCalculator.ListContentRect.y,
                rectCalculator.ListContentRect.width,
                styleManager.CollapsedElementHeight
            );
            GUI.Label(messageRect, message);
        }

        private void DrawElements()
        {
            float currentY = rectCalculator.ListContentRect.y;

            int displayCount = searchHandler.DisplayCount;
            int endIndex = paginationManager.EnablePagination
                ?  Mathf.Min(paginationManager.PageBeginIndex + paginationManager.PageElementCount, displayCount)
                : displayCount;

            for (int i = paginationManager.PageBeginIndex; i < endIndex; i++)
            {
                int actualIndex = searchHandler.GetActualIndex(i);
                bool isSelected = (actualIndex == selectedIndex);

                Rect elementRect = new Rect(
                    rectCalculator.ListContentRect.x,
                    currentY,
                    rectCalculator.ListContentRect.width,
                    styleManager.CollapsedElementHeight
                );

                elementRenderer.DrawElement(elementRect, actualIndex, isSelected);

                currentY += styleManager.CollapsedElementHeight;
            }
        }
    }
}