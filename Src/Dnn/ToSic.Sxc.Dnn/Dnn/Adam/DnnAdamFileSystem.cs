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

namespace ToSic.Sxc.Dnn.Adam
{
    public class DnnAdamFileSystem : HasLog, IAdamFileSystem<int, int>
    {
        #region Constructor / DI / Init

        public DnnAdamFileSystem(): base("Dnn.FilSys") { }

        public IAdamFileSystem<int, int> Init(AdamManager<int, int> adamManager, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            var wrapLog = Log.Call();
            AdamContext = adamManager;
            wrapLog("ok");
            return this;
        }


        protected AdamManager<int, int> AdamContext;

        #endregion

        #region FileSystem Settings

        public int MaxUploadKb()
            => (ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection)
               ?.MaxRequestLength ?? 1;

        #endregion

        #region Files
        private readonly IFileManager _dnnFiles = FileManager.Instance;

        public File<int, int> GetFile(int fileId)
        {
            var file = _dnnFiles.GetFile(fileId);
            return DnnToAdam(file);
        }

        public void Rename(IFile file, string newName)
        {
            var callLog = Log.Call();
            var dnnFile = _dnnFiles.GetFile(file.AsDnn().SysId);
            _dnnFiles.RenameFile(dnnFile, newName);
            callLog("ok");
        }

        public void Delete(IFile file)
        {
            var callLog = Log.Call();
            var dnnFile = _dnnFiles.GetFile(file.AsDnn().SysId);
            _dnnFiles.DeleteFile(dnnFile);
            callLog("ok");
        }

        public File<int, int> Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName)
        {
            var callLog = Log.Call<File<int, int>>($"..., ..., {fileName}, {ensureUniqueName}");
            if (ensureUniqueName)
                fileName = FindUniqueFileName(parent, fileName);
            var dnnFolder = _dnnFolders.GetFolder(parent.AsDnn().SysId);
            var dnnFile = _dnnFiles.AddFile(dnnFolder, Path.GetFileName(fileName), body);
            return callLog("ok", GetFile(dnnFile.FileId));
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
            var callLog = Log.Call<string>($"..., {fileName}");

            var dnnFolder = _dnnFolders.GetFolder(parentFolder.AsDnn().SysId);
            var name = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            for (var i = 1; i < AdamFileSystemBasic.MaxSameFileRetries && _dnnFiles.FileExists(dnnFolder, Path.GetFileName(fileName)); i++)
                fileName = $"{name}-{i}{ext}";

            return callLog(fileName, fileName);
        }

        #endregion



        #region Folders
        
        private readonly IFolderManager _dnnFolders = FolderManager.Instance;

        public bool FolderExists(string path) => _dnnFolders.FolderExists(AdamContext.Site.Id, path);



        public void AddFolder(string path)
        {
            var callLog = Log.Call(path);
            try
            {
                _dnnFolders.AddFolder(AdamContext.Site.Id, path);
                callLog("ok");
            }
            catch (SqlException)
            {
                // don't do anything - this happens when multiple processes try to add the folder at the same time
                // like when two fields in a dialog cause the web-api to create the folders in parallel calls
                // see also https://github.com/2sic/2sxc/issues/811
                Log.Add("error in DNN SQL, probably folder already exists");
            }
            catch (FolderAlreadyExistsException)
            {
                // Dnn reports it already exists - it shouldn't have got here because that was checked before
                // but I guess depending on the DNN version this isn't 100% reliable
                Log.Add("Dnn says folder already exists");
            }
            catch (NullReferenceException)
            {
                // also catch this, as it's an additional exception which also happens in the AddFolder when a folder already existed
                Log.Add("error, probably folder already exists");
            }

            callLog("?");
        }

        public void Rename(IFolder folder, string newName)
        {
            var callLog = Log.Call($"..., {newName}");
            var fld = _dnnFolders.GetFolder(folder.AsDnn().SysId);
            _dnnFolders.RenameFolder(fld, newName);
            callLog("ok");
        }

