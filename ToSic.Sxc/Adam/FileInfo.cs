using System;

namespace ToSic.SexyContent.Adam
{
    public class FileInfo
    {
        //public int Width { get; internal set; }

        //public string SHA1Hash { get; internal set; }

        public int StorageLocation { get; internal set; }

        public int Size { get; internal set; }

        public int PortalId { get; internal set; }

        public bool IsCached { get; internal set; }

        //public int Height { get; internal set; }

        public int FolderId { get; internal set; }

        public string Folder { get; internal set; }

        public string FileName { get; internal set; }

        public Guid VersionGuid { get; internal set; }

        public Guid UniqueId { get; internal set; }

        public int FileId { get; internal set; }

        public string Extension { get; internal set; }

        public string ContentType { get; internal set; }

        public string Path { get; internal set; }

        public DateTime CreatedOnDate { get; internal set; }

    }
}
