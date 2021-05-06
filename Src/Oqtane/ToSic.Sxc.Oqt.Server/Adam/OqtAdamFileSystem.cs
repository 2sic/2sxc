using Oqtane.Extensions;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Oqt.Shared.Dev;
using File = Oqtane.Models.File;

namespace ToSic.Sxc.Oqt.Server.Adam
{
    public class OqtAdamFileSystem : HasLog, IAdamFileSystem<int, int>
    {
        private readonly IServerPaths _serverPaths;
        private readonly IAdamPaths _adamPaths;
        public IFileRepository OqtFileRepository { get; }
        public IFolderRepository OqtFolderRepository { get; }

        #region Constructor / DI / Init

        public OqtAdamFileSystem(IFileRepository oqtFileRepository, IFolderRepository oqtFolderRepository, IServerPaths serverPaths, IAdamPaths adamPaths) : base("Dnn.FilSys")
        {
            _serverPaths = serverPaths;
            _adamPaths = adamPaths;
            OqtFileRepository = oqtFileRepository;
            OqtFolderRepository = oqtFolderRepository;
        }

        public IAdamFileSystem<int, int> Init(AdamManager<int, int> adamContext, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            var wrapLog = Log.Call();
            AdamContext = adamContext;
            _adamPaths.Init(adamContext, Log);
            wrapLog("ok");
            return this;
        }

        protected AdamManager<int, int> AdamContext;

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
            var file = OqtFileRepository.GetFile(fileId);
            return OqtToAdam(file);
        }

        public void Rename(IFile file, string newName)
        {
            var callLog = Log.Call();
            try
            {
                var path = Path.Combine(_serverPaths.FullContentPath(AdamContext.Site.ContentPath), file.Path);

                var currentFilePath = Path.Combine(path, file.FullName);
                if (TryToRenameFile(newName, currentFilePath, path))
                {
                    return;
                }

                var dnnFile = OqtFileRepository.GetFile(file.AsOqt().SysId);
                dnnFile.Name = newName;
                OqtFileRepository.UpdateFile(dnnFile);
                Log.Add($"VirtualFile {dnnFile.FileId} renamed to {dnnFile.Name}");

                callLog("ok");
            }
            catch (Exception e)
            {
                callLog($"Error:{e.Message}; {e.InnerException}");
            }
        }

        // TODO: try to inherit from AdamFileSystemBasic and use the TryToRename from there
        private bool TryToRenameFile(string newName, string fullPath, string path)
        {
            var callLog = Log.Call($"{newName}");
            if (!System.IO.File.Exists(fullPath))
            {
                callLog($"Can't rename because source file do not exists {fullPath}");
                return true;
            }

            var newFilePath = Path.Combine(path, newName);
            if (System.IO.File.Exists(newFilePath))
            {
                callLog($"Can't rename because file with new name already exists {newFilePath}");
                return true;
            }

            System.IO.File.Move(fullPath, newFilePath);
            Log.Add($"File renamed {fullPath} to {newFilePath}");
            return false;
        }

        public void Delete(IFile file)
        {
            var callLog = Log.Call();
            var dnnFile = OqtFileRepository.GetFile(file.AsOqt().SysId);
            OqtFileRepository.DeleteFile(dnnFile.FileId);
            callLog("ok");
        }

