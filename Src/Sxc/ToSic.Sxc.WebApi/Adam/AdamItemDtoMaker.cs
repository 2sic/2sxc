using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Adam;

namespace ToSic.Sxc.WebApi.Adam
{
    public class AdamItemDtoMaker
    {
        #region Constructor / DI

         public AdamItemDtoMaker(SecurityChecksBase security)
        {
            _security = security;
        }

        private SecurityChecksBase _security;


        #endregion

        internal AdamItemDto Create<TFolderId, TFileId>(IFile<TFolderId, TFileId> original, AdamState state)
        {
            var item = new AdamItemDto<TFolderId, TFileId>(false, original.Id, original.FolderId, original.FullName, original.Size,
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


        internal AdamItemDto Create<TFolderId, TFileId>(IFolder<TFolderId, TFileId> folder, AdamState state)
        {
            var item = new AdamItemDto<TFolderId, TFolderId>(true, folder.Id, folder.ParentId, folder.Name, 0, folder.Created,
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
