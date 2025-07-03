using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Apps.Assets.Sys;
using ToSic.Sxc.Data.Models;

namespace ToSic.Sxc.Cms.Assets.Sys;

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


    public string? Name => GetThis<string>(null);

    public string? Extension => GetThis<string>(null);

    public string? FullName => GetThis<string>(null);

    public string? Path => GetThis<string>(null);

    [field: AllowNull, MaybeNull]
    public IFolderModel Folder => field ??= As<FolderModelOfEntity>(_entity.Children(field: nameof(Folder)).FirstOrDefault())!;

    public int Size => GetThis(0);

    [field: AllowNull, MaybeNull]
    public ISizeInfo SizeInfo => field ??= new SizeInfo(Size);

    public string? Url => GetThis<string>(null);

    public DateTime Created => _entity.Created;

    public DateTime Modified => _entity.Modified;

}