using ToSic.Eav.Metadata;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Dto.Metadata;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Adam.Work.Internal;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Backend.Adam;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamItemDtoMaker<TFolderId, TFileId>(AdamItemDtoMaker<TFolderId, TFileId>.MyServices services)
    : ServiceWithOptionsBase<AdamItemDtoMaker<TFolderId, TFileId>, AdamItemDtoMakerOptions>("Adm", services.SelfGenerator)
{
    #region Constructor / DI

    public class MyServices(IAdamSecurityCheckService security, Generator<AdamItemDtoMaker<TFolderId, TFileId>> selfGenerator)
    {
        public IAdamSecurityCheckService Security { get; } = security;
        public Generator<AdamItemDtoMaker<TFolderId, TFileId>> SelfGenerator { get; } = selfGenerator;
    }

    public AdamItemDtoMaker<TFolderId, TFileId> Init(AdamContext adamContext)
    {
        AdamContext = adamContext;
        return this;
    }

    private readonly IAdamSecurityCheckService _security = services.Security;
    public AdamContext AdamContext
    {
        get => field ??= Options.AdamContext;
        private set;
    }

    #endregion

    private const string ThumbnailPattern = "{0}?w=120&h=120&mode=crop&urlSource=backend";
    private const string PreviewPattern = "{0}?w=800&h=800&mode=max&urlSource=backend";

    public IEnumerable<AdamItemDto> Convert(AdamFolderFileSet<TFolderId, TFileId> set)
    {
        // This will contain the list of items
        var list = new List<AdamItemDto>();

        if (set.Root != null)
        {
            var currentFolderDto = Create(set.Root);
            currentFolderDto.Name = ".";
            list.Insert(0, currentFolderDto);
        }

        var folders = set.Folders.Select(Create).ToList();
        list.AddRange(folders);

        var files = set.Files.Select(Create).ToList();
        list.AddRange(files);
        return list;
    }

    public virtual AdamItemDto Create(File<TFolderId, TFileId> file)
    {
        var url = file.Url;
        var item = new AdamItemDto<TFolderId, TFileId>(false, file.SysId, file.ParentSysId,
            file.FullName, file.Size,
            file.Created, file.Modified)
        {
            Path = file.Path,
            ThumbnailUrl = string.Format(ThumbnailPattern, url),
            PreviewUrl = string.Format(PreviewPattern, url),
            Url = url,
            ReferenceId = (file as IHasMetadata).Metadata.Target.KeyString,
            AllowEdit = CanEditFolder(file),
            Metadata = GetMetadataOf(file.Metadata),
            Type = Classification.TypeName(file.Extension),
        };
        return item;
    }


    public virtual AdamItemDto Create(Folder<TFolderId, TFileId> folder)
    {
        var item = new AdamItemDto<TFolderId, TFolderId>(true, folder.SysId, folder.ParentSysId, folder.Name, 0,
            folder.Created,
            folder.Modified)
        {
            Path = folder.Path,
            AllowEdit = CanEditFolder(folder),
            ReferenceId = (folder as IHasMetadata).Metadata.Target.KeyString,
            //MetadataId = (int)folder.Metadata.EntityId,
            Metadata = GetMetadataOf(folder.Metadata),
        };
        return item;
    }

    private IEnumerable<MetadataOfDto> GetMetadataOf(IMetadata md)
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
    {
        return AdamContext.UseSiteRoot
            ? _security.CanEditFolder(original)
            : ContextAllowsEdit;
    }

    /// <summary>
    /// Do this check once only, as the result will never change during one lifecycle
    /// </summary>
    private bool ContextAllowsEdit
        => _contextAllowsEdit ??= !AdamContext.Security.UserIsRestricted || AdamContext.Security.FieldPermissionOk(GrantSets.WriteSomething);
    private bool? _contextAllowsEdit;


}