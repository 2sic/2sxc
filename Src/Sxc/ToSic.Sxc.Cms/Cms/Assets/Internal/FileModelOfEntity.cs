using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Apps.Assets.Internal;
using ToSic.Sxc.Data.Models;

namespace ToSic.Sxc.Cms.Assets.Internal;

[PrivateApi("Still tweaking details and naming v19.0x")]
internal class FileModelOfEntity: ModelFromEntity, IFileModelSync, IFileModel
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


    public string Name => _entity.Get<string>(nameof(Name));

    public string Extension => _entity.Get<string>(nameof(Extension));

    public string FullName => _entity.Get<string>(nameof(FullName));

    public string Path => _entity.Get<string>(nameof(Path));

    public IFolderModel Folder => field ??= As<FolderModelOfEntity>(_entity.Children(field: nameof(Folder)).FirstOrDefault());

    public int Size => _entity.Get<int>(nameof(Size));

    public ISizeInfo SizeInfo => field ??= new SizeInfo(Size);

    public string Url => _entity.Get<string>(nameof(Url));

    public DateTime Created => _entity.Get<DateTime>(nameof(Created));

    public DateTime Modified => _entity.Get<DateTime>(nameof(Modified));

}