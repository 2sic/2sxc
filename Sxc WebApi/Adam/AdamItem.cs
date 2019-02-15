using System;
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
            AllowEdit = true; // todo: STV
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
            AllowEdit = true; // todo: STV
        }
    }
}