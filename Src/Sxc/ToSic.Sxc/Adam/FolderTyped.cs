using Custom.Data;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Adam;

/// <summary>
/// A Folder Entity for typed use.
/// It defines the schema for a folder as returned by the <see cref="DataSources.AppAssets"/> DataSource.
/// </summary>
/// <remarks>
/// * Introduced (BETA) in v19.00 for the <see cref="DataSources.AppAssets"/> DataSource.
/// * Not to be seen as final, since we may rename this type when we also
/// * This is similar to the <see cref="IFolder"/> but still a bit different. For example, it has a <see cref="Folder"/> property.
/// </remarks>
[PublicApi]
public class FolderTyped: CustomItem, IFolderEntity
{
    /// <inheritdoc cref="FileTyped.Id"/>
    public new int Id => base.Id;

    /// <inheritdoc cref="FileTyped.Guid"/>
    public new Guid Guid => base.Guid;

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
    public new FolderTyped Folder => _item.Child<FolderTyped>(nameof(Folder));

    /// <summary>
    /// All sub folders in this folder.
    /// </summary>
    public IEnumerable<FolderTyped> Folders => _item.Children<FolderTyped>(nameof(Folders));

    /// <summary>
    /// All files in this folder.
    /// </summary>
    public IEnumerable<FileTyped> Files => _item.Children<FileTyped>(nameof(Files));

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