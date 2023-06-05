using System.Collections.Generic;
using ToSic.Sxc.Adam;

namespace ToSic.Sxc.Data
{
    public partial interface ITypedItem
    {
        /// <summary>
        /// Get the ADAM (Automatic Digital Asset Manager) for this field.
        /// This is a folder which contains all the files and possibly folders which are uploaded on exactly this field.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The Folder object</returns>
        /// <remarks>Added in 16.02</remarks>
        IFolderTyped Folder(string name);

        ///// <summary>
        ///// All the sub-folders of this field, if any.
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns>A list of folders, or an empty list (TODO: VERIFY) if no subfolders</returns>
        ///// <remarks>Added in 16.02</remarks>
        //IEnumerable<IFolderTyped> Folders(string name);

        ///// <summary>
        ///// All the files in the main folder of this field, if any.
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns>A list of files, or an empty list (TODO: VERIFY) if no files</returns>
        ///// <remarks>Added in 16.02</remarks>
        //IEnumerable<IFileTyped> Files(string name);
    }
}
