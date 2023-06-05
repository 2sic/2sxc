using System.Collections.Generic;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// An ADAM (Automatic Digital Asset Management) folder.
    /// This simple interface assumes that it uses int-IDs.
    /// </summary>
    [WorkInProgressApi("Still WIP v16.02")]
    public interface IFolderTyped: IFolder, IAssetTyped
    {
        /// <summary>
        /// Get the files in this folder
        /// </summary>
        /// <returns>A list of files in this folder as <see cref="IFile"/></returns>
        new IEnumerable<IFileTyped> Files { get; }

        /// <summary>
        /// Get the sub-folders in this folder
        /// </summary>
        /// <returns>A list of folders inside this folder - as <see cref="IFolder"/>.</returns>
        new IEnumerable<IFolderTyped> Folders { get; }
    }
}
