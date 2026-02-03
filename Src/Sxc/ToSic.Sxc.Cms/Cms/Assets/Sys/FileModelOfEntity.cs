using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Apps.Assets.Sys;

namespace ToSic.Sxc.Cms.Assets.Sys;

[PrivateApi("Still tweaking details and naming v19.0x")]
internal record FileModelOfEntity: ModelOfEntityCore, IFileModelSync, IFileModel
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
    public IFolderModel Folder => field
        ??= Entity.Children(field: nameof(Folder)).FirstOrDefault()?.As<FolderModelOfEntity>(skipTypeCheck: true)!;

    public int Size => GetThis(0);

    [field: AllowNull, MaybeNull]
    public ISizeInfo SizeInfo => field ??= new SizeInfo(Size);

    public string? Url => GetThis<string>(null);

    public DateTime Created => Entity.Created;

    public DateTime Modified => Entity.Modified;

}