        public File<int, int> Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName)
        {
            var callLog = Log.Call<File<int, int>>($"..., ..., {fileName}, {ensureUniqueName}");
            if (ensureUniqueName)
                fileName = FindUniqueFileName(parent, fileName);
            var fullContentPath = _serverPaths.FullContentPath(parent.Path);
            Directory.CreateDirectory(fullContentPath);
            var filePath = Path.Combine(fullContentPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                body.CopyTo(stream);
            }
            var fileInfo = new FileInfo(filePath);

            // register into oqtane
            var oqtFileData = new File
            {
                Name = Path.GetFileName(fileName),
                FolderId = parent.Id,
                Extension = fileInfo.Extension.ToLowerInvariant().Replace(".", ""),
                Size = (int)fileInfo.Length,
                ImageHeight = 0,
                ImageWidth = 0
            };
            var oqtFile = OqtFileRepository.AddFile(oqtFileData);
            return callLog("ok", GetFile(oqtFile.FileId));
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

            var dnnFolder = OqtFolderRepository.GetFolder(parentFolder.AsOqt().SysId);
            var name = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            for (var i = 1; i < AdamFileSystemBasic.MaxSameFileRetries
                            && System.IO.File.Exists(Path.Combine(_serverPaths.FullContentPath(AdamContext.Site.ContentPath), dnnFolder.Path, Path.GetFileName(fileName))); i++)
                fileName = $"{name}-{i}{ext}";

            return callLog(fileName, fileName);
        }

        #endregion



        #region Folders


        public bool FolderExists(string path) => GetOqtFolderByName(path) != null;

        private Folder GetOqtFolderByName(string path) => OqtFolderRepository.GetFolder(AdamContext.Site.Id, path.Backslash());


        public void AddFolder(string path)
        {
            path = path.Backslash();
            var callLog = Log.Call(path);

            if (FolderExists(path)) return;

            try
            {
                // find parent
                var pathWithPretendFileName = path.TrimEnd().TrimEnd('/').TrimEnd('\\');
                var parent = Path.GetDirectoryName(pathWithPretendFileName) + Path.DirectorySeparatorChar;
                var subfolder = Path.GetFileName(pathWithPretendFileName);
                var parentFolder = GetOqtFolderByName(parent) ?? GetOqtFolderByName("");

                // Create the new virtual folder
                CreateVirtualFolder(parentFolder, path, subfolder);
                callLog("ok");
            }
            catch (SqlException)
            {
                // don't do anything - this happens when multiple processes try to add the folder at the same time
                // like when two fields in a dialog cause the web-api to create the folders in parallel calls
                // see also https://github.com/2sic/2sxc/issues/811
                Log.Add("error in SQL, probably folder already exists");
            }
            catch (NullReferenceException)
            {
                // also catch this, as it's an additional exception which also happens in the AddFolder when a folder already existed
                Log.Add("error, probably folder already exists");
            }

            callLog("?");
        }

        private Folder CreateVirtualFolder(Folder parentFolder, string path, string folder)
        {
            var newVirtualFolder = new Folder
            {
                SiteId = AdamContext.Site.Id,
                ParentId = parentFolder.FolderId,
                Name = folder,
                Path = path,
                Order = 1,
                IsSystem = true,
                Permissions = new List<Permission>
                {
                    new Permission(PermissionNames.View, RoleNames.Everyone, true),
                }.EncodePermissions()
            };
            OqtFolderRepository.AddFolder(newVirtualFolder);
            return newVirtualFolder;
        }

        public void Rename(IFolder folder, string newName)
        {
            var callLog = Log.Call($"..., {newName}");
            var fld = OqtFolderRepository.GetFolder(folder.AsOqt().SysId);
            WipConstants.AdamNotImplementedYet();
            Log.Add("Not implement yet in Oqtane");
            //FolderRepository.RenameFolder(fld, newName);
            callLog("ok");
        }

        public void Delete(IFolder folder)
        {
            var callLog = Log.Call();
            OqtFolderRepository.DeleteFolder(folder.AsOqt().SysId);
            callLog("ok");
        }

        public Folder<int, int> Get(string path) => OqtToAdam(GetOqtFolderByName(path));

        public List<Folder<int, int>> GetFolders(IFolder folder)
        {
            var callLog = Log.Call<List<Folder<int, int>>>();
            var fldObj = GetOqtFolder(folder.AsOqt().SysId);
            if(fldObj == null) return new List<Folder<int, int>>();

            var firstList = OqtFolderRepository.GetFolders(fldObj.FolderId);
            var folders = firstList?.Select(OqtToAdam).ToList()
                          ?? new List<Folder<int, int>>();
            return callLog($"{folders.Count}", folders);
        }

        public Folder<int, int> GetFolder(int folderId) => OqtToAdam(GetOqtFolder(folderId));

        #endregion

        #region Oqtane typed calls

        private Folder GetOqtFolder(int folderId) => OqtFolderRepository.GetFolder(folderId);


        public List<File<int, int>> GetFiles(IFolder folder)
        {
            var callLog = Log.Call<List<File<int, int>>>();
            var fldObj = OqtFolderRepository.GetFolder(folder.AsOqt().SysId);
            // sometimes the folder doesn't exist for whatever reason
            if (fldObj == null) return  new List<File<int, int>>();

            // try to find the files
            var firstList = OqtFileRepository.GetFiles(fldObj.FolderId);
            var files = firstList?.Select(OqtToAdam).ToList()
                     ?? new List<File<int, int>>();
            return callLog($"{files.Count}", files);
        }

        #endregion

        #region OqtToAdam
        private Folder<int, int> OqtToAdam(Folder f)
            => new Folder<int, int>(AdamContext)
            {
                Path = ((OqtAdamPaths)_adamPaths).Path(f.Path),
                SysId = f.FolderId,

                ParentSysId = f.ParentId ?? WipConstants.ParentFolderNotFound,

                Name = f.Name,
                Created = f.CreatedOn,
                Modified = f.ModifiedOn,
                Url = _adamPaths.Url(f.Path.ForwardSlash())
            };



        private File<int, int> OqtToAdam(File f)
        {
            var adamFile = new File<int, int>(AdamContext)
            {
                FullName = f.Name,
                Extension = f.Extension,
                Size = f.Size,
                SysId = f.FileId,
                Folder = f.Folder.Name,
                ParentSysId = f.FolderId,

                Path = ((OqtAdamPaths)_adamPaths).Path(f.Folder.Path),

                Created = f.CreatedOn,
                Modified = f.ModifiedOn,
                Name = Path.GetFileNameWithoutExtension(f.Name),
                Url = _adamPaths.Url(Path.Combine(f.Folder.Path, f.Name).ForwardSlash())
            };
            return adamFile;
        }

        #endregion
    }
}
