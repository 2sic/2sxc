using System;

namespace ToSic.SexyContent.EAVExtensions
{
    /// <summary>
    /// Basic interface which tells the system that this item has editing-information attached
    /// </summary>
    interface IHasEditingData
    {
        int SortOrder { get; set; }
        DateTime ContentGroupItemModified { get; set; }
    }
}
