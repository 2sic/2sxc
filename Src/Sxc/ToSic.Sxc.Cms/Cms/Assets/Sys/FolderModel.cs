namespace ToSic.Sxc.Cms.Assets.Sys;

[PrivateApi("Still tweaking details and naming v19.0x")]
public record FolderModel: ModelFromEntity, IFolderModelSync, IFolderModel
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
        ??= Entity.Children(field: nameof(Folder)).FirstOrDefault()?.ToModel<FolderModel>(options: new() { TypeName = ToModelOptions.TypeNameAny })!;

    [field: AllowNull, MaybeNull]
    public IEnumerable<IFolderModel> Folders => field 
        ??= Entity.Children(field: nameof(Folders)).ToModels<FolderModel>();

    [field: AllowNull, MaybeNull]
    public IEnumerable<IFileModel> Files => field
        ??= Entity.Children(field: nameof(Files)).ToModels<FileModel>()!;

    public string? Url => GetThis<string>(null);
    public DateTime Created => Entity.Created;
    public DateTime Modified => Entity.Modified;
}