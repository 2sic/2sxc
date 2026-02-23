using ToSic.Eav.Apps.Assets;
using ToSic.Sxc.Cms.Assets.Sys;

namespace ToSic.Sxc.Cms.Assets;

/// <summary>
/// BETA: A File Model which describes a file as returned by the <see cref="DataSources.AppAssets"/> DataSource.
/// </summary>
/// <remarks>
/// History
/// 
/// * Introduced (BETA) in v19.00 for the <see cref="DataSources.AppAssets"/> DataSource.
/// * Not to be seen as final, since we may rename this type when we also
/// * This is similar to the <see cref="Adam.IFile"/> but still a bit different.
/// For example, it has a <see cref="Folder"/> property which is different from the <see cref="IFile.Folder"/> property.
/// </remarks>
[ModelSpecs(Use = typeof(FileModelOfEntity))]
[InternalApi_DoNotUse_MayChangeWithoutNotice("Still tweaking details and naming v19.0x")]
public interface IFileModel: IModelFromData
{
    /// <inheritdoc cref="IFileModelSync.Name" />
    string? Name { get; }

    /// <inheritdoc cref="IFileModelSync.Extension" />
    string? Extension { get; }

    /// <inheritdoc cref="IFileModelSync.FullName" />
    string? FullName { get; }

    /// <inheritdoc cref="IFileModelSync.Path" />
    string? Path { get; }

    /// <summary>
    /// Reference to the folder this file is in.
    /// Returns `null` on the root folder.
    /// </summary>
    IFolderModel Folder { get; }

    /// <inheritdoc cref="IFileModelSync.Size" />
    int Size { get; }

    ISizeInfo SizeInfo { get; }

    /// <inheritdoc cref="IFileModelSync.Url" />
    string? Url { get; }

    /// <inheritdoc cref="IFileModelSync.Created" />
    DateTime Created { get; }

    /// <inheritdoc cref="IFileModelSync.Modified" />
    DateTime Modified { get; }
}