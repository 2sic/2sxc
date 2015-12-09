using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Services.FileSystem;
using ToSic.Eav;

namespace ToSic.SexyContent.Adam
{
    public class AdamNavigator : AdamFolder
    {
        public Core Core;

        public AdamNavigator(App app, Razor.Helpers.DnnHelper dnn, Guid entityGuid, string fieldName)
        {
            Core = new Core(app, dnn, entityGuid, fieldName);

            if (!Exists)
                return;

            var f = Core.Get(Root) as FolderInfo;

            if (f == null)
                return;

            PortalID = f.PortalID;
            FolderPath = f.FolderPath;
            MappedPath = f.MappedPath;
            StorageLocation = f.StorageLocation;
            IsProtected = f.IsProtected;
            IsCached = f.IsCached;
            FolderMappingID = f.FolderMappingID;
            LastUpdated = f.LastUpdated;
            FolderID = f.FolderID;
            DisplayName = f.DisplayName;
            DisplayPath = f.DisplayPath;
            IsVersioned = f.IsVersioned;
            KeyID = f.KeyID;
            ParentID = f.ParentID;
            UniqueId = f.UniqueId;
            VersionGuid = f.VersionGuid;
            WorkflowID = f.WorkflowID;

        }

        public string Root => Core.GeneratePath("");

        public bool Exists => Core.Exists(Root);


    }

    public class AdamFolder : FolderInfo
    {
        private IFolderManager _fldm = FolderManager.Instance;
        private IFileManager _filem = FileManager.Instance;

        /// <summary>
        /// Metadata for this folder
        /// This is usually an entity which has additional information related to this file
        /// </summary>
        public IEntity Metadata { get; }

        private IEnumerable<AdamFolder> _folders;

        /// <summary>
        ///  Get all subfolders
        /// </summary>
        public IEnumerable<AdamFolder> Folders
        {
            get
            {
                if (_folders == null)
                {
                    // this is to skip it if it doesn't have subfolders...
                    if (!HasChildren || string.IsNullOrEmpty(FolderName))
                        return _folders = new List<AdamFolder>();

                    var firstList = _fldm.GetFolders(this);

                    _folders = firstList?.Select(f => new AdamFolder()
                    {
                        PortalID = f.PortalID,
                        FolderPath = f.FolderPath,
                        MappedPath = f.MappedPath,
                        StorageLocation = f.StorageLocation,
                        IsProtected = f.IsProtected,
                        IsCached = f.IsCached,
                        FolderMappingID = f.FolderMappingID,
                        LastUpdated = f.LastUpdated,
                        FolderID = f.FolderID,
                        DisplayName = f.DisplayName,
                        DisplayPath = f.DisplayPath,
                        IsVersioned = f.IsVersioned,
                        KeyID = (f as FolderInfo)?.KeyID ?? 0,
                        ParentID = f.ParentID,
                        UniqueId = f.UniqueId,
                        VersionGuid = f.VersionGuid,
                        WorkflowID = f.WorkflowID,

                    }).ToList()
                               ?? new List<AdamFolder>();
                }
                return _folders;
            }
        }


        private IEnumerable<AdamFile> _files;

        /// <summary>
        /// Get all files in this folder
        /// </summary>
        public IEnumerable<AdamFile> Files
        {
            get
            {
                if (_files == null)
                {
                    var firstList = _fldm.GetFiles(this);

                    _files = firstList?.Select(f => new AdamFile()
                    {
                        UniqueId = f.UniqueId,
                        VersionGuid = f.VersionGuid,
                        PortalId = f.PortalId,
                        FileName = f.FileName,
                        Extension = f.Extension,
                        Size = f.Size,
                        Width = f.Width,
                        Height = f.Height,
                        ContentType = f.ContentType,
                        Folder = f.Folder,
                        FolderId = f.FolderId,
                        StorageLocation = f.StorageLocation,
                        IsCached = f.IsCached,
                        SHA1Hash = f.SHA1Hash
                    }).ToList() 
                    ?? new List<AdamFile>();
                }
                return _files;
            }
        }
    }



    public class AdamFile : FileInfo
    {
        /// <summary>
        /// Metadata for this file
        /// This is usually an entity which has additional information related to this file
        /// </summary>
        public IEntity Metadata { get; }
    }

}