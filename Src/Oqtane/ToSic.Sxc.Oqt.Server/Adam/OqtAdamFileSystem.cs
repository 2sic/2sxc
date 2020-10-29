using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Oqtane.Models;
using Oqtane.Repository;
using ToSic.Eav.Logging;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Oqt.Shared.Dev;
using File = Oqtane.Models.File;
using System.Linq;

namespace ToSic.Sxc.Oqt.Server.Adam
{
    public class OqtAdamFileSystem : HasLog, IAdamFileSystem<int, int>
    {
        public IFileRepository FileRepository { get; }
        public IFolderRepository FolderRepository { get; }

        #region Constructor / DI / Init

        public OqtAdamFileSystem(IFileRepository fileRepository, IFolderRepository folderRepository) : base("Dnn.FilSys")
        {
            FileRepository = fileRepository;
            FolderRepository = folderRepository;
        }

        public IAdamFileSystem<int, int> Init(AdamAppContext<int, int> adamContext, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            var wrapLog = Log.Call();
            AdamContext = adamContext;
            wrapLog("ok");
            return this;
        }


        protected AdamAppContext<int, int> AdamContext;

        #endregion

        #region FileSystem Settings

        public int MaxUploadKb() => WipConstants.MaxUploadSize;

        //public int MaxUploadKb()
        //    => (ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection)
        //       ?.MaxRequestLength ?? 1;

        #endregion

        #region Files

        public File<int, int> GetFile(int fileId)
        {
            var file = FileRepository.GetFile(fileId);
            return OqtToAdam(file);
        }

        public void Rename(IFile file, string newName)
        {
            var callLog = Log.Call();
            var dnnFile = FileRepository.GetFile(file.AsOqt().SysId);
            WipConstants.AdamNotImplementedYet();
            Log.Add("Not implement yet in Oqtane");
            //FileRepository.RenameFile(dnnFile, newName);
            callLog("ok");
        }

        public void Delete(IFile file)
        {
            var callLog = Log.Call();
            var dnnFile = FileRepository.GetFile(file.AsOqt().SysId);
            FileRepository.DeleteFile(dnnFile.FileId);
            callLog("ok");
        }

        public File<int, int> Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName)
        {
            var callLog = Log.Call<File<int, int>>($"..., ..., {fileName}, {ensureUniqueName}");
            if (ensureUniqueName)
                fileName = FindUniqueFileName(parent, fileName);
            var dnnFolder = FolderRepository.GetFolder(parent.AsOqt().SysId);
            WipConstants.AdamNotImplementedYet();
            Log.Add("Not implement yet in Oqtane");
            //var dnnFile = FileRepository.AddFile(dnnFolder, Path.GetFileName(fileName), body);
            //return callLog("ok", GetFile(dnnFile.FileId));
            return callLog("not implemented", null);
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

            var dnnFolder = FolderRepository.GetFolder(parentFolder.AsOqt().SysId);
            var name = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            WipConstants.AdamNotImplementedYet();
            Log.Add("Not implement yet in Oqtane");
            //for (var i = 1; i < 1000 && FileRepository.FileExists(dnnFolder, Path.GetFileName(fileName)); i++)
            //    fileName = $"{name}-{i}{ext}";

            return callLog(fileName, fileName);
        }

        #endregion



        #region Folders
        

        public bool FolderExists(string path) => FolderRepository.GetFolder(AdamContext.Tenant.Id, path) != null;


        public void AddFolder(string path)
        {
            var callLog = Log.Call(path);
            try
            {
                WipConstants.AdamNotImplementedYet();
                Log.Add("Not implement yet in Oqtane");
                //FolderRepository.AddFolder(AdamContext.Tenant.Id, path);
                callLog("ok");
            }
            catch (SqlException)
            {
                // don't do anything - this happens when multiple processes try to add the folder at the same time
                // like when two fields in a dialog cause the web-api to create the folders in parallel calls
                // see also https://github.com/2sic/2sxc/issues/811
                Log.Add("error in DNN SQL, probably folder already exists");
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
            var fld = FolderRepository.GetFolder(folder.AsOqt().SysId);
            WipConstants.AdamNotImplementedYet();
            Log.Add("Not implement yet in Oqtane");
            //FolderRepository.RenameFolder(fld, newName);
            callLog("ok");
        }

        public void Delete(IFolder folder)
        {
            var callLog = Log.Call();
            FolderRepository.DeleteFolder(folder.AsOqt().SysId);
            callLog("ok");
        }

        public Folder<int, int> Get(string path) 
            => OqtToAdam(FolderRepository.GetFolder(AdamContext.Tenant.Id, path));

        public List<Folder<int, int>> GetFolders(IFolder folder)
        {
            var callLog = Log.Call<List<Folder<int, int>>>();
            var fldObj = GetDnnFolder(folder.AsOqt().SysId);
            if(fldObj == null) return new List<Folder<int, int>>();

            var firstList = FolderRepository.GetFolders(fldObj.FolderId);
            var folders = firstList?.Select(OqtToAdam).ToList()
                          ?? new List<Folder<int, int>>();
            return callLog($"{folders.Count}", folders);
        }

        public Folder<int, int> GetFolder(int folderId) => OqtToAdam(GetDnnFolder(folderId));

        #endregion




        #region Dnn typed calls

        private Folder GetDnnFolder(int folderId) => FolderRepository.GetFolder(folderId);


        public List<File<int, int>> GetFiles(IFolder folder)
        {
            var callLog = Log.Call<List<File<int, int>>>();
            var fldObj = FolderRepository.GetFolder(folder.AsOqt().SysId);
            // sometimes the folder doesn't exist for whatever reason
            if (fldObj == null) return  new List<File<int, int>>();

            // try to find the files
            var firstList = FileRepository.GetFiles(fldObj.FolderId);
            var files = firstList?.Select(OqtToAdam).ToList()
                     ?? new List<File<int, int>>();
            return callLog($"{files.Count}", files);
        }

        #endregion

        #region DnnToAdam
        private Folder<int, int> OqtToAdam(Folder f)
            => new Folder<int, int>(AdamContext)
            {
                Path = f.Path,
                SysId = f.FolderId,
                
                ParentSysId = f.ParentId ?? WipConstants.ParentFolderNotFound,

                Name = f.Name,
                Created = f.CreatedOn,
                Modified = f.ModifiedOn,
                Url = AdamContext.Tenant.ContentPath + f.Path,
            };



        private File<int, int> OqtToAdam(File f)
            => new File<int, int>(AdamContext)
            {
                FullName = f.Name,
                Extension = f.Extension,
                Size = f.Size,
                SysId = f.FileId,
                Folder = f.Folder.Name,
                ParentSysId = f.FolderId,

                Path = f.Folder.Path,

                Created = f.CreatedOn,
                Modified = f.ModifiedOn,
                Name = Path.GetFileNameWithoutExtension(f.Name),
                Url = // f.StorageLocation == 0
                    /*?*/ AdamContext.Tenant.ContentPath + f.Folder + f.Name
                    //: FileLinkClickController.Instance.GetFileLinkClick(f)
            };

        #endregion
    }
}
