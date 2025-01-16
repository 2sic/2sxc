using ToSic.Eav.Apps.Assets;
using ToSic.Sxc.Data.Model;
using ToSic.Sxc.Models.Internal;

namespace ToSic.Sxc.Models;

[DataModelConversion(Map = [
    typeof(DataModelFrom<IEntity, IFileModel, FileModelOfEntity>),
])]
[InternalApi_DoNotUse_MayChangeWithoutNotice("Still tweaking details and naming v19.0x")]
public interface IFileModel: IDataModel
{
    /// <inheritdoc cref="IFileModelSync.Name" />
    string Name { get; }

    /// <inheritdoc cref="IFileModelSync.Extension" />
    string Extension { get; }

    /// <inheritdoc cref="IFileModelSync.FullName" />
    string FullName { get; }

    /// <inheritdoc cref="IFileModelSync.Path" />
    string Path { get; }

    /// <summary>
    /// Reference to the folder this file is in.
    /// Returns `null` on the root folder.
    /// </summary>
    IFolderModel Folder { get; }

    /// <inheritdoc cref="IFileModelSync.Size" />
    int Size { get; }

    ISizeInfo SizeInfo { get; }

    /// <inheritdoc cref="IFileModelSync.Url" />
    string Url { get; }

    /// <inheritdoc cref="IFileModelSync.Created" />
    DateTime Created { get; }

    /// <inheritdoc cref="IFileModelSync.Modified" />
    DateTime Modified { get; }
}