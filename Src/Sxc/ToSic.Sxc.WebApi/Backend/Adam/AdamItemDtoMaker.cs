using ToSic.Eav.Metadata;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Adam.Manager.Internal;
using ToSic.Sxc.Adam.Security.Internal;
using ToSic.Sxc.Adam.Work.Internal;
using ToSic.Sxc.Data;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend.Adam;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamItemDtoMaker<TFolderId, TFileId>(AdamItemDtoMaker<TFolderId, TFileId>.MyServices services)
    : ServiceWithSetup<AdamItemDtoMakerOptions>("Adm"), IAdamItemDtoMaker
{
    #region Constructor / DI

    public class MyServices(IAdamSecurityCheckService security)
    {
        public IAdamSecurityCheckService Security { get; } = security;
    }

    private readonly IAdamSecurityCheckService _security = services.Security;
    public AdamContext AdamContext => field ??= Options.AdamContext;

    #endregion

    private const string ThumbnailPattern = "{0}?w=120&h=120&mode=crop&urlSource=backend";
    private const string PreviewPattern = "{0}?w=800&h=800&mode=max&urlSource=backend";

    public IEnumerable<AdamItemDto> Convert(AdamFolderFileSet set)
    {
        // This will contain the list of items
        var list = new List<AdamItemDto>();

        if (set.Root != null)
        {
            var currentFolderDto = Create(set.Root);
            currentFolderDto.Name = ".";
            list.Insert(0, currentFolderDto);
        }

        var folders = set.Folders
            .Select(Create)
            .ToListOpt();
        list.AddRange(folders);

        var files = set.Files
            .Select(Create)
            .ToListOpt();
        list.AddRange(files);
        return list;
    }

    public virtual AdamItemDto Create(IFile file)
    {
        var url = file.Url;
        // Cast to typed, to access the SysId and ParentSysId
        var fileAsTyped = (File<TFolderId, TFileId>)file;
        var item = new AdamItemDto<TFolderId, TFileId>(
            false,
            fileAsTyped.SysId,
            fileAsTyped.ParentSysId,
            file.FullName, file.Size,
            file.Created, file.Modified)
        {
            Path = file.Path,
            ThumbnailUrl = string.Format(ThumbnailPattern, url),
            PreviewUrl = string.Format(PreviewPattern, url),
            Url = url,
            ReferenceId = ((IHasMetadata)file).Metadata.Target.KeyString,
            AllowEdit = CanEditFolder(file),
            Metadata = GetMetadataOf(file.Metadata),
            Type = Classification.TypeName(file.Extension),
        };
        return item;
    }


    public virtual AdamItemDto Create(IFolder folder)
    {
        // Cast to typed, to access the SysId and ParentSysId
        var folderAsTyped = (Folder<TFolderId, TFileId>)folder;
        var item = new AdamItemDto<TFolderId, TFolderId>(
            true,
            folderAsTyped.SysId,
            folderAsTyped.ParentSysId,
            folder.Name,
            0,
            folder.Created,
            folder.Modified)
        {
            Path = folder.Path,
            AllowEdit = CanEditFolder(folder),
            ReferenceId = ((IHasMetadata)folder).Metadata.Target.KeyString,
            //MetadataId = (int)folder.Metadata.EntityId,
            Metadata = GetMetadataOf(folder.Metadata),
        };
        return item;
    }

    private IEnumerable<MetadataOfDto> GetMetadataOf(ITypedMetadata md)
    {
        if (md == null) return null;

        var result = ((IHasMetadata)md).Metadata
            .Select(m => new MetadataOfDto
            {
                Id = m.EntityId,
                Guid = m.EntityGuid,
                Type = new(m)
            })
            .ToArray();
        return result.Any() ? result : null;
    }

    private bool CanEditFolder(Eav.Apps.Assets.IAsset original)
        => AdamContext.UseSiteRoot
            ? _security.CanEditFolder(original)
            : ContextAllowsEdit;

    /// <summary>
    /// Do this check once only, as the result will never change during one lifecycle
    /// </summary>
    private bool ContextAllowsEdit
        => _contextAllowsEdit ??= !AdamContext.Security.UserIsRestricted || AdamContext.Security.FieldPermissionOk(GrantSets.WriteSomething);
    private bool? _contextAllowsEdit;


}