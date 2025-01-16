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
[PrivateApi("Still tweaking details and naming v19.0x")]
internal class FolderModelOfEntity: DataModel, IFolderModelSync, IFolderModel
{
    ///// <inheritdoc cref="FileTyped.Id"/>
    //public int Id => ((ITypedItem)this).Id;

    ///// <inheritdoc cref="FileTyped.Guid"/>
    //public Guid Guid => ((ITypedItem)this).Guid;

    public string Name => _entity.Get<string>(nameof(Name));
    public string FullName => _entity.Get<string>(nameof(FullName));
    public string Path => _entity.Get<string>(nameof(Path));
    public IFolderModel Folder => As<FolderModelOfEntity>(_entity.Children(field: nameof(Folder)).FirstOrDefault());
    public IEnumerable<IFolderModel> Folders => AsList<FolderModelOfEntity>(_entity.Children(field: nameof(Folders)));
    public IEnumerable<IFileModel> Files => AsList<FileModelOfEntity>(_entity.Children(field: nameof(Files)));
    public string Url => _entity.Get<string>(nameof(Url));
    public DateTime Created => _entity.Get<DateTime>(nameof(Created));
    public DateTime Modified => _entity.Get<DateTime>(nameof(Modified));
}