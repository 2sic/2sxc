using ToSic.Sxc.Data.Models;

namespace ToSic.Sxc.Cms.Assets.Internal;

[PrivateApi("Still tweaking details and naming v19.0x")]
internal class FolderModelOfEntity: ModelFromEntity, IFolderModelSync, IFolderModel
{
    ///// <inheritdoc cref="FileTyped.Id"/>
    //public int Id => ((ITypedItem)this).Id;

    ///// <inheritdoc cref="FileTyped.Guid"/>
    //public Guid Guid => ((ITypedItem)this).Guid;

    public string Name => _entity.Get<string>(nameof(Name));
    public string FullName => _entity.Get<string>(nameof(FullName));
    public string Path => _entity.Get<string>(nameof(Path));
    public IFolderModel Folder => field ??= As<FolderModelOfEntity>(_entity.Children(field: nameof(Folder)).FirstOrDefault());
    public IEnumerable<IFolderModel> Folders => field ??= AsList<FolderModelOfEntity>(_entity.Children(field: nameof(Folders)));
    public IEnumerable<IFileModel> Files => field ??= AsList<FileModelOfEntity>(_entity.Children(field: nameof(Files)));
    public string Url => _entity.Get<string>(nameof(Url));
    public DateTime Created => _entity.Get<DateTime>(nameof(Created));
    public DateTime Modified => _entity.Get<DateTime>(nameof(Modified));
}