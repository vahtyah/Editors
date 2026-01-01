using System;
using UnityEngine;

namespace CustomInspector
{
    /// <summary>
    /// Attribute để nhóm các field trong Inspector với visual đẹp
    /// Usage: [CustomBoxGroup("groupId", "Label Text")]
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class CustomBoxGroupAttribute : PropertyAttribute
    {
        public string GroupID { get; private set; }
        public string Label { get; private set; }
        public BoxStyle Style { get; private set; }
        public bool ShowIcon { get; private set; }
        public string IconName { get; private set; }

        public CustomBoxGroupAttribute(string groupId, string label = "", BoxStyle style = BoxStyle.Dark, bool showIcon = false, string iconName = "")
        {
            GroupID = groupId;
            Label = string.IsNullOrEmpty(label) ? groupId : label;
            Style = style;
            ShowIcon = showIcon;
            IconName = iconName;
            order = 0;
        }

        public override int GetHashCode()
        {
            return GroupID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is CustomBoxGroupAttribute other)
            {
                return GroupID == other.GroupID;
            }
            return false;
        }
    }

    public enum BoxStyle
    {
        Dark,      // Style tối như trong hình
        Light,     // Style sáng
        Accent,    // Style với màu accent
        Custom     // Custom style
    }
}