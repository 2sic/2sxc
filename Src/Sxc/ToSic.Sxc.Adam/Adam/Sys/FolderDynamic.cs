using System.Text.Json.Serialization;
using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Cms.Sys;

namespace ToSic.Sxc.Adam.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class FolderDynamic<TFolderId, TFileId>(AdamManager adamManager): Folder<TFolderId, TFileId>(adamManager)
{

    [JsonIgnore]
    [field: AllowNull, MaybeNull]
    public new object Metadata => field ??= AdamManager.CreateMetadataDynamic($"{CmsMetadata.FolderPrefix}{SysId}", Name);

    /// <summary>
    /// Create a dynamic folder from a typed folder.
    /// </summary>
    public static FolderDynamic<TFolderId, TFileId> Create(AdamManager adamManager, Folder<TFolderId, TFileId> typed)
        => new(adamManager)
        {
            Path = typed.Path,
            SysId = typed.SysId,

            ParentSysId = typed.ParentSysId,

            Name = typed.Name,
            Created = typed.Created,
            Modified = typed.Modified,
            Url = typed.Url,
            PhysicalPath = typed.PhysicalPath,
        };
}
