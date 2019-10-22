using ToSic.SexyContent;
using ToSic.SexyContent.Interfaces;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Edit.InPageEditingSystem;

namespace ToSic.Sxc.Interfaces
{
#pragma warning disable 618
    public interface IAppAndDataHelpers: SexyContent.IAppAndDataHelpers, ISharedCodeBuilder
#pragma warning restore 618
    {

        #region AsAdam

        /// <summary>
        /// Provides an Adam instance for this item and field
        /// </summary>
        /// <param name="entity">The entity, often Content or similar</param>
        /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
        /// <returns>An Adam object for navigating the assets</returns>
        FolderOfField AsAdam(DynamicEntity entity, string fieldName);

        /// <summary>
        /// Provides an Adam instance for this item and field
        /// </summary>
        /// <param name="entity">The entity, often Content or similar</param>
        /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
        /// <returns>An Adam object for navigating the assets</returns>
        FolderOfField AsAdam(Eav.Interfaces.IEntity entity, string fieldName);

        #endregion

        #region Linking

        /// <summary>
        /// Link helper object to create the correct links
        /// </summary>
        ILinkHelper Link { get; }

        #endregion

        #region Edit

        /// <summary>
        /// Helper commands to enable in-page editing functionality
        /// Use it to check if edit is enabled, generate context-json infos and provide toolbar buttons
        /// </summary>
        IInPageEditingSystem Edit { get; }
        #endregion
    }
}