using ToSic.Sxc.Data.Model;
using ToSic.Sxc.Models.Internal;

namespace ToSic.Sxc.Models;

[DataModelConversion(Map = [
    typeof(DataModelFrom<IEntity, IFolderModel, FolderModelOfEntity>),
])]
[InternalApi_DoNotUse_MayChangeWithoutNotice("Still tweaking details and naming v19.0x")]
public interface IFolderModel: IDataModel
{
    /// <inheritdoc cref="IFolderModelSync.Name" />
    string Name { get; }

    /// <inheritdoc cref="IFolderModelSync.FullName" />
    string FullName { get; }

    /// <inheritdoc cref="IFolderModelSync.Path" />
    string Path { get; }

    /// <summary>
    /// Reference to the parent folder.
    /// Returns `null` on the root folder.
    /// </summary>
    IFolderModel Folder { get; }

    /// <summary>
    /// All sub folders in this folder.
    /// </summary>
    IEnumerable<IFolderModel> Folders { get; }

    /// <summary>
    /// All files in this folder.
    /// </summary>
    IEnumerable<IFileModel> Files { get; }

    /// <inheritdoc cref="IFileModelSync.Url" />
    string Url { get; }

    /// <inheritdoc cref="IFolderModelSync.Created" />
    DateTime Created { get; }

    /// <inheritdoc cref="IFolderModelSync.Modified" />
    DateTime Modified { get; }
}