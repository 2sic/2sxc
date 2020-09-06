//using ToSic.Eav.Security.Permissions;
//using ToSic.Sxc.WebApi.Adam;

//namespace ToSic.Sxc.Adam.WebApi
//{
//    public class AdamItem: AdamItemDto
//    {
//        //internal AdamItem(IFileInfo original, bool usePortalRoot, AdamState state)
//        //    : base(false, original.FileId, original.FolderId, original.FileName, original.Size, 
//        //        original.CreatedOnDate, original.LastModifiedOnDate)
//        //{
//        //    //IsFolder = false;
//        //    //Id = original.FileId;
//        //    //ParentId = original.FolderId;
//        //    Path = (original.StorageLocation == 0) ? original.RelativePath : FileLinkClickController.Instance.GetFileLinkClick(original);
//        //    //Name = original.FileName;
//        //    //Size = original.Size;
//        //    //Type = "unknown"; // will be set from the outside
//        //    //Created = original.CreatedOnDate;
//        //    //Modified = original.LastModifiedOnDate;
//        //    AllowEdit = usePortalRoot ? DnnAdamSecurityChecks.CanEdit(original) : !state.Security.UserIsRestricted || state.Security.FieldPermissionOk(GrantSets.WriteSomething);
//        //}

//        //internal AdamItem(IFolderInfo original, bool usePortalRoot, AdamState state)
//        //    : base(true, original.FolderID, original.ParentID, original.DisplayName, 0, original.CreatedOnDate, original.LastModifiedOnDate)

//        //{
//        //    //IsFolder = true;
//        //    //Id = original.FolderID;
//        //    //ParentId = original.ParentID;
//        //    Path = original.DisplayPath;
//        //    //Name = original.DisplayName;
//        //    //Size = 0;
//        //    //Type = "folder";
//        //    //Created = original.CreatedOnDate;
//        //    //Modified = original.LastModifiedOnDate;
//        //    AllowEdit = usePortalRoot ? DnnAdamSecurityChecks.CanEdit(original) : !state.Security.UserIsRestricted || state.Security.FieldPermissionOk(GrantSets.WriteSomething);
//        //}

//        internal AdamItem(IFile original, bool usePortalRoot, AdamState state)
//            : base(false, original.Id, original.FolderId, original.FullName, original.Size,
//                original.Created, original.Modified)
//        {
//            Path = original.Path; // (original.StorageLocation == 0) ? original.Path : FileLinkClickController.Instance.GetFileLinkClick(original);
//            AllowEdit = usePortalRoot ? new DnnAdamSecurityChecks().CanEditFolder(original.FolderId) : !state.Security.UserIsRestricted || state.Security.FieldPermissionOk(GrantSets.WriteSomething);
//        }


//        internal AdamItem(IFolder folder, bool usePortalRoot, AdamState state)
//            :base(true, folder.Id, folder.ParentId, folder.Name, 0, folder.Created, folder.Modified)
//        {
//            Path = folder.Path;
//            AllowEdit = usePortalRoot ? new DnnAdamSecurityChecks().CanEditFolder(folder.Id) : !state.Security.UserIsRestricted || state.Security.FieldPermissionOk(GrantSets.WriteSomething);
//        }
//    }
//}