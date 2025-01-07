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
public class FolderModel: DataModel, IFolderEntity
{
    ///// <inheritdoc cref="FileTyped.Id"/>
    //public int Id => ((ITypedItem)this).Id;

    ///// <inheritdoc cref="FileTyped.Guid"/>
    //public Guid Guid => ((ITypedItem)this).Guid;

    /// <inheritdoc />
    public string Name => _data.Get<string>(nameof(Name));
    /// <inheritdoc />
    public string FullName => _data.Get<string>(nameof(FullName));
    /// <inheritdoc />
    public string Path => _data.Get<string>(nameof(Path));

    /// <summary>
    /// Reference to the parent folder.
    /// Returns `null` on the root folder.
    /// </summary>
    //public FolderModel Folder => _item.Child<FolderModel>(nameof(Folder));

    public FolderModel Folder => As<FolderModel>(_data.Entity.Children(field: nameof(Folder)).FirstOrDefault());

    /// <summary>
    /// All sub folders in this folder.
    /// </summary>
    //public IEnumerable<FolderModel> Folders => _item.Children<FolderModel>(nameof(Folders));

    public IEnumerable<FolderModel> Folders => AsList<FolderModel>(_data.Entity.Children(field: nameof(Folders)));

    /// <summary>
    /// All files in this folder.
    /// </summary>
    //public IEnumerable<FileModel> Files => _item.Children<FileModel>(nameof(Files));
    public IEnumerable<FileModel> Files => AsList<FileModel>(_data.Entity.Children(field: nameof(Files)));

    /// <inheritdoc cref="IFileEntity.Url" />
    public string Url => _data.Get<string>(nameof(Url));

    /// <inheritdoc />
    public DateTime Created => _data.Get<DateTime>(nameof(Created));
    /// <inheritdoc />
    public DateTime Modified => _data.Get<DateTime>(nameof(Modified));
}