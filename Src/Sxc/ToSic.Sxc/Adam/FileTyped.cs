using Custom.Data;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Apps.Assets.Internal;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Adam;

/// <summary>
/// A File Entity for typed use.
/// It defines the schema for a file as returned by the <see cref="DataSources.AppAssets"/> DataSource.
/// </summary>
/// <remarks>
/// * Introduced (BETA) in v19.00 for the <see cref="DataSources.AppAssets"/> DataSource.
/// * Not to be seen as final, since we may rename this type when we also
/// * This is similar to the <see cref="IFile"/> but still a bit different. For example, it has a <see cref="Folder"/> property which is different from the <see cref="ToSic.Eav.Apps.Assets.IFile.Folder"/> property.
/// </remarks>
[PublicApi]
public class FileTyped: CustomItem, IFileEntity
{
    /// <summary>
    /// The ID of this asset (file/folder).
    /// 
    /// In the case of App files/folders, these IDs will usually be negative, to indicate that they are not stable and can change at any time.
    /// </summary>
    public new int Id => base.Id;

    /// <summary>
    /// An empty Guid, since files/folders don't have a Guid.
    /// </summary>
    public new Guid Guid => base.Guid;

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
    public new FolderTyped Folder => _item.Child<FolderTyped>(nameof(Folder));

    /// <inheritdoc />
    public int Size => _item.Int(nameof(Size));

    public ISizeInfo SizeInfo => field ??= new SizeInfo(Size);

    /// <inheritdoc cref="IFileEntity.Url" />
    /// <remarks>
    /// It hides the base method <see cref="ITypedItem.Url"/>.
    /// </remarks>
    public new string Url => _item.String(nameof(Url));

    /// <inheritdoc />
    public DateTime Created => _item.DateTime(nameof(Created));
    /// <inheritdoc />
    public DateTime Modified => _item.DateTime(nameof(Modified));

}