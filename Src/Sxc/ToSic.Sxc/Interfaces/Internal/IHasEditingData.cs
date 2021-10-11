using System;

namespace ToSic.Sxc.Interfaces
{
    /// <summary>
    /// Basic interface which tells the system that this item has editing-information attached
    /// </summary>
    internal interface IHasEditingData
    {
        /// <summary>
        /// The position in the list
        /// </summary>
        /// <remarks>
        /// This has been in use since ca. 2sxc 2.0
        /// </remarks>
        int SortOrder { get; }

        /// <summary>
        /// The parent item which has the list containing this item.
        /// </summary>
        Guid? Parent { get; }

        /// <summary>
        /// The field which has the list containing this item.
        /// </summary>
        string Field { get; }
    }
}
