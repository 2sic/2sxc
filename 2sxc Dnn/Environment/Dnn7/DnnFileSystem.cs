using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DotNetNuke.Services.FileSystem;
using ToSic.Eav.Apps.Assets;

namespace ToSic.Sxc.Adam
{
    public class DnnFileSystem : IEnvironmentFileSystem
    {
        private readonly IFolderManager _folderManager = FolderManager.Instance;

        public bool FolderExists(int tenantId, string path) => _folderManager.FolderExists(tenantId, path);

        public void AddFolder(int tenantId, string path)
        {
            try
            {
                _folderManager.AddFolder(tenantId, path);
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

        public Folder Get(int tenantId, string path, AdamBrowseContext fsh) 
            => DnnToAdam(fsh, _folderManager.GetFolder(tenantId, path));


        public List<AdamFolder> GetFolders(int folderId, AdamBrowseContext adamBrowseContext) 
            => GetFolders(GetFolder(folderId), adamBrowseContext);

        private IFolderInfo GetFolder(int folderId) => _folderManager.GetFolder(folderId);

        private List<AdamFolder> GetFolders(IFolderInfo fldObj, AdamBrowseContext adamBrowseContext = null)
        {
            var firstList = _folderManager.GetFolders(fldObj);

            var folders = firstList?.Select(f => DnnToAdam(adamBrowseContext, f)).ToList()
                          ?? new List<AdamFolder>();
            return folders;
        }

        private AdamFolder DnnToAdam(AdamBrowseContext adamBrowseContext, IFolderInfo f) => new AdamFolder(this)
        {
            FolderPath = f.FolderPath,
            Id = f.FolderID,

            AdamBrowseContext = adamBrowseContext,
            Name = f.DisplayName,
            CreatedOnDate = f.CreatedOnDate,
            LastUpdated = f.LastUpdated,

            // commented out stuff is from DNN
            // but it will probably never be cross-platform
            //DisplayName = f.DisplayName,
            //DisplayPath = f.DisplayPath,            //PortalID = f.PortalID,
            //MappedPath = f.MappedPath,
            //StorageLocation = f.StorageLocation,
            //IsProtected = f.IsProtected,
            //IsCached = f.IsCached,
            //FolderMappingID = f.FolderMappingID,
            //IsVersioned = f.IsVersioned,
            //KeyID = (f as FolderInfo)?.KeyID ?? 0,
            //ParentID = f.ParentID,
            //UniqueId = f.UniqueId,
            //VersionGuid = f.VersionGuid,
            //WorkflowID = f.WorkflowID,
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
            FileName = f.FileName,
            Extension = f.Extension,
            Size = f.Size,
            Id = f.FileId,
            Folder = f.Folder,
            FolderId = f.FolderId,

            Path = f.RelativePath,
            AdamBrowseContext = adamBrowseContext,

            CreatedOnDate = f.CreatedOnDate,
            Name = System.IO.Path.GetFileNameWithoutExtension(f.FileName)

            // commented out stuff is from DNN
            // but it will probably never be cross-platform
            //UniqueId = f.UniqueId,
            //VersionGuid = f.VersionGuid,
            //PortalId = f.PortalId, 
            //Width = f.Width,
            //Height = f.Height,
            //ContentType = f.ContentType,
            //StorageLocation = f.StorageLocation,
            //IsCached = f.IsCached,
            //SHA1Hash = f.SHA1Hash,
        };
    }
}