        public void Delete(IFolder folder)
        {
            var callLog = Log.Call();
            _dnnFolders.DeleteFolder(folder.AsDnn().SysId);
            callLog("ok");
        }

        public Folder<int, int> Get(string path)
        {
            var callLog = Log.Call<Folder<int, int>>(path);
            return callLog(null, DnnToAdam(_dnnFolders.GetFolder(AdamContext.Site.Id, path)));
        }

        public List<Folder<int, int>> GetFolders(IFolder folder)
        {
            var callLog = Log.Call<List<Folder<int, int>>>();
            var fldObj = GetDnnFolder(folder.AsDnn().SysId);
            if(fldObj == null) return new List<Folder<int, int>>();

            var firstList = _dnnFolders.GetFolders(fldObj);
            var folders = firstList?.Select(DnnToAdam).ToList()
                          ?? new List<Folder<int, int>>();
            return callLog($"{folders.Count}", folders);
        }

        public Folder<int, int> GetFolder(int folderId) => DnnToAdam(GetDnnFolder(folderId));

        #endregion




        #region Dnn typed calls

        private IFolderInfo GetDnnFolder(int folderId) => _dnnFolders.GetFolder(folderId);


        public List<File<int, int>> GetFiles(IFolder folder)
        {
            var callLog = Log.Call<List<File<int, int>>>();
            var fldObj = _dnnFolders.GetFolder(folder.AsDnn().SysId);
            // sometimes the folder doesn't exist for whatever reason
            if (fldObj == null) return  new List<File<int, int>>();

            // try to find the files
            var firstList = _dnnFolders.GetFiles(fldObj);
            var files = firstList?.Select(DnnToAdam).ToList()
                     ?? new List<File<int, int>>();
            return callLog($"{files.Count}", files);
        }

        #endregion

        #region DnnToAdam

        private const string ErrorDnnObjectNull = "Tried to create Adam object but the original is invalid. " +
                                                    "Probably the DNN file system is out of sync. " +
                                                    "Re-Sync in the DNN files recursively (in Admin - Files) and the error should go away. ";

        private Folder<int, int> DnnToAdam(IFolderInfo dnnFolderInfo)
        {
            var callLog = Log.Call<Folder<int, int>>();
            
            if (dnnFolderInfo == null) throw new ArgumentNullException(nameof(dnnFolderInfo), ErrorDnnObjectNull);

            return callLog(null, new Folder<int, int>(AdamContext)
            {
                Path = dnnFolderInfo.FolderPath,
                SysId = dnnFolderInfo.FolderID,

                ParentSysId = dnnFolderInfo.ParentID,

                Name = dnnFolderInfo.DisplayName,
                Created = dnnFolderInfo.CreatedOnDate,
                Modified = dnnFolderInfo.LastModifiedOnDate,
                Url = AdamContext.Site.ContentPath + dnnFolderInfo.FolderPath,
            });
        }


        private File<int, int> DnnToAdam(IFileInfo dnnFileInfo)
        {
            var callLog = Log.Call<File<int, int>>();
            
            if (dnnFileInfo == null) throw new ArgumentNullException(nameof(dnnFileInfo), ErrorDnnObjectNull);

            return callLog(null, new File<int, int>(AdamContext)
            {
                FullName = dnnFileInfo.FileName,
                Extension = dnnFileInfo.Extension,
                Size = dnnFileInfo.Size,
                SysId = dnnFileInfo.FileId,
                Folder = dnnFileInfo.Folder,
                ParentSysId = dnnFileInfo.FolderId,

                Path = dnnFileInfo.RelativePath,

                Created = dnnFileInfo.CreatedOnDate,
                Modified = dnnFileInfo.LastModifiedOnDate,
                Name = Path.GetFileNameWithoutExtension(dnnFileInfo.FileName),
                Url = dnnFileInfo.StorageLocation == 0
                    ? AdamContext.Site.ContentPath + dnnFileInfo.Folder + dnnFileInfo.FileName
                    : FileLinkClickController.Instance.GetFileLinkClick(dnnFileInfo),
                PhysicalPath = dnnFileInfo.PhysicalPath,
            });
        }

        #endregion
    }
}
