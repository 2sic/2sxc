namespace ToSic.Sxc.Cms.Assets.Sys;

[PrivateApi("Still tweaking details and naming v19.0x")]
internal record FolderModelOfEntity: ModelOfEntityCore, IFolderModelSync, IFolderModel
{
    ///// <inheritdoc cref="FileTyped.Id"/>
    //public int Id => ((ITypedItem)this).Id;

    ///// <inheritdoc cref="FileTyped.Guid"/>
    //public Guid Guid => ((ITypedItem)this).Guid;

    public string? Name => GetThis<string>(null);
    public string? FullName => GetThis<string>(null);
    public string? Path => GetThis<string>(null);

    [field: AllowNull, MaybeNull]
    public IFolderModel Folder => field
        ??= Entity.Children(field: nameof(Folder)).FirstOrDefault()?.As<FolderModelOfEntity>(skipTypeCheck: true)!;

    [field: AllowNull, MaybeNull]
    public IEnumerable<IFolderModel> Folders => field 
        ??= Entity.Children(field: nameof(Folders)).AsList<FolderModelOfEntity>();

    [field: AllowNull, MaybeNull]
    public IEnumerable<IFileModel> Files => field
        ??= Entity.Children(field: nameof(Files)).AsList<FileModelOfEntity>()!;

    public string? Url => GetThis<string>(null);
    public DateTime Created => Entity.Created;
    public DateTime Modified => Entity.Modified;
}