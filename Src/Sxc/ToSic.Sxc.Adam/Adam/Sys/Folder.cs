using System.Text.Json.Serialization;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Adam.Sys.FileSystem;
using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Cms.Sys;
using ToSic.Sxc.Data;
using ToSic.Sys.Performance;

namespace ToSic.Sxc.Adam.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class Folder<TFolderId, TFileId>(AdamManager adamManager)
    : Eav.Apps.Assets.Sys.Folder<TFolderId, TFileId>, IFolder
{
    protected AdamManager AdamManager { get; } = adamManager;
    protected IAdamFileSystem AdamFs { get; } = adamManager.AdamFs;

    /// <inheritdoc />
    [JsonIgnore]
    [field: AllowNull, MaybeNull]
    public ITypedMetadata Metadata => field
        ??= AdamManager.CreateMetadataTyped($"{CmsMetadata.FolderPrefix}{SysId}", Name);

    IMetadata IHasMetadata.Metadata => (Metadata as IHasMetadata)!.Metadata;

    /// <inheritdoc />
    [JsonIgnore]
    public bool HasMetadata => (Metadata as IHasMetadata)!.Metadata.Any();



    /// <inheritdoc />
    public string? Url { get; set; }

    /// <inheritdoc />
    public string Type => AssetTypes.Folder;


    /// <inheritdoc />
    public override bool HasChildren
        => _hasChildren ??= AdamFs.GetFiles(this).Any()
                            || AdamFs.GetFolders(this).Any();
    private bool? _hasChildren;



    /// <inheritdoc />
    public IEnumerable<IFolder> Folders => field
        ??= AdamFs
            .GetFolders(this)
            .Select(f =>
            {
                ((Folder<TFolderId, TFileId>)f).Field = Field;
                return f;
            })
            .ToListOpt();

    /// <inheritdoc/>
    public IEnumerable<IFile> Files => field
        ??= AdamFs
            .GetFiles(this)
            .Select(f =>
            {
                ((File<TFolderId, TFileId>)f).Field = Field;
                return f;
            })
            .ToListOpt();

    [PrivateApi]
    public IField? Field { get; set; }
}