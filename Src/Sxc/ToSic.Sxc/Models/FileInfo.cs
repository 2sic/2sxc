using Custom.Data;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Apps.Assets.Internal;
using ToSic.Sxc.Adam.Internal;

namespace ToSic.Sxc.Models;

/// <summary>
/// A File Entity for typed use.
/// It defines the schema for a file as returned by the <see cref="DataSources.AppAssets"/> DataSource.
/// </summary>
/// <remarks>
/// * Introduced (BETA) in v19.00 for the <see cref="DataSources.AppAssets"/> DataSource.
/// * Not to be seen as final, since we may rename this type when we also
/// * This is similar to the <see cref="Adam.IFile"/> but still a bit different. For example, it has a <see cref="Folder"/> property which is different from the <see cref="ToSic.Eav.Apps.Assets.IFile.Folder"/> property.
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("Still tweaking details and naming v19.0x")]
public class FileInfo: CustomData, IFileEntity
{
    ///// <summary>
    ///// The ID of this asset (file/folder).
    ///// 
    ///// In the case of App files/folders, these IDs will usually be negative, to indicate that they are not stable and can change at any time.
    ///// </summary>
    //public int Id => ((ITypedItem)this).Id;

    ///// <summary>
    ///// An empty Guid, since files/folders don't have a Guid.
    ///// </summary>
    //public Guid Guid => ((ITypedItem)this).Guid;

    /// <inheritdoc />
    public string Name => _item.String(nameof(Name));
    /// <inheritdoc />
    public string Extension => _item.String(nameof(Extension));
    /// <inheritdoc />
    public string FullName => _item.String(nameof(FullName));
    /// <inheritdoc />
    public string Path => _item.String(nameof(Path));

    /// <summary>
    /// Reference to the folder this file is in.
    /// Returns `null` on the root folder.
    /// </summary>
    public FolderInfo Folder => _item.Child<FolderInfo>(nameof(Folder));

    /// <inheritdoc />
    public int Size => _item.Int(nameof(Size));

    public ISizeInfo SizeInfo => field ??= new SizeInfo(Size);

    /// <inheritdoc cref="IFileEntity.Url" />
    public string Url => _item.String(nameof(Url));

    /// <inheritdoc />
    public DateTime Created => _item.DateTime(nameof(Created));
    /// <inheritdoc />
    public DateTime Modified => _item.DateTime(nameof(Modified));

}