using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using DotNetNuke.Services.FileSystem;
using ToSic.Eav.Logging;
using ToSic.Sxc.Adam;
using File = ToSic.Sxc.Adam.File;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnFileSystem : HasLog, IAdamFileSystem
    {
        #region Constructor / DI / Init

        public DnnFileSystem(): base("Dnn.FilSys") { }

        public IAdamFileSystem Init(AdamAppContext adamContext)
        {
            AdamContext = adamContext;
            return this;
        }


        protected AdamAppContext AdamContext;

        #endregion

        #region FileSystem Settings

        public int MaxUploadKb()
            => (ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection)
               ?.MaxRequestLength ?? 1;

        #endregion

        #region Files
        private readonly IFileManager _dnnFiles = FileManager.Instance;

        public IFile GetFile(int fileId)
        {
            var file = _dnnFiles.GetFile(fileId);
            return DnnToAdam(file);
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

        public IFile Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName)
        {
            if (ensureUniqueName)
                fileName = FindUniqueFileName(parent, fileName);
            var dnnFolder = _dnnFolders.GetFolder(parent.Id);
            var dnnFile = _dnnFiles.AddFile(dnnFolder, Path.GetFileName(fileName), body);
            return GetFile(dnnFile.FileId);
        }

        /// <summary>
        /// When uploading a new file, we must verify that the name isn't used. 
        /// If it is used, walk through numbers to make a new name which isn't used. 
        /// </summary>
        /// <param name="parentFolder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string FindUniqueFileName(IFolder parentFolder, string fileName)
        {
            var numberedFile = fileName;

            var dnnFolder = _dnnFolders.GetFolder(parentFolder.Id);
            bool FileExists(string fileToCheck) => _dnnFiles.FileExists(dnnFolder, Path.GetFileName(fileToCheck));

            for (var i = 1; i < 1000 && FileExists(numberedFile); i++)
                numberedFile = Path.GetFileNameWithoutExtension(fileName)
                               + "-" + i + Path.GetExtension(fileName);
            fileName = numberedFile;
            return fileName;
        }

        #endregion

        #region FolderPermissions

        //public bool CanUserViewFolder(int folderId)
        //{
        //    if (folderId <= 0) return false;
        //    var folder = (FolderInfo)_dnnFolders.GetFolder(folderId);
        //    return FolderPermissionController.CanViewFolder(folder);
        //}


        #endregion

        #region Folders




        private readonly IFolderManager _dnnFolders = FolderManager.Instance;

        public bool FolderExists(string path) => _dnnFolders.FolderExists(AdamContext.Tenant.Id, path);



        public void AddFolder(string path)
        {
            try
            {
                _dnnFolders.AddFolder(AdamContext.Tenant.Id, path);
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
            _dnnFolders.DeleteFolder(folder.Id);
        }

        public Folder Get(string path) 
            => DnnToAdam(_dnnFolders.GetFolder(AdamContext.Tenant.Id, path));

        public List<Folder> GetFolders(Eav.Apps.Assets.Folder folder) 
            => GetFolders(GetDnnFolder(folder.Id));

        public Folder GetFolder(int folderId) => DnnToAdam(GetDnnFolder(folderId));

        #endregion




        #region Dnn typed calls

        private IFolderInfo GetDnnFolder(int folderId) => _dnnFolders.GetFolder(folderId);

        private List<Folder> GetFolders(IFolderInfo fldObj)
        {
            var firstList = _dnnFolders.GetFolders(fldObj);
            var folders = firstList?.Select(DnnToAdam).ToList()
                          ?? new List<Folder>();
            return folders;
        }

        public List<File> GetFiles(Eav.Apps.Assets.Folder folder)
        {
            var folderId = folder.Id;
            var fldObj = _dnnFolders.GetFolder(folderId);
            // sometimes the folder doesn't exist for whatever reason
            if (fldObj == null) return  new List<File>();

            // try to find the files
            var firstList = _dnnFolders.GetFiles(fldObj);
            var files = firstList?.Select(DnnToAdam).ToList()
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



        private File DnnToAdam(IFileInfo f) 
            => new File(AdamContext)
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
            Name = Path.GetFileNameWithoutExtension(f.FileName),
            Url = f.StorageLocation == 0 
                ? AdamContext.Tenant.ContentPath + f.Folder + f.FileName
                : FileLinkClickController.Instance.GetFileLinkClick(f),
            // note: there are more properties in the DNN data, but we don't use it,
            // because it will probably never be cross-platform
            };
        #endregion
    }
}
