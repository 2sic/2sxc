using System;
using DotNetNuke.Services.FileSystem;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.Adam.WebApi
{
    public class AdamItem
    {
        public bool IsFolder, AllowEdit;
        public int Id, ParentId, Size, MetadataId;
        public string Path, Name, Type;
        public DateTime Created, Modified;

        internal AdamItem(IFileInfo original, bool usePortalRoot, AdamState state)
        {
            IsFolder = false;
            Id = original.FileId;
            ParentId = original.FolderId;
            Path = (original.StorageLocation == 0) ? original.RelativePath : FileLinkClickController.Instance.GetFileLinkClick(original);
            Name = original.FileName;
            Size = original.Size;
            Type = "unknown"; // will be set from the outside
            Created = original.CreatedOnDate;
            Modified = original.LastModifiedOnDate;
            AllowEdit = usePortalRoot ? DnnAdamSecurityChecks.CanEdit(original) : !state.Security.UserIsRestricted || state.Security.FieldPermissionOk(GrantSets.WriteSomething);
        }

        internal AdamItem(IFolderInfo original, bool usePortalRoot, AdamState state)
        {
            IsFolder = true;
            Id = original.FolderID;
            ParentId = original.ParentID;
            Path = original.DisplayPath;
            Name = original.DisplayName;
            Size = 0;
            Type = "folder";
            Created = original.CreatedOnDate;
            Modified = original.LastModifiedOnDate;
            AllowEdit = usePortalRoot ? DnnAdamSecurityChecks.CanEdit(original) : !state.Security.UserIsRestricted || state.Security.FieldPermissionOk(GrantSets.WriteSomething);
        }
    }
}