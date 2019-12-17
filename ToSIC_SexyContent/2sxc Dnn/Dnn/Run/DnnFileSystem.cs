using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DotNetNuke.Services.FileSystem;
using ToSic.Sxc.Adam;

namespace ToSic.Sxc.Dnn.Run
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

        public Eav.Apps.Assets.Folder Get(int tenantId, string path, AdamAppContext appContext) 
            => DnnToAdam( appContext, _folderManager.GetFolder(tenantId, path));

        public List<Folder> GetFolders(int folderId, AdamAppContext appContext) 
            => GetFolders(GetFolder(folderId), appContext);

        private IFolderInfo GetFolder(int folderId) => _folderManager.GetFolder(folderId);

        private List<Folder> GetFolders(IFolderInfo fldObj, AdamAppContext appContext = null)
        {
            var firstList = _folderManager.GetFolders(fldObj);
            var folders = firstList?.Select(f => DnnToAdam(appContext, f)).ToList()
                          ?? new List<Folder>();
            return folders;
        }

        private Folder DnnToAdam(AdamAppContext appContext, IFolderInfo f) 
            => new Folder(appContext, this)
        {
            Path = f.FolderPath,
            Id = f.FolderID,

            Name = f.DisplayName,
            Created = f.CreatedOnDate,
            Modified = f.LastUpdated,
            // note: there are more properties in the DNN data, but we don't use it,
            // because it will probably never be cross-platform
        };


        public List<File> GetFiles(int folderId, AdamAppContext appContext)
        {
            var fldObj = _folderManager.GetFolder(folderId);
            // sometimes the folder doesn't exist for whatever reason
            if (fldObj == null) return  new List<File>();

            // try to find the files
            var firstList = _folderManager.GetFiles(fldObj);
            var files = firstList?.Select(f => DnnToAdam(appContext, f)).ToList()
                     ?? new List<File>();
            return files;
        }

        private static File DnnToAdam(AdamAppContext appContext, IFileInfo f) 
            => new File(appContext)
        {
            FullName = f.FileName,
            Extension = f.Extension,
            Size = f.Size,
            Id = f.FileId,
            Folder = f.Folder,
            FolderId = f.FolderId,

            Path = f.RelativePath,

            Created = f.CreatedOnDate,
            Name = System.IO.Path.GetFileNameWithoutExtension(f.FileName)
            // note: there are more properties in the DNN data, but we don't use it,
            // because it will probably never be cross-platform
        };
    }
}
