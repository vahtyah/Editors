using UnityEngine;

namespace Watermelon.List
{
    /// <summary>
    /// Quản lý pagination logic
    /// </summary>
    public class ListPaginationManager
    {
        private bool enablePagination = false;
        private int currentPage = 0;
        private int pagesCount = 0;
        private int pageBeginIndex = 0;
        private int pageElementCount = 0;
        private int maxElementCount = 0;

        public bool EnablePagination => enablePagination;
        public int CurrentPage => currentPage;
        public int PagesCount => pagesCount;
        public int PageBeginIndex => pageBeginIndex;
        public int PageElementCount => pageElementCount;
        public int MaxElementCount => maxElementCount;

        public void Calculate(float availableHeight, float elementHeight, int displayCount, int selectedIndex)
        {
            // Calculate max elements that can fit WITHOUT pagination
            maxElementCount = Mathf.FloorToInt(availableHeight / elementHeight);

            // Check if pagination is needed
            enablePagination = (displayCount > maxElementCount);

            if (enablePagination)
            {
                // Subtract pagination height
                const float PAGINATION_HEIGHT = 20f;
                availableHeight -= PAGINATION_HEIGHT;
                pageElementCount = Mathf.FloorToInt(availableHeight / elementHeight);
            }
            else
            {
                pageElementCount = maxElementCount;
            }

            // Calculate pagination values
            if (enablePagination)
            {
                pagesCount = Mathf.CeilToInt((displayCount + 0f) / pageElementCount);

                if (pagesCount > 1)
                {
                    currentPage = Mathf.Clamp(currentPage, 0, pagesCount - 1);

                    // Keep selected element in view
                    if (selectedIndex != -1 && selectedIndex < displayCount)
                    {
                        currentPage = Mathf.FloorToInt((selectedIndex + 0f) / pageElementCount);
                    }
                }

                pageBeginIndex = currentPage * pageElementCount;
            }
            else
            {
                currentPage = 0;
                pageBeginIndex = 0;
                pagesCount = 1;
            }
        }

        public void FirstPage()
        {
            currentPage = 0;
        }

        public void PreviousPage()
        {
            if (currentPage > 0)
                currentPage--;
        }

        public void NextPage()
        {
            if (currentPage < pagesCount - 1)
                currentPage++;
        }

        public void LastPage()
        {
            currentPage = pagesCount - 1;
        }

        public bool CanGoToPreviousPage()
        {
            return currentPage > 0;
        }

        public bool CanGoToNextPage()
        {
            return currentPage < pagesCount - 1;
        }
    }
}