//using System;

//namespace ToSic.Sxc.Interfaces
//{
//    /// <summary>
//    /// Basic interface which tells the system that this item has editing-information attached
//    /// </summary>
//    internal interface IHasEditingData
//    {
//        /// <summary>
//        /// The position in the list
//        /// </summary>
//        /// <remarks>
//        /// This has been in use since ca. 2sxc 2.0
//        /// </remarks>
//        int SortOrder { get; }

//        /// <summary>
//        /// The parent item which has the list containing this item.
//        /// </summary>
//        /// <remarks>
//        /// Important: as of now this is NOT the content-block guid, and shouldn't be because there is code checking for this to be empty
//        /// on content-blocks.
//        /// </remarks>
//        Guid? Parent { get; }

//        /// <summary>
//        /// The field which has the list containing this item.
//        /// </summary>
//        string Field { get; }
//    }
//}
