using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Oqtane.Models;
using Oqtane.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Eav.Helpers;
using ToSic.Eav.Run;
using ToSic.Lib.Logging;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Oqt.Server.Integration;
using ToSic.Sxc.Oqt.Shared.Dev;
using File = Oqtane.Models.File;

namespace ToSic.Sxc.Oqt.Server.Adam
{
    public class OqtAdamFileSystem : AdamFileSystemBasic, IAdamFileSystem<int, int>
    {
        private readonly IServerPaths _serverPaths;
        private readonly IAdamPaths _adamPaths;
        public IFileRepository OqtFileRepository { get; }
        public IFolderRepository OqtFolderRepository { get; }

        #region Constructor / DI / Init

        public OqtAdamFileSystem(IFileRepository oqtFileRepository, IFolderRepository oqtFolderRepository, IServerPaths serverPaths, IAdamPaths adamPaths) : base(adamPaths)
        {
            ConnectServices(
                _serverPaths = serverPaths,
                _adamPaths = adamPaths,
                OqtFileRepository = oqtFileRepository,
                OqtFolderRepository = oqtFolderRepository
            );
        }

        public IAdamFileSystem<int, int> Init(AdamManager<int, int> adamContext)
        {
            var wrapLog = Log.Fn<IAdamFileSystem<int, int>>();
            AdamContext = adamContext;
            _adamPaths.Init(adamContext);
            return wrapLog.ReturnAsOk(this);
        }

        protected AdamManager<int, int> AdamContext;

        #endregion

        #region FileSystem Settings

        public new int MaxUploadKb() => WipConstants.MaxUploadSize;

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

        public new void Rename(IFile file, string newName) => Log.Do(l =>
        {
            try
            {
                var path = _serverPaths.FullContentPath(file.Path);

                var currentFilePath = Path.Combine(path, file.FullName);
                if (!TryToRenameFile(currentFilePath, newName)) return "";

                var oqtFile = OqtFileRepository.GetFile(file.AsOqt().SysId);
                oqtFile.Name = newName;
                OqtFileRepository.UpdateFile(oqtFile);
                l.A($"VirtualFile {oqtFile.FileId} renamed to {oqtFile.Name}");

                return ("ok");
            }
            catch (Exception e)
            {
                return ($"Error:{e.Message}; {e.InnerException}");
            }
        });

        public new void Delete(IFile file) => Log.Do(() =>
        {
            var oqtFile = OqtFileRepository.GetFile(file.AsOqt().SysId);
            OqtFileRepository.DeleteFile(oqtFile.FileId);
        });

        public new File<int, int> Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName)
        {
            var callLog = Log.Fn<File<int, int>>($"..., ..., {fileName}, {ensureUniqueName}");
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
            return callLog.ReturnAsOk(GetFile(oqtFile.FileId));
        }

        /// <summary>
        /// When uploading a new file, we must verify that the name isn't used.
        /// If it is used, walk through numbers to make a new name which isn't used.
        /// </summary>
        /// <param name="parentFolder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private new string FindUniqueFileName(IFolder parentFolder, string fileName)
        {
            var callLog = Log.Fn<string>($"..., {fileName}");

            var oqtFolder = OqtFolderRepository.GetFolder(parentFolder.AsOqt().SysId);
            var name = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            for (var i = 1; i < AdamFileSystemBasic.MaxSameFileRetries
                            && System.IO.File.Exists(Path.Combine(_serverPaths.FullContentPath(AdamContext.Site.ContentPath), oqtFolder.Path, Path.GetFileName(fileName))); i++)
                fileName = $"{name}-{i}{ext}";

            return callLog.Return(fileName, fileName);
        }

        #endregion



        #region Folders


        public new bool FolderExists(string path) => GetOqtFolderByName(path) != null;

        private Folder GetOqtFolderByName(string path) => OqtFolderRepository.GetFolder(AdamContext.Site.Id, path.EnsureOqtaneFolderFormat());

