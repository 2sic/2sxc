using ToSic.Eav.Security.Permissions;

namespace ToSic.Sxc.WebApi.Adam
{
    public class AdamItemDtoMaker<TFolderId, TFileId>
    {
        #region Constructor / DI

         public AdamItemDtoMaker(SecurityChecksBase security)
        {
            _security = security;
        }

        private SecurityChecksBase _security;


        #endregion

        internal virtual AdamItemDto Create(Sxc.Adam.File<TFolderId, TFileId> original, AdamState state)
        {
            var item = new AdamItemDto<TFolderId, TFileId>(false, original.SysId, original.ParentSysId, original.FullName, original.Size,
                original.Created, original.Modified)
            {
                Path = original.Path,
                AllowEdit = state.UseTenantRoot
                    ? _security.CanEditFolder(original)
                    : !state.Security.UserIsRestricted || state.Security.FieldPermissionOk(GrantSets.WriteSomething)
            };
            // (original.StorageLocation == 0) ? original.Path : FileLinkClickController.Instance.GetFileLinkClick(original);
            return item;
        }


        internal virtual AdamItemDto Create(Sxc.Adam.Folder<TFolderId, TFileId> folder, AdamState state)
        {
            // todo: AdamId
            var item = new AdamItemDto<TFolderId, TFolderId>(true, folder.SysId, folder.ParentSysId, folder.Name, 0, folder.Created,
                folder.Modified)
            {
                Path = folder.Path,
                AllowEdit = state.UseTenantRoot
                    ? _security.CanEditFolder(folder)
                    : !state.Security.UserIsRestricted || state.Security.FieldPermissionOk(GrantSets.WriteSomething)
            };
            return item;
        }

    }
}
