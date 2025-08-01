using System.Text.Json.Serialization;
using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Adam.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class FileDynamic<TFolderId, TFileId>(AdamManager adamManager): File<TFolderId, TFileId>(adamManager)
#if NETFRAMEWORK
#pragma warning disable 618
    , ToSic.SexyContent.Adam.AdamFile
#pragma warning restore 618
#endif

{
    public string FileName => FullName;

    public DateTime CreatedOnDate => Created;

    public int FileId => SysId as int? ?? 0;

    [JsonIgnore]
    public new object Metadata => base.Metadata;

    public static FileDynamic<TFolderId, TFileId> Create(AdamManager adamManager, File<TFolderId, TFileId> typed)
        => new(adamManager)
        {
            FullName = typed.FullName,
            Extension = typed.Extension,
            Size = typed.Size,
            SysId = typed.SysId,
            Folder = typed.Folder,
            ParentSysId = typed.ParentSysId,

            Path = typed.Path,

            Created = typed.Created,
            Modified = typed.Modified,
            Name = typed.Name,
            Url = typed.Url,
            PhysicalPath = typed.PhysicalPath,
        };
}
