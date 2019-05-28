//using ToSic.SexyContent.Interfaces;
//using ToSic.Sxc.Edit.InPageEditingSystem;

//namespace ToSic.Sxc.Interfaces
//{
//    /// <summary>
//    /// This interface is for any object which generates output from 2sxc data
//    /// It ensures that these objects have various helpers to
//    /// 1. access context information
//    /// 2. generate necessary outputs
//    /// </summary>
//    public interface IAppOutputGenerators: IAppAndDataHelpers, ISharedCodeBuilder
//    {
//        #region Linking

//        /// <summary>
//        /// Link helper object to create the correct links
//        /// </summary>
//        ILinkHelper Link { get; }

//        #endregion

//        #region Edit

//        /// <summary>
//        /// Helper commands to enable in-page editing functionality
//        /// Use it to check if edit is enabled, generate context-json infos and provide toolbar buttons
//        /// </summary>
//        IInPageEditingSystem Edit { get; }
//        #endregion

//    }
//}
