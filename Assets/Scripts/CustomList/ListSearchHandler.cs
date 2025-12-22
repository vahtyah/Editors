﻿using System;
using System.Collections. Generic;
using UnityEditor;

namespace Watermelon.List
{
    /// <summary>
    /// Xử lý search/filter logic
    /// </summary>
    public class ListSearchHandler
    {
        private string searchQuery = "";
        private List<int> filteredIndices = new List<int>();
        private bool isSearchActive = false;

        private ListDataManager dataManager;
        private SimpleCustomList. SearchFilterDelegate searchFilterCallback;

        // Callback when search query changes
        public System.Action SearchChangedCallback;

        public string SearchQuery
        {
            get => searchQuery;
            set
            {
                searchQuery = value;
                UpdateFilter();
                SearchChangedCallback?.Invoke();
            }
        }

        public bool IsActive => isSearchActive;
        public List<int> FilteredIndices => filteredIndices;
        public int DisplayCount => isSearchActive ? filteredIndices. Count : dataManager.Count;

        public void Initialize(ListDataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        public void SetFilterCallback(SimpleCustomList.SearchFilterDelegate callback)
        {
            searchFilterCallback = callback;
        }

        public void UpdateFilter()
        {
            filteredIndices.Clear();

            if (string.IsNullOrEmpty(searchQuery))
            {
                isSearchActive = false;
                return;
            }

            isSearchActive = true;

            for (int i = 0; i < dataManager.Count; i++)
            {
                if (MatchesSearchQuery(i))
                {
                    filteredIndices.Add(i);
                }
            }
        }

        private bool MatchesSearchQuery(int index)
        {
            if (string.IsNullOrEmpty(searchQuery))
                return true;

            // Use custom filter callback if provided
            if (searchFilterCallback != null)
            {
                SerializedProperty prop = dataManager.GetElement(index);
                return searchFilterCallback(prop, index, searchQuery);
            }

            // Default:  search in element label (case-insensitive)
            string label = dataManager.GetElementLabel(index);
            return label.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public void Clear()
        {
            searchQuery = "";
            filteredIndices.Clear();
            isSearchActive = false;
            SearchChangedCallback?.Invoke();
        }

        public int GetActualIndex(int displayIndex)
        {
            if (isSearchActive && displayIndex >= 0 && displayIndex < filteredIndices.Count)
            {
                return filteredIndices[displayIndex];
            }
            return displayIndex;
        }
    }
}