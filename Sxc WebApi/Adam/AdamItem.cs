using System;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Adam.WebApi
{
    public class AdamItem
    {
        public bool IsFolder, AllowEdit;
        public int Id, ParentId, Size, MetadataId;
        public string Path, Name, Type;
        public DateTime Created, Modified;

        public AdamItem(IFileInfo original)
        {
            IsFolder = false;
            Id = original.FileId;
            ParentId = original.FolderId;
            Path = original.RelativePath;
            Name = original.FileName;
            Size = original.Size;
            Type = "unknown"; // will be set from the outside
            Created = original.CreatedOnDate;
            Modified = original.LastModifiedOnDate;
            AllowEdit = CanEditFile(original); // todo: STV
        }

        public AdamItem(IFolderInfo original)
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
            AllowEdit = CanEditFolder(original); // todo: STV
        }

        private bool CanEditFile(IFileInfo file)
        {
            if (file == null) return false;
            var folder = (FolderInfo)FolderManager.Instance.GetFolder(file.FolderId);
            return CanEditFolder(folder);
        }

        private bool CanEditFolder(IFolderInfo folder)
        {
            if (folder == null) return false;
            return FolderPermissionController.CanAddFolder(folder as FolderInfo);
        }
    }
}