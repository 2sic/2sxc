using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Security.Permissions;

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

        internal AdamItemDto Create(IFile original, bool usePortalRoot, AdamState state)
        {
            var item = new AdamItemDto(false, original.Id, original.FolderId, original.FullName, original.Size,
                original.Created, original.Modified)
            {
                Path = original.Path,
                AllowEdit = usePortalRoot
                    ? _security.CanEditFolder(original.FolderId)
                    : !state.Security.UserIsRestricted || state.Security.FieldPermissionOk(GrantSets.WriteSomething)
            };
            // (original.StorageLocation == 0) ? original.Path : FileLinkClickController.Instance.GetFileLinkClick(original);
            return item;
        }


        internal AdamItemDto Create(IFolder folder, bool usePortalRoot, AdamState state)
        {
            var item = new AdamItemDto(true, folder.Id, folder.ParentId, folder.Name, 0, folder.Created,
                folder.Modified)
            {
                Path = folder.Path,
                AllowEdit = usePortalRoot
                    ? _security.CanEditFolder(folder.Id)
                    : !state.Security.UserIsRestricted || state.Security.FieldPermissionOk(GrantSets.WriteSomething)
            };
            return item;
        }

    }
}
