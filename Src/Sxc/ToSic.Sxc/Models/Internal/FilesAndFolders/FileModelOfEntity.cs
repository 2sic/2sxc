using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Apps.Assets.Internal;
using ToSic.Sxc.Data.Model;

namespace ToSic.Sxc.Models.Internal;

/// <summary>
/// A File Model which describes a file as returned by the <see cref="DataSources.AppAssets"/> DataSource.
/// </summary>
/// <remarks>
/// History
/// 
/// * Introduced (BETA) in v19.00 for the <see cref="DataSources.AppAssets"/> DataSource.
/// * Not to be seen as final, since we may rename this type when we also
/// * This is similar to the <see cref="Adam.IFile"/> but still a bit different. For example, it has a <see cref="Folder"/> property which is different from the <see cref="ToSic.Eav.Apps.Assets.IFile.Folder"/> property.
/// </remarks>
[PrivateApi("Still tweaking details and naming v19.0x")]
internal class FileModelOfEntity: DataModel, IFileModelSync, IFileModel
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

    public IFolderModel Folder => As<FolderModelOfEntity>(_entity.Children(field: nameof(Folder)).FirstOrDefault());

    public int Size => _entity.Get<int>(nameof(Size));

    public ISizeInfo SizeInfo => field ??= new SizeInfo(Size);

    public string Url => _entity.Get<string>(nameof(Url));

    public DateTime Created => _entity.Get<DateTime>(nameof(Created));

    public DateTime Modified => _entity.Get<DateTime>(nameof(Modified));

}