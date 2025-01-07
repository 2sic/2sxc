using Custom.Data;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Internal;

namespace ToSic.Sxc.Models;

/// <summary>
/// A Folder Entity for typed use.
/// It defines the schema for a folder as returned by the <see cref="DataSources.AppAssets"/> DataSource.
/// </summary>
/// <remarks>
/// * Introduced (BETA) in v19.00 for the <see cref="DataSources.AppAssets"/> DataSource.
/// * Not to be seen as final, since we may rename this type when we also
/// * This is similar to the <see cref="IFolder"/> but still a bit different. For example, it has a <see cref="Folder"/> property.
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("Still tweaking details and naming v19.0x")]
public class FolderInfo: CustomData, IFolderEntity
{
    ///// <inheritdoc cref="FileTyped.Id"/>
    //public int Id => ((ITypedItem)this).Id;

    ///// <inheritdoc cref="FileTyped.Guid"/>
    //public Guid Guid => ((ITypedItem)this).Guid;

    /// <inheritdoc />
    public string Name => _item.String(nameof(Name));
    /// <inheritdoc />
    public string FullName => _item.String(nameof(FullName));
    /// <inheritdoc />
    public string Path => _item.String(nameof(Path));

    /// <summary>
    /// Reference to the parent folder.
    /// Returns `null` on the root folder.
    /// </summary>
    public FolderInfo Folder => _item.Child<FolderInfo>(nameof(Folder));

    /// <summary>
    /// All sub folders in this folder.
    /// </summary>
    public IEnumerable<FolderInfo> Folders => _item.Children<FolderInfo>(nameof(Folders));

    /// <summary>
    /// All files in this folder.
    /// </summary>
    public IEnumerable<FileInfo> Files => _item.Children<FileInfo>(nameof(Files));

    /// <inheritdoc cref="IFileEntity.Url" />
    public string Url => _item.String(nameof(Url));

    /// <inheritdoc />
    public DateTime Created => _item.DateTime(nameof(Created));
    /// <inheritdoc />
    public DateTime Modified => _item.DateTime(nameof(Modified));
}