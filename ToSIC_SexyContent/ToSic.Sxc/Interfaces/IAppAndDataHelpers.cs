//using ToSic.Eav.Documentation;
//using ToSic.Sxc.Adam;

//namespace ToSic.Sxc.Interfaces
//{
//    /// <summary>
//    /// This is for razor-templates and Sxc WebAPIs - so that they have a consistent
//    /// API like AsAdam(...) or Link, Edit etc. 
//    /// </summary>
//    [PublicApi]
//#pragma warning disable 618
//    public interface IAppAndDataHelpers: SexyContent.IAppAndDataHelpers, ISharedCodeBuilder
//#pragma warning restore 618
//    {

//        //#region AsAdam

//        ///// <summary>
//        ///// Provides an Adam instance for this item and field
//        ///// </summary>
//        ///// <param name="entity">The entity, often Content or similar</param>
//        ///// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
//        ///// <returns>An Adam object for navigating the assets</returns>
//        //FolderOfField AsAdam(IDynamicEntity entity, string fieldName);

//        ///// <summary>
//        ///// Provides an Adam instance for this item and field
//        ///// </summary>
//        ///// <param name="entity">The entity, often Content or similar</param>
//        ///// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
//        ///// <returns>An Adam object for navigating the assets</returns>
//        //FolderOfField AsAdam(Eav.Interfaces.IEntity entity, string fieldName);

//        //#endregion

//        //#region Linking

//        ///// <summary>
//        ///// Link helper object to create the correct links
//        ///// </summary>
//        ///// <returns>
//        ///// A LinkHelper object.
//        ///// </returns>
//        //ILinkHelper Link { get; }

//        //#endregion

//        //#region Edit

//        ///// <summary>
//        ///// Helper commands to enable in-page editing functionality
//        ///// Use it to check if edit is enabled, generate context-json infos and provide toolbar buttons
//        ///// </summary>
//        ///// <returns>
//        ///// An InPageEditingSystem object.
//        ///// </returns>
//        //IInPageEditingSystem Edit { get; }
//        //#endregion


//    }
//}