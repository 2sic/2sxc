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
        int SortOrder { get; set; }
        // CodeChange #2020-03-20#ContentGroupItemModified - Delete if no side-effects till June 2020
        //DateTime ContentGroupItemModified { get; set; }

        /// <summary>
        /// The parent item which has the list containing this item.
        /// </summary>
        /// <remarks>
        /// Experimental 2020-03-20 v10.27
        /// </remarks>
        Guid? Parent { get; set; }

        /// <summary>
        /// The field which has the list containing this item.
        /// </summary>
        /// <remarks>
        /// Experimental 2020-03-20 v10.27
        /// </remarks>
        string Fields { get; set; }
    }
}
