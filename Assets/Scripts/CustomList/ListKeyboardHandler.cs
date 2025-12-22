using UnityEngine;

namespace Watermelon.List
{
    /// <summary>
    /// Xử lý keyboard navigation
    /// </summary>
    public class ListKeyboardHandler
    {
        private ListDataManager dataManager;
        private ListPaginationManager paginationManager;

        private int selectedIndex = -1;

        public SimpleCustomList.SelectionChangedCallbackDelegate SelectionChangedCallback { get; set; }
        public SimpleCustomList.RemoveElementCallbackDelegate RemoveElementCallback { get; set; }

        public void Initialize(ListDataManager dataManager, ListPaginationManager paginationManager)
        {
            this.dataManager = dataManager;
            this.paginationManager = paginationManager;
        }

        public void SetSelectedIndex(int index)
        {
            selectedIndex = index;
        }

        public int GetSelectedIndex()
        {
            return selectedIndex;
        }

        public void HandleKeyboard()
        {
            Event currentEvent = Event.current;

            if (currentEvent.type != EventType.KeyDown || currentEvent.keyCode == KeyCode.None)
                return;

            bool handled = false;

            switch (currentEvent.keyCode)
            {
                case KeyCode.UpArrow:
                    handled = SelectPrevious();
                    break;

                case KeyCode.DownArrow:
                    handled = SelectNext();
                    break;

                case KeyCode. LeftArrow:
                    handled = PreviousPage();
                    break;

                case KeyCode.RightArrow:
                    handled = NextPage();
                    break;

                case KeyCode.Home:
                    handled = SelectFirst();
                    break;

                case KeyCode.End:
                    handled = SelectLast();
                    break;

                case KeyCode.Delete:
                case KeyCode.Backspace:
                    handled = DeleteSelected();
                    break;

                case KeyCode.PageUp:
                    handled = FirstPage();
                    break;

                case KeyCode.PageDown:
                    handled = LastPage();
                    break;
            }

            if (handled)
            {
                currentEvent. Use();
            }
        }

        private bool SelectPrevious()
        {
            if (selectedIndex > 0)
            {
                OnSelectionChanged(selectedIndex - 1);

                if (paginationManager.EnablePagination && selectedIndex < paginationManager.PageBeginIndex)
                {
                    paginationManager.PreviousPage();
                }

                return true;
            }
            return false;
        }

        private bool SelectNext()
        {
            if (selectedIndex < dataManager.Count - 1)
            {
                OnSelectionChanged(selectedIndex + 1);

                if (paginationManager.EnablePagination && 
                    selectedIndex >= paginationManager.PageBeginIndex + paginationManager.PageElementCount)
                {
                    paginationManager.NextPage();
                }

                return true;
            }
            return false;
        }

        private bool PreviousPage()
        {
            if (paginationManager.EnablePagination && paginationManager. CanGoToPreviousPage())
            {
                paginationManager.PreviousPage();
                selectedIndex = -1;
                return true;
            }
            return false;
        }

        private bool NextPage()
        {
            if (paginationManager.EnablePagination && paginationManager. CanGoToNextPage())
            {
                paginationManager. NextPage();
                selectedIndex = -1;
                return true;
            }
            return false;
        }

        private bool SelectFirst()
        {
            if (dataManager.Count > 0)
            {
                OnSelectionChanged(0);
                if (paginationManager.EnablePagination)
                {
                    paginationManager.FirstPage();
                }
                return true;
            }
            return false;
        }

        private bool SelectLast()
        {
            if (dataManager.Count > 0)
            {
                OnSelectionChanged(dataManager. Count - 1);
                if (paginationManager.EnablePagination)
                {
                    paginationManager.LastPage();
                }
                return true;
            }
            return false;
        }

        private bool DeleteSelected()
        {
            if (selectedIndex >= 0 && selectedIndex < dataManager.Count)
            {
                RemoveElementCallback?.Invoke();
                return true;
            }
            return false;
        }

        private bool FirstPage()
        {
            if (paginationManager. EnablePagination && paginationManager.CanGoToPreviousPage())
            {
                paginationManager.FirstPage();
                selectedIndex = -1;
                return true;
            }
            return false;
        }

        private bool LastPage()
        {
            if (paginationManager.EnablePagination && paginationManager. CanGoToNextPage())
            {
                paginationManager. LastPage();
                selectedIndex = -1;
                return true;
            }
            return false;
        }

        private void OnSelectionChanged(int index)
        {
            selectedIndex = index;
            SelectionChangedCallback?.Invoke();
        }
    }
}