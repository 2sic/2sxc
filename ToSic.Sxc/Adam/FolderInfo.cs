using System;

namespace ToSic.SexyContent.Adam
{
    public class FolderInfo
    {
        public int FolderMappingID { get; internal set; }
        public int StorageLocation { get; internal set; }

        public int PortalID { get; internal set; }

        public int ParentID { get; internal set; }

        public DateTime LastUpdated { get; internal set; }

        //public int WorkflowID { get; internal set; }

        public string MappedPath { get; internal set; }

        public bool IsVersioned { get; internal set; }

        public bool IsCached { get; internal set; }

        public string DisplayPath { get; internal set; }

        public string FolderPath { get; internal set; }

        public string DisplayName { get; internal set; }

        public bool HasChildren { get; internal set;}

        public string FolderName { get; internal set;}

        //public Guid VersionGuid { get; internal set; }

        public Guid UniqueId { get; internal set; }

        public int FolderID { get; internal set; }

        public bool IsProtected { get; internal set; }

        public int KeyID { get; internal set; }

        public DateTime CreatedOnDate { get; internal set; }
    }
}
