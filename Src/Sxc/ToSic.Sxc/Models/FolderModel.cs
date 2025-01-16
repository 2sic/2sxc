using ToSic.Sxc.Adam;
using ToSic.Sxc.Models.Internal;

namespace ToSic.Sxc.Models;

/// <summary>
/// A Folder Model which describes a folder as returned by the <see cref="DataSources.AppAssets"/> DataSource.
/// </summary>
/// <remarks>
/// History
/// 
/// * Introduced (BETA) in v19.00 for the <see cref="DataSources.AppAssets"/> DataSource.
/// * Not to be seen as final, since we may rename this type when we also
/// * This is similar to the <see cref="IFolder"/> but still a bit different. For example, it has a <see cref="Folder"/> property.
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("Still tweaking details and naming v19.0x")]
public class FolderModel: DataModel, IFolderModel
{
    ///// <inheritdoc cref="FileTyped.Id"/>
    //public int Id => ((ITypedItem)this).Id;

    ///// <inheritdoc cref="FileTyped.Guid"/>
    //public Guid Guid => ((ITypedItem)this).Guid;

    /// <inheritdoc />
    public string Name => _entity.Get<string>(nameof(Name));
    /// <inheritdoc />
    public string FullName => _entity.Get<string>(nameof(FullName));
    /// <inheritdoc />
    public string Path => _entity.Get<string>(nameof(Path));

    /// <summary>
    /// Reference to the parent folder.
    /// Returns `null` on the root folder.
    /// </summary>
    public FolderModel Folder => As<FolderModel>(_entity.Children(field: nameof(Folder)).FirstOrDefault());

    /// <summary>
    /// All sub folders in this folder.
    /// </summary>
    public IEnumerable<FolderModel> Folders => AsList<FolderModel>(_entity.Children(field: nameof(Folders)));

    /// <summary>
    /// All files in this folder.
    /// </summary>
    public IEnumerable<FileModel> Files => AsList<FileModel>(_entity.Children(field: nameof(Files)));

    /// <inheritdoc cref="IFileModelSync.Url" />
    public string Url => _entity.Get<string>(nameof(Url));

    /// <inheritdoc />
    public DateTime Created => _entity.Get<DateTime>(nameof(Created));
    /// <inheritdoc />
    public DateTime Modified => _entity.Get<DateTime>(nameof(Modified));
}