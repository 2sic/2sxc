using System.Text.Json.Serialization;
using ToSic.Eav.Metadata;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam.FileSystem.Internal;
using ToSic.Sxc.Adam.Manager.Internal;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Adam.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class Folder<TFolderId, TFileId>(AdamManager adamManager)
    : Eav.Apps.Assets.Internal.Folder<TFolderId, TFileId>, IFolder
{
    protected AdamManager AdamManager { get; } = adamManager;
    protected IAdamFileSystem AdamFs { get; } = adamManager.AdamFs;

    /// <inheritdoc />
    [JsonIgnore]
    public ITypedMetadata Metadata => field ??= AdamManager.CreateMetadata(CmsMetadata.FolderPrefix + SysId, Name);

    IMetadataOf IHasMetadata.Metadata => (Metadata as IHasMetadata)?.Metadata;

    /// <inheritdoc />
    [JsonIgnore]
    public bool HasMetadata => (Metadata as IHasMetadata)?.Metadata.Any() ?? false;



    /// <inheritdoc />
    public string Url { get; set; }

    /// <inheritdoc />
    public string Type => Classification.Folder;


    /// <inheritdoc />
    public override bool HasChildren
        => _hasChildren ??= AdamFs.GetFiles(this).Any()
                            || AdamFs.GetFolders(this).Any();
    private bool? _hasChildren;



    /// <inheritdoc />
    public IEnumerable<IFolder> Folders => _folders.Get(() =>
    {
        var folders = AdamFs.GetFolders(this);
        folders?.ForEach(f => ((Folder<TFolderId, TFileId>)f).Field = Field);
        return folders;
    });
    private readonly GetOnce<IEnumerable<IFolder>> _folders = new();


    /// <inheritdoc/>
    public IEnumerable<IFile> Files => _files.Get(() =>
    {
        var files = AdamFs.GetFiles(this);
        files?.ForEach(f => ((File<TFolderId, TFileId>)f).Field = Field);
        return files;
    });
    private readonly GetOnce<IEnumerable<IFile>> _files = new();

    [PrivateApi]
    public IField Field { get; set; }
}