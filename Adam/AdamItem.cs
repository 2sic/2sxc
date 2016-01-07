using DotNetNuke.Services.FileSystem;

namespace ToSic.SexyContent.Adam
{
    public class AdamItem

    {
        public bool IsFolder;
        public int Id, ParentId, Size, MetadataId;
        public string Path, Name, Type;

        public AdamItem(IFileInfo original)
        {
            IsFolder = false;
            Id = original.FileId;
            ParentId = original.FolderId;
            Path = original.RelativePath;
            Name = original.FileName;
            Size = original.Size;
            Type = "unknown"; // will be set from the outside

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
        }


    }
}