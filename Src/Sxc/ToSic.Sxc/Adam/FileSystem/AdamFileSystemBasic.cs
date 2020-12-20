using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Adam
{
    public class AdamFileSystemBasic: AdamFileSystemBase<string, string>, IAdamFileSystem<string, string>
    {
        #region Constructor / DI / Init

        public AdamFileSystemBasic(IAdamPaths adamPaths) : base(LogNames.Basic)
        {
            _adamPaths = adamPaths;
        }
        private readonly IAdamPaths _adamPaths;

        public IAdamFileSystem<string, string> Init(AdamManager<string, string> adamManager, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            AdamManager = adamManager;
            _adamPaths.Init(adamManager, Log);
            return this;
        }

        protected AdamManager<string, string> AdamManager;

        #endregion


        public int MaxUploadKb() => MaxUploadKbDefault;

        public File<string, string> GetFile(string fileId)
        {
            var dir = AdjustPathToSiteRoot(fileId);
            return FileToAdam(dir);
        }

        public List<File<string, string>> GetFiles(IFolder folder)
        {
            var dir = Directory.GetFiles(AdjustPathToSiteRoot(folder.Path));
            return dir.Select(FileToAdam).ToList();
        }

        public void Rename(IFile file, string newName) => throw new NotImplementedException();

        public void Delete(IFile file) => throw new NotImplementedException();

        public File<string, string> Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName)
        {
            var callLog = Log.Call<File<string, string>>($"..., ..., {fileName}, {ensureUniqueName}");
            if (ensureUniqueName)
                fileName = FindUniqueFileName(parent, fileName);
            var fullContentPath = _adamPaths.PhysicalPath(parent.Path);
            Directory.CreateDirectory(fullContentPath);
            var filePath = Path.Combine(fullContentPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                body.CopyTo(stream);
            }
            var fileInfo = GetFile(filePath);

            return callLog("ok", fileInfo);
        }

        public void AddFolder(string path)
        {
            path = _adamPaths.PhysicalPath(path);
            Directory.CreateDirectory(path);
        }

        public bool FolderExists(string path)
        {
            var serverPath = _adamPaths.PhysicalPath(path);
            var exists = Directory.Exists(serverPath);
            return exists;
        }

        public Folder<string, string> GetFolder(string folderId) => AdamFolder(AdjustPathToSiteRoot(folderId));

        public List<Folder<string, string>> GetFolders(IFolder folder)
        {
            var dir = Directory.GetDirectories(AdjustPathToSiteRoot(folder.Path));
            return dir.Select(AdamFolder).ToList();
        }

        private string AdjustPathToSiteRoot(string path)
        {
            path = path.Backslash();
            return path.StartsWith("adam", StringComparison.CurrentCultureIgnoreCase)
                ? _adamPaths.PhysicalPath(path)
                : path;
        }

        public void Rename(IFolder folder, string newName) => throw new NotImplementedException();

        public void Delete(IFolder folder) => throw new NotImplementedException();

        public Folder<string, string> Get(string path) => AdamFolder(path);


        #region Helpers

        /// <summary>
        /// When uploading a new file, we must verify that the name isn't used. 
        /// If it is used, walk through numbers to make a new name which isn't used. 
        /// </summary>
        /// <param name="parentFolder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string FindUniqueFileName(IFolder parentFolder, string fileName)
        {
            var callLog = Log.Call<string>($"..., {fileName}");

            var folder = parentFolder;
            var name = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            for (var i = 1; i < MaxSameFileRetries && File.Exists(_adamPaths.PhysicalPath(Path.Combine(folder.Path, Path.GetFileName(fileName)))); i++)
                fileName = $"{name}-{i}{ext}";

            return callLog(fileName, fileName);
        }


        #endregion

        #region PathToAdam

        private Folder<string, string> AdamFolder(string path)
        {
            var f = new DirectoryInfo(_adamPaths.PhysicalPath(path));

            var relativePath = _adamPaths.PhysicalRelative(path);
            return new Folder<string, string>(AdamManager)
            {
                Path = relativePath,
                SysId = relativePath,
                ParentSysId = FindParentPath(path),
                Name = f.Name,
                Created = f.CreationTime,
                Modified = f.LastWriteTime,

                Url = _adamPaths.Url(relativePath),
            };
        }

        private static string FindParentPath(string path)
        {
            var cleanedPath = path.Forwardslash().TrimEnd('/');
            var lastSlash /*prevSlashF*/ = cleanedPath.LastIndexOf('/');
            //var prevSlashB = cleanedPath.LastIndexOf('\\');
            //var lastSlash = prevSlashB > prevSlashF ? prevSlashB : prevSlashF;
            var parentPath = lastSlash == -1 ? "" : cleanedPath.Substring(0, lastSlash);
            return parentPath;
        }


        private File<string, string> FileToAdam(string path)
        {
            var f = new FileInfo(_adamPaths.PhysicalPath(path));
            var directoryName = f.Directory.Name;

            var relativePath = _adamPaths.PhysicalRelative(path);

            return new File<string, string>(AdamManager)
            {
                FullName = f.Name,
                Extension = f.Extension.TrimStart('.'),
                Size = Convert.ToInt32(f.Length),
                SysId = relativePath,
                Folder = directoryName,
                ParentSysId = relativePath.Replace(f.Name, ""),
                Path = relativePath,

                Created = f.CreationTime,
                Modified = f.LastWriteTime,
                Name = Path.GetFileNameWithoutExtension(f.Name),
                Url =  _adamPaths.Url(relativePath),
            };
        }

        #endregion
    }
}
