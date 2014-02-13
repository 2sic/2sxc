using System;

namespace ToSic.SexyContent
{
    public partial class ContentGroupItem
    {
        /// <summary>
        /// Returns the Type as enum
        /// </summary>
        public ContentGroupItemType ItemType
        {
            get {
                return (ContentGroupItemType)Enum.Parse(typeof(ContentGroupItemType), Type);
            }
        }
    }

    /// <summary>
    /// The ContentGroupItemType (Type) enum
    /// </summary>
    public enum ContentGroupItemType
    {
        Content, Presentation, ListContent, ListPresentation
    }
}