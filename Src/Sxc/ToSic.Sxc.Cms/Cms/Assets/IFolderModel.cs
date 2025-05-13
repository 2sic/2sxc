using ToSic.Sxc.Adam;
using ToSic.Sxc.Cms.Assets.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Models;

namespace ToSic.Sxc.Cms.Assets;

/// <summary>
/// BETA: A Folder Model which describes a folder as returned by the <see cref="DataSources.AppAssets"/> DataSource.
/// </summary>
/// <remarks>
/// History
/// 
/// * Introduced (BETA) in v19.01 for the <see cref="DataSources.AppAssets"/> DataSource.
/// * Not to be seen as final, since we may rename this type when we also
/// * This is similar to the <see cref="IFolder"/> but still a bit different. For example, it has a <see cref="Folder"/> property.
/// </remarks>
[ModelCreation(Use = typeof(FolderModelOfEntity))]
[InternalApi_DoNotUse_MayChangeWithoutNotice("Still tweaking details and naming v19.0x")]
public interface IFolderModel: ICanWrapData
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