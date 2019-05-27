using ToSic.SexyContent;
using ToSic.Sxc.Adam;

namespace ToSic.Sxc.Interfaces
{
#pragma warning disable 618
    public interface IAppAndDataHelpers: SexyContent.IAppAndDataHelpers
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
    }
}