using ToSic.Eav.Metadata;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Adam;

namespace ToSic.Sxc.WebApi.Adam
{
    public class AdamItemDtoMaker<TFolderId, TFileId>
    {
        #region Constructor / DI

        public class Dependencies
        {
            public AdamSecurityChecksBase Security { get; }
            public Dependencies(AdamSecurityChecksBase security)
            {
                Security = security;
            }
        }

        public AdamItemDtoMaker(Dependencies dependencies)
        {
            _security = dependencies.Security;
        }

        public AdamItemDtoMaker<TFolderId, TFileId> Init(AdamContext adamContext)
        {
            AdamContext = adamContext;
            return this;
        }

        private readonly AdamSecurityChecksBase _security;
        public AdamContext AdamContext;

        #endregion

        private const string ThumbnailPattern = "{0}?w=120&h=120&mode=crop&urlSource=backend";
        private const string PreviewPattern = "{0}?w=800&h=800&mode=max&urlSource=backend";

        public virtual AdamItemDto Create(File<TFolderId, TFileId> original)
        {
            var url = original.Url;
            var item = new AdamItemDto<TFolderId, TFileId>(false, original.SysId, original.ParentSysId, original.FullName, original.Size,
                original.Created, original.Modified)
            {
                Path = original.Path,
                ThumbnailUrl = string.Format(ThumbnailPattern, url),
                PreviewUrl = string.Format(PreviewPattern, url),
                Url = url,
                ReferenceId = (original as IHasMetadata).Metadata.Target.KeyString,
                AllowEdit = CanEditFolder(original)
            };
            // (original.StorageLocation == 0) ? original.Path : FileLinkClickController.Instance.GetFileLinkClick(original);
            return item;
        }


        public virtual AdamItemDto Create(Folder<TFolderId, TFileId> folder)
        {
            var item = new AdamItemDto<TFolderId, TFolderId>(true, folder.SysId, folder.ParentSysId, folder.Name, 0, folder.Created,
                folder.Modified)
            {
                Path = folder.Path,
                AllowEdit = CanEditFolder(folder),
                ReferenceId = (folder as IHasMetadata).Metadata.Target.KeyString,
            };
            return item;
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
            => _contextAllowsEdit 
               ?? (_contextAllowsEdit = !AdamContext.Security.UserIsRestricted || AdamContext.Security.FieldPermissionOk(GrantSets.WriteSomething)).Value;
        private bool? _contextAllowsEdit;


    }
}
