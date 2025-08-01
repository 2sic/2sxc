using System.Text.Json.Serialization;
using ToSic.Sxc.Adam.Sys.Manager;

namespace ToSic.Sxc.Adam.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class FolderDynamic<TFolderId, TFileId>(AdamManager adamManager): Folder<TFolderId, TFileId>(adamManager)
{

    [JsonIgnore]
    public new object Metadata => base.Metadata;

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
