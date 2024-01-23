using System.Text.Json.Serialization;
using ToSic.Eav.Metadata;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Adam.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class Folder<TFolderId, TFileId>(AdamManager<TFolderId, TFileId> adamManager)
    : Eav.Apps.Assets.Internal.Folder<TFolderId, TFileId>, IFolder
{
    protected AdamManager<TFolderId, TFileId> AdamManager { get; } = adamManager;

    /// <inheritdoc />
    [JsonIgnore]
    public IMetadata Metadata => _metadata ??= AdamManager.Create(CmsMetadata.FolderPrefix + SysId, Name);
    private IMetadata _metadata;

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
        => _hasChildren ??= AdamManager.AdamFs.GetFiles(this).Any()
                            || AdamManager.AdamFs.GetFolders(this).Any();
    private bool? _hasChildren;



    /// <inheritdoc />
    public IEnumerable<IFolder> Folders => _folders.Get(() =>
    {
        var folders = AdamManager.AdamFs.GetFolders(this);
        folders?.ForEach(f => f.Field = Field);
        return folders;
    });
    private readonly GetOnce<IEnumerable<IFolder>> _folders = new();


    /// <inheritdoc/>
    public IEnumerable<IFile> Files => _files.Get(() =>
    {
        var files = AdamManager.AdamFs.GetFiles(this);
        files?.ForEach(f => f.Field = Field);
        return files;
    });
    private readonly GetOnce<IEnumerable<IFile>> _files = new();

    [PrivateApi]
    public IField Field { get; set; }
}