using UnityEngine;

namespace VahTyah.List
{
    /// <summary>
    /// Tính toán tất cả rectangles cho list components
    /// </summary>
    public class ListRectCalculator
    {
        // Constants
        private const float HEADER_HEIGHT = 20f;
        private const float SEARCH_HEIGHT = 22f;
        private const float PAGINATION_HEIGHT = 20f;
        private const float FOOTER_HEIGHT = 20f;
        
        // Input settings
        private float minHeight = 200f;
        private float minWidth = 150f;
        private bool stretchHeight = true;
        private bool stretchWidth = true;
        private float collapsedElementHeight = 20f;
        
        // Feature flags
        private bool enableHeader;
        private bool enableSearch;
        private bool enablePagination;
        
        // Pagination data
        private int currentPage;
        private int pageBeginIndex;
        private int pageElementCount;
        
        // Output rects
        public Rect GlobalRect { get; private set; }
        public Rect HeaderRect { get; private set; }
        public Rect HeaderContentRect { get; private set; }
        public Rect SearchRect { get; private set; }
        public Rect SearchFieldRect { get; private set; }
        public Rect SearchClearButtonRect { get; private set; }
        public Rect ListRect { get; private set; }
        public Rect ListContentRect { get; private set; }
        public Rect FooterPaginationRect { get; private set; }
        public Rect PaginationContentRect { get; private set; }
        public Rect FirstPageButtonRect { get; private set; }
        public Rect PreviousPageButtonRect { get; private set; }
        public Rect NextPageButtonRect { get; private set; }
        public Rect LastPageButtonRect { get; private set; }
        public Rect PaginationLabelRect { get; private set; }
        public Rect FooterButtonsRect { get; private set; }
        public Rect ButtonsRect { get; private set; }
        public Rect FilledElementsRect { get; private set; }
        
        // Element template rects
        public Rect ElementHeaderRect { get; private set; }
        public Rect DraggingHandleRect { get; private set; }
        public Rect LabelRect { get; private set; }
        public Rect RemoveButtonRect { get; private set; }
        
        // Cache
        private Rect calculatedGlobalRect;
        private GUIStyle controlStyle;
        
        public void Initialize(float minHeight, float minWidth, bool stretchHeight, bool stretchWidth, float elementHeight)
        {
            this.minHeight = minHeight;
            this.minWidth = minWidth;
            this.stretchHeight = stretchHeight;
            this.stretchWidth = stretchWidth;
            this.collapsedElementHeight = elementHeight;
            
            controlStyle = new GUIStyle();
            controlStyle.stretchHeight = stretchHeight;
            controlStyle.stretchWidth = stretchWidth;
        }
        
        public void Calculate(bool enableHeader, bool enableSearch, bool enablePagination, 
            int displayCount, int selectedIndex, int maxElementCount, int pageElementCount,
            int currentPage, bool enableElementRemoveButton)
        {
            this.enableHeader = enableHeader;
            this.enableSearch = enableSearch;
            this.enablePagination = enablePagination;
            this.currentPage = currentPage;
            this.pageElementCount = pageElementCount;
            
            CalculateGlobalRect();
            
            if (ShouldSkipRecalculation())
                return;
            
            calculatedGlobalRect = GlobalRect;
            
            CalculateSubRects(displayCount, selectedIndex, maxElementCount, enableElementRemoveButton);
        }
        
        private void CalculateGlobalRect()
        {
            GlobalRect = GUILayoutUtility.GetRect(
                GUIContent.none,
                controlStyle,
                GUILayout.MinHeight(minHeight),
                GUILayout.MinWidth(minWidth)
            );
        }
        
        private bool ShouldSkipRecalculation()
        {
            if (GlobalRect. height < 5)
                return true;
            
            if (calculatedGlobalRect == GlobalRect &&
                (Event.current.type != EventType.Layout && Event.current.type != EventType. Repaint))
            {
                return true;
            }
            
            return false;
        }
        
