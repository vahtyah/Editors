using UnityEngine;
using UnityEditor;
using CustomLayerDrawing;

namespace Watermelon. List
{
    /// <summary>
    /// Quản lý style loading và application
    /// </summary>
    public class ListStyleManager
    {
        private CustomListStyle currentStyle;

        // Output configs
        public LayerConfiguration GlobalBackgroundConfig { get; private set; }
        public LayerConfiguration HeaderBackgroundConfig { get; private set; }
        public LayerConfiguration ListBackgroundConfig { get; private set; }
        public LayerConfiguration PaginationBackgroundConfig { get; private set; }
        public LayerConfiguration FooterBackgroundConfig { get; private set; }
        public LayerConfiguration SelectedElementConfig { get; private set; }
        public LayerConfiguration UnselectedElementConfig { get; private set; }

        // Applied settings
        public bool EnableHeader { get; private set; }
        public bool EnableSearch { get; private set; }
        public bool EnableFooterAddButton { get; private set; }
        public bool EnableFooterRemoveButton { get; private set; }
        public bool EnableElementRemoveButton { get; private set; }
        public bool IgnoreDragEvents { get; private set; }

        public float MinHeight { get; private set; }
        public float MinWidth { get; private set; }
        public bool StretchHeight { get; private set; }
        public bool StretchWidth { get; private set; }
        public float CollapsedElementHeight { get; private set; }

        public string EmptyListMessage { get; private set; }
        public string NoResultsMessage { get; private set; }

        public CustomListStyle CurrentStyle => currentStyle;

        public void LoadStyle(int index = 0)
        {
            ListStylesDatabase database = EditorUtils.GetAsset<ListStylesDatabase>();

            if (database == null)
            {
                currentStyle = new CustomListStyle();
                currentStyle.SetDefaultStyleValues();
            }
            else
            {
                currentStyle = database.GetStyle(index);
            }

            if (currentStyle == null)
            {
                Debug.LogWarning("Cannot load null custom style");
                return;
            }

            ApplyStyle();
        }

        private void ApplyStyle()
        {
            ApplyFeatureFlags();
            ApplyDimensions();
            ApplyMessages();
            ApplyLayerConfigurations();
        }

        private void ApplyFeatureFlags()
        {
            EnableHeader = currentStyle.enableHeader;
            EnableSearch = currentStyle.enableSearch;
            EnableFooterAddButton = currentStyle. enableFooterAddButton;
            EnableFooterRemoveButton = currentStyle.enableFooterRemoveButton;
            EnableElementRemoveButton = currentStyle.enableElementRemoveButton;
            IgnoreDragEvents = currentStyle.ignoreDragEvents;
        }

        private void ApplyDimensions()
        {
            MinHeight = currentStyle.minHeight;
            MinWidth = currentStyle.minWidth;
            StretchHeight = currentStyle.stretchHeight;
            StretchWidth = currentStyle.stretchWidth;
            CollapsedElementHeight = currentStyle.element.collapsedElementHeight;
        }

        private void ApplyMessages()
        {
            EmptyListMessage = currentStyle.emptyListMessage;
            NoResultsMessage = currentStyle.noResultsMessage;
        }

        private void ApplyLayerConfigurations()
        {
            var style = currentStyle;

            GlobalBackgroundConfig = SafeGetConfig(style.globalBackground?. backgroundConfig);
            HeaderBackgroundConfig = SafeGetConfig(style. header?.backgroundConfig);
            ListBackgroundConfig = SafeGetConfig(style.list?.backgroundConfig);
            PaginationBackgroundConfig = SafeGetConfig(style.pagination?.backgroundConfig);
            FooterBackgroundConfig = SafeGetConfig(style.footerButtons?.backgroundConfig);
            SelectedElementConfig = SafeGetConfig(style.element?.selectedBackgroundConfig);
            UnselectedElementConfig = SafeGetConfig(style.element?.unselectedBackgroundConfig);
        }

        private LayerConfiguration SafeGetConfig(LayerConfiguration config)
        {
            return config ??  new LayerConfiguration(0);
        }

        public void InitializeDefaultStyle()
        {
            currentStyle = new CustomListStyle();
            currentStyle.SetDefaultStyleValues();
            ApplyStyle();
        }
    }
}