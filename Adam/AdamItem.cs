using DotNetNuke.Services.FileSystem;

namespace ToSic.SexyContent.Adam
{
    public class AdamItem

    {
        public bool IsFolder;
        public int Id, ParentId;
        public string Path, Name;

        public AdamItem(IFileInfo original)
        {
            IsFolder = false;
            Id = original.FileId;
            ParentId = original.FolderId;
            Path = original.PhysicalPath;
            Name = original.FileName;

        }

        public AdamItem(IFolderInfo original)
        {
            IsFolder = true;
            Id = original.FolderID;
            ParentId = original.ParentID;
            Path = original.DisplayPath;
            Name = original.DisplayName;
        }
    }
}