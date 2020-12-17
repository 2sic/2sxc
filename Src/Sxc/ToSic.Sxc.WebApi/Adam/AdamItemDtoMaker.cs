using System.IO;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.Helpers;
using ToSic.Eav.WebApi.Dto;

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

        public AdamItemDtoMaker<TFolderId, TFileId> Init(AdamState state)
        {
            AdamState = state;
            return this;
        }

        private readonly AdamSecurityChecksBase _security;
        public AdamState AdamState;

        #endregion

        private const string ThumbnailPattern = "{0}?w=120&h=120&mode=crop";
        private const string PreviewPattern = "{0}?w=800&h=800&mode=max";

        public virtual AdamItemDto Create(Sxc.Adam.File<TFolderId, TFileId> original)
        {
            var url = Path.Combine(AdamBaseUrl, original.Path).Forwardslash();
            var item = new AdamItemDto<TFolderId, TFileId>(false, original.SysId, original.ParentSysId, original.FullName, original.Size,
                original.Created, original.Modified)
            {
                Path = original.Path,
                ThumbnailUrl = string.Format(ThumbnailPattern, url),
                PreviewUrl = string.Format(PreviewPattern, url),
                Url = url,
                AllowEdit = CanEditFolder(original)
            };
            // (original.StorageLocation == 0) ? original.Path : FileLinkClickController.Instance.GetFileLinkClick(original);
            return item;
        }


        public virtual AdamItemDto Create(Sxc.Adam.Folder<TFolderId, TFileId> folder)
        {
            var item = new AdamItemDto<TFolderId, TFolderId>(true, folder.SysId, folder.ParentSysId, folder.Name, 0, folder.Created,
                folder.Modified)
            {
                Path = folder.Path,
                AllowEdit = CanEditFolder(folder)
            };
            return item;
        }

        private string AdamBaseUrl => _adamBaseUrl ?? (_adamBaseUrl = AdamState.Context.Site.ContentPath);
        private string _adamBaseUrl;

        private bool CanEditFolder(Eav.Apps.Assets.IAsset original)
        {
            return AdamState.UseSiteRoot
                ? _security.CanEditFolder(original)
                : ContextAllowsEdit;
        }

        /// <summary>
        /// Do this check once only, as the result will never change during one lifecycle
        /// </summary>
        private bool ContextAllowsEdit 
            => _contextAllowsEdit 
               ?? (_contextAllowsEdit = !AdamState.Security.UserIsRestricted || AdamState.Security.FieldPermissionOk(GrantSets.WriteSomething)).Value;
        private bool? _contextAllowsEdit;


    }
}