        public new void AddFolder(string path) => Log.Do(() =>
        {
            path = path.Backslash();
            if (FolderExists(path)) return "";

            try
            {
                // find parent
                var pathWithPretendFileName = path.TrimLastSlash();
                var parent = Path.GetDirectoryName(pathWithPretendFileName) + "/";
                var subfolder = Path.GetFileName(pathWithPretendFileName);
                var parentFolder = GetOqtFolderByName(parent) ?? GetOqtFolderByName("");

                // Create the new virtual folder
                CreateVirtualFolder(parentFolder, path, subfolder);
                return "ok";
            }
            catch (SqlException)
            {
                // don't do anything - this happens when multiple processes try to add the folder at the same time
                // like when two fields in a dialog cause the web-api to create the folders in parallel calls
                // see also https://github.com/2sic/2sxc/issues/811
                return "error in SQL, probably folder already exists";
            }
            catch (DbUpdateException)
            {
                return $"error in EF, probably folder already exists";
            }
            catch (NullReferenceException)
            {
                // also catch this, as it's an additional exception which also happens in the AddFolder when a folder already existed
                return "error, probably folder already exists";
            }
        });

        private Folder CreateVirtualFolder(Folder parentFolder, string path, string folder)
        {
            var newVirtualFolder = AdamFolderHelper.NewVirtualFolder(AdamContext.Site.Id, parentFolder.FolderId, path, folder);
            OqtFolderRepository.AddFolder(newVirtualFolder);
            return newVirtualFolder;
        }

        public new void Rename(IFolder folder, string newName) => Log.Do($"..., {newName}", () =>
        {
            var fld = OqtFolderRepository.GetFolder(folder.AsOqt().SysId);
            WipConstants.AdamNotImplementedYet();
            Log.A("Not implement yet in Oqtane");
        });

        public new void Delete(IFolder folder) => Log.Do(() => OqtFolderRepository.DeleteFolder(folder.AsOqt().SysId));

        public new Folder<int, int> Get(string path) => OqtToAdam(GetOqtFolderByName(path));

        public new List<Folder<int, int>> GetFolders(IFolder folder) 
        {
            var callLog = Log.Fn<List<Folder<int, int>>>();
            var fldObj = GetOqtFolder(folder.AsOqt().SysId);
            if(fldObj == null) return new();

            var firstList = GetSubFoldersRecursive(fldObj);
            var folders = firstList?.Select(OqtToAdam).ToList()
                          ?? new List<Folder<int, int>>();
            return callLog.Return(folders, $"{folders.Count}");
        }

        private List<Folder> GetSubFoldersRecursive(Folder parentFolder, List<Folder> allFolders = null, List<Folder> subFolders = null)
        {
            allFolders ??= OqtFolderRepository.GetFolders(parentFolder.SiteId).ToList();
            subFolders ??= new();
            allFolders.Where(f => f.ParentId == parentFolder.FolderId).ToList().ForEach(f =>
            {
                subFolders.Add(f);
                GetSubFoldersRecursive(f, allFolders, subFolders);
            });
            return subFolders;
        }

        public Folder<int, int> GetFolder(int folderId) => OqtToAdam(GetOqtFolder(folderId));

        #endregion

        #region Oqtane typed calls

        private Folder GetOqtFolder(int folderId) => OqtFolderRepository.GetFolder(folderId);


        public new List<File<int, int>> GetFiles(IFolder folder)
        {
            var callLog = Log.Fn<List<File<int, int>>>();
            var fldObj = OqtFolderRepository.GetFolder(folder.AsOqt().SysId);
            // sometimes the folder doesn't exist for whatever reason
            if (fldObj == null) return  new();

            // try to find the files
            var firstList = OqtFileRepository.GetFiles(fldObj.FolderId);
            var files = firstList?.Select(OqtToAdam).ToList()
                     ?? new List<File<int, int>>();
            return callLog.Return(files, $"{files.Count}");
        }

        #endregion

        #region OqtToAdam
        private Folder<int, int> OqtToAdam(Folder f)
            => new(AdamContext)
            {
                Path = ((OqtAdamPaths)_adamPaths).Path(f.Path),
                SysId = f.FolderId,

                ParentSysId = f.ParentId ?? WipConstants.ParentFolderNotFound,

                Name = f.Name,
                Created = f.CreatedOn,
                Modified = f.ModifiedOn,
                Url = _adamPaths.Url(f.Path.ForwardSlash()),
                PhysicalPath = _adamPaths.PhysicalPath(f.Path),
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
                Url = _adamPaths.Url(Path.Combine(f.Folder.Path, f.Name).ForwardSlash()),
                PhysicalPath = _adamPaths.PhysicalPath(Path.Combine(f.Folder.Path, f.Name)),
        };
            return adamFile;
        }

        #endregion
    }
}
