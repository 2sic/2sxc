using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DotNetNuke.Services.FileSystem;

namespace ToSic.SexyContent.Adam
{
    public class DnnFileSystem
    {
        private readonly IFolderManager _folderManager = FolderManager.Instance;

        public bool FolderExists(int tennantId, string path) => _folderManager.FolderExists(tennantId, path);

        public void AddFolder(int tennantId, string path)
        {
            try
            {
                _folderManager.AddFolder(tennantId, path);
            }
            catch (SqlException)
            {
                // don't do anything - this happens when multiple processes try to add the folder at the same time
                // like when two fields in a dialog cause the web-api to create the folders in parallel calls
                // see also https://github.com/2sic/2sxc/issues/811
            }
            catch (NullReferenceException)
            {
                // also catch this, as it's an additional exception which also happens in the AddFolder when a folder already existed
            }

        }

        internal FolderInfo Get(int tennantId, string path, AdamBrowseContext fsh) 
            => DnnToAdam(fsh, _folderManager.GetFolder(tennantId, path));


        public List<AdamFolder> GetFolders(int folderId, AdamBrowseContext adamBrowseContext) 
            => GetFolders(GetFolder(folderId), adamBrowseContext);

        private IFolderInfo GetFolder(int folderId) => _folderManager.GetFolder(folderId);

        public List<AdamFolder> GetFolders(IFolderInfo fldObj, AdamBrowseContext adamBrowseContext = null)
        {
            var firstList = _folderManager.GetFolders(fldObj);

            var folders = firstList?.Select(f => DnnToAdam(adamBrowseContext, f)).ToList()
                          ?? new List<AdamFolder>();
            return folders;
        }

        private static AdamFolder DnnToAdam(AdamBrowseContext adamBrowseContext, IFolderInfo f) => new AdamFolder
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
            //KeyID = (f as FolderInfo)?.KeyID ?? 0,
            ParentID = f.ParentID,
            UniqueId = f.UniqueId,
            //VersionGuid = f.VersionGuid,
            //WorkflowID = f.WorkflowID,
            AdamBrowseContext = adamBrowseContext,
            Name = f.DisplayName,
            CreatedOnDate = f.CreatedOnDate
        };


        public List<AdamFile> GetFiles(int folderId, AdamBrowseContext adamBrowseContext)
        {
            var fldObj = _folderManager.GetFolder(folderId);
            var firstList = _folderManager.GetFiles(fldObj);

            var files = firstList?.Select(f => DnnToAdam(adamBrowseContext, f)).ToList()
                     ?? new List<AdamFile>();
            return files;
        }

        private static AdamFile DnnToAdam(AdamBrowseContext adamBrowseContext, IFileInfo f) => new AdamFile
        {
            UniqueId = f.UniqueId,
            VersionGuid = f.VersionGuid,
            PortalId = f.PortalId,
            FileName = f.FileName,
            Extension = f.Extension,
            Size = f.Size,
            //Width = f.Width,
            //Height = f.Height,
            ContentType = f.ContentType,
            FileId = f.FileId,
            Folder = f.Folder,
            FolderId = f.FolderId,
            StorageLocation = f.StorageLocation,
            IsCached = f.IsCached,
            //SHA1Hash = f.SHA1Hash,
            Path = f.RelativePath,
            AdamBrowseContext = adamBrowseContext,

            CreatedOnDate = f.CreatedOnDate,
            Name = System.IO.Path.GetFileNameWithoutExtension(f.FileName)
        };
    }
}