        private void CalculateSubRects(int displayCount, int selectedIndex, int maxElementCount, bool enableElementRemoveButton)
        {
            Rect listRect = new Rect(GlobalRect.x, GlobalRect.y, GlobalRect.width, GlobalRect.height);
            
            // Header
            if (enableHeader)
            {
                HeaderRect = new Rect(GlobalRect.x, GlobalRect.y, GlobalRect.width, HEADER_HEIGHT);
                HeaderContentRect = new Rect(
                    HeaderRect.x + 6,
                    HeaderRect.y + 2,
                    HeaderRect.width - 12,
                    HeaderRect.height - 4
                );
                listRect.yMin += HEADER_HEIGHT;
            }
            
            // Search
            if (enableSearch)
            {
                float searchY = enableHeader ? HeaderRect.yMax : GlobalRect.y;
                SearchRect = new Rect(GlobalRect.x, searchY, GlobalRect.width, SEARCH_HEIGHT);
                
                SearchFieldRect = new Rect(
                    SearchRect.x + 6,
                    SearchRect.y + 2,
                    SearchRect.width - 12,
                    SearchRect.height - 4
                );
                
                SearchClearButtonRect = new Rect(
                    SearchFieldRect.xMax - 18,
                    SearchFieldRect.y - 1,
                    20,
                    SearchFieldRect.height - 2
                );
                
                listRect.yMin += SEARCH_HEIGHT;
            }
            
            // Footer
            FooterButtonsRect = new Rect(
                GlobalRect.x,
                GlobalRect.yMax - FOOTER_HEIGHT,
                GlobalRect.width,
                FOOTER_HEIGHT
            );
            listRect.yMax -= FOOTER_HEIGHT;
            
            // Pagination
            if (enablePagination)
            {
                FooterPaginationRect = new Rect(
                    GlobalRect.x,
                    FooterButtonsRect.y - PAGINATION_HEIGHT,
                    GlobalRect.width,
                    PAGINATION_HEIGHT
                );
                listRect.yMax -= PAGINATION_HEIGHT;
                
                CalculatePaginationRects();
            }
            
            // List content
            ListRect = listRect;
            ListContentRect = new Rect(
                ListRect.x + 6,
                ListRect.y + 2,
                ListRect.width - 12,
                ListRect.height - 4
            );
            
            // Element templates
            CalculateElementTemplateRects(enableElementRemoveButton);
            
            // Filled elements rect
            int displayElementCount = enablePagination
                ?  Mathf.Min(pageElementCount, displayCount - pageBeginIndex)
                : displayCount;
            
            FilledElementsRect = new Rect(
                ListContentRect.x,
                ListContentRect.y,
                ListContentRect.width,
                displayElementCount * collapsedElementHeight
            );
        }
        
        private void CalculatePaginationRects()
        {
            PaginationContentRect = new Rect(
                FooterPaginationRect.x + 6,
                FooterPaginationRect.y + 2,
                FooterPaginationRect.width - 12,
                FooterPaginationRect. height - 4
            );
            
            float buttonWidth = 25f;
            float buttonHeight = 16f;
            float buttonY = PaginationContentRect.y + (PaginationContentRect.height - buttonHeight) / 2;
            
            FirstPageButtonRect = new Rect(PaginationContentRect.xMin, buttonY, buttonWidth, buttonHeight);
            PreviousPageButtonRect = new Rect(FirstPageButtonRect.xMax, buttonY, buttonWidth, buttonHeight);
            NextPageButtonRect = new Rect(PaginationContentRect.xMax - (2 * buttonWidth), buttonY, buttonWidth, buttonHeight);
            LastPageButtonRect = new Rect(PaginationContentRect.xMax - buttonWidth, buttonY, buttonWidth, buttonHeight);
            
            PaginationLabelRect = new Rect(
                PreviousPageButtonRect.xMax,
                buttonY,
                NextPageButtonRect.xMin - PreviousPageButtonRect.xMax,
                buttonHeight
            );
        }
        
        private void CalculateElementTemplateRects(bool enableElementRemoveButton)
        {
            const float DRAG_HANDLE_ALLOCATED_SPACE = 20f;
            const float REMOVE_BUTTON_ALLOCATED_SPACE = 26f;
            const float DRAG_HANDLE_WIDTH = 10f;
            const float REMOVE_BUTTON_WIDTH = 20f;
            
            ElementHeaderRect = new Rect(
                ListContentRect.x,
                ListContentRect.y,
                ListContentRect.width,
                collapsedElementHeight
            );
            
            DraggingHandleRect = new Rect(
                ElementHeaderRect.x + 5,
                ElementHeaderRect.yMax - 6 - 6,
                DRAG_HANDLE_WIDTH,
                6
            );
            
            Rect labelRect = new Rect(
                ElementHeaderRect.x + DRAG_HANDLE_ALLOCATED_SPACE,
                ElementHeaderRect.y,
                ElementHeaderRect.width - DRAG_HANDLE_ALLOCATED_SPACE,
                ElementHeaderRect.height
            );
            
            if (enableElementRemoveButton)
            {
                RemoveButtonRect = new Rect(
                    ElementHeaderRect.xMax - REMOVE_BUTTON_WIDTH,
                    ElementHeaderRect.y,
                    REMOVE_BUTTON_WIDTH,
                    collapsedElementHeight
                );
                labelRect.xMax -= REMOVE_BUTTON_ALLOCATED_SPACE;
            }
            
            LabelRect = labelRect;
        }
        
        public void CalculateFooterButtonsRect(bool enableFooterAddButton, bool enableFooterRemoveButton)
        {
            float rightEdge = FooterButtonsRect.xMax - 10;
            float leftEdge = rightEdge - 4 - 4 - 25;
            
            if (enableFooterAddButton && enableFooterRemoveButton)
                leftEdge -= 25;
            
            ButtonsRect = new Rect(
                leftEdge,
                FooterButtonsRect.y,
                rightEdge - leftEdge,
                FOOTER_HEIGHT
            );
        }
    }
}