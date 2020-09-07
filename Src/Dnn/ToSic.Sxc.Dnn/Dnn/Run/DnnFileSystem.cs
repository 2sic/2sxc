using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DotNetNuke.Services.FileSystem;
using ToSic.Eav.Logging;
using ToSic.Sxc.Adam;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnFileSystem : HasLog, IEnvironmentFileSystem
    {
        #region Constructor / DI / Init

        public DnnFileSystem(): base("Dnn.FilSys") { }

        public IEnvironmentFileSystem Init(AdamAppContext adamContext)
        {
            AdamContext = adamContext;
            return this;
        }

        protected AdamAppContext AdamContext;

        #endregion

        #region Files
        private readonly IFileManager _dnnFiles = FileManager.Instance;

        public IFile GetFile(int fileId)
        {
            var file = _dnnFiles.GetFile(fileId);
            return DnnToAdam(AdamContext, file);
        }

        public void Rename(IFile file, string newName)
        {
            var dnnFile = _dnnFiles.GetFile(file.Id);
            _dnnFiles.RenameFile(dnnFile, newName);
        }

        public void Delete(IFile file)
        {
            var dnnFile = _dnnFiles.GetFile(file.Id);
            _dnnFiles.DeleteFile(dnnFile);
        }

        #endregion

        #region Folders




        private readonly IFolderManager _dnnFolders = FolderManager.Instance;

        public bool FolderExists(int tenantId, string path) => _dnnFolders.FolderExists(tenantId, path);



        public void AddFolder(int tenantId, string path)
        {
            try
            {
                _dnnFolders.AddFolder(tenantId, path);
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

        public void Rename(IFolder folder, string newName)
        {
            var fld = _dnnFolders.GetFolder(folder.Id);
            _dnnFolders.RenameFolder(fld, newName);
        }

        public void Delete(IFolder folder)
        {
            // var fld = _folderManager.GetFolder(folder.Id);
            _dnnFolders.DeleteFolder(folder.Id);
        }

        public Eav.Apps.Assets.Folder Get(int tenantId, string path) 
            => DnnToAdam(_dnnFolders.GetFolder(tenantId, path));

        public List<Folder> GetFolders(int folderId) 
            => GetFolders(GetFolder(folderId));

        Folder IEnvironmentFileSystem.GetFolder(int folderId) => DnnToAdam(GetFolder(folderId));

        #endregion




        #region Dnn typed calls

        private IFolderInfo GetFolder(int folderId) => _dnnFolders.GetFolder(folderId);

        private List<Folder> GetFolders(IFolderInfo fldObj)
        {
            var firstList = _dnnFolders.GetFolders(fldObj);
            var folders = firstList?.Select(f => DnnToAdam(/*AdamContext,*/ f)).ToList()
                          ?? new List<Folder>();
            return folders;
        }

        public List<File> GetFiles(int folderId)
        {
            var fldObj = _dnnFolders.GetFolder(folderId);
            // sometimes the folder doesn't exist for whatever reason
            if (fldObj == null) return  new List<File>();

            // try to find the files
            var firstList = _dnnFolders.GetFiles(fldObj);
            var files = firstList?.Select(f => DnnToAdam(AdamContext, f)).ToList()
                     ?? new List<File>();
            return files;
        }

        #endregion

        #region DnnToAdam
        private Folder DnnToAdam(IFolderInfo f)
            => new Folder(AdamContext)
            {
                Path = f.FolderPath,
                Id = f.FolderID,
                
                ParentId = f.ParentID,

                Name = f.DisplayName,
                Created = f.CreatedOnDate,
                Modified = f.LastModifiedOnDate, // .LastUpdated,

                Url = AdamContext.Tenant.ContentPath + f.FolderPath,
                // note: there are more properties in the DNN data, but we don't use it,
                // because it will probably never be cross-platform
            };



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
            Modified = f.LastModifiedOnDate,
            Name = System.IO.Path.GetFileNameWithoutExtension(f.FileName),
            Url = f.StorageLocation == 0 
                ? appContext.Tenant.ContentPath + f.Folder + f.FileName
                : FileLinkClickController.Instance.GetFileLinkClick(f),
            // note: there are more properties in the DNN data, but we don't use it,
            // because it will probably never be cross-platform
            };
        #endregion
    }
}
