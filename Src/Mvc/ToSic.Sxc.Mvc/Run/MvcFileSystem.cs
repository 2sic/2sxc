using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Mvc.Web;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcFileSystem: HasLog, IAdamFileSystem<string, string>
    {
        private readonly IServerPaths _serverPaths;

        #region Constructor / DI / Init

        public MvcFileSystem(IServerPaths serverPaths) : base("Dnn.FilSys") => _serverPaths = serverPaths;

        public IAdamFileSystem<string, string> Init(AdamManager<string, string> adamContext, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            AdamContext = adamContext;
            return this;
        }


        protected AdamManager<string, string> AdamContext;

        #endregion
        // #todo MVC
        public int MaxUploadKb() => 25000;

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

        public File<string, string> Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName) => throw new NotImplementedException();

        public void AddFolder(string path)
        {
            path = PathOnDrive(path);
            Directory.CreateDirectory(path);
        }

        public bool FolderExists(string path)
        {
            var serverPath = PathOnDrive(path);
            var exists = Directory.Exists(serverPath);
            return exists;
        }

        private string PathOnDrive(string path)
        {
            if (path.Contains("..")) throw new ArgumentException("path may not contain ..", nameof(path));
            // check if it already has the root path attached, otherwise add
            path = path.StartsWith(AdamContext.Site.ContentPath) ? path : Path.Combine(AdamContext.Site.ContentPath, path);
            path = path.Replace("//", "/").Replace("\\\\", "\\");
            return _serverPaths.FullContentPath(path);
        }

        public Folder<string, string> GetFolder(string folderId) => AdamFolder(AdjustPathToSiteRoot(folderId));

        public List<Folder<string, string>> GetFolders(IFolder folder)
        {
            var dir = Directory.GetDirectories(AdjustPathToSiteRoot(folder.Path));
            return dir.Select(AdamFolder).ToList();
        }

        private static string AdjustPathToSiteRoot(string path)
        {
            return path.StartsWith("adam", StringComparison.CurrentCultureIgnoreCase)
                ? Path.Combine(MvcConstants.WwwRoot, path)
                : path;
        }

        public void Rename(IFolder folder, string newName) => throw new NotImplementedException();

        public void Delete(IFolder folder) => throw new NotImplementedException();

        public Folder<string, string> Get(string path) => AdamFolder(path);

        #region DnnToAdam
        private Folder<string, string> AdamFolder(string path)
        {
            var f = new DirectoryInfo(PathOnDrive(path));

            return new Folder<string, string>(AdamContext)
            {
                Path = path,
                SysId = path,
                ParentSysId = FindParentPath(path),
                Name = f.Name,
                Created = f.CreationTime,
                Modified = f.LastWriteTime,

                Url = AdamContext.Site.ContentPath + path,
            };
        }

        private static string FindParentPath(string path)
        {
            var cleanedPath = path.TrimEnd('/').TrimEnd('\\');
            var prevSlashF = cleanedPath.LastIndexOf('/');
            var prevSlashB = cleanedPath.LastIndexOf('\\');
            var lastSlash = prevSlashB > prevSlashF ? prevSlashB : prevSlashF;
            var parentPath = lastSlash == -1 ? "" : cleanedPath.Substring(0, lastSlash);
            return parentPath;
        }


        private File<string, string> FileToAdam(string path)
        {
            var f = new FileInfo(PathOnDrive(path));
            var directoryName = f.Directory.Name;
            return new File<string, string>(AdamContext)
            {
                FullName = f.Name,
                Extension = f.Extension,
                Size = Convert.ToInt32(f.Length),
                SysId = path,
                Folder = directoryName,
                ParentSysId = path.Replace(f.Name, "", StringComparison.InvariantCultureIgnoreCase), // "todo mvc",// Constants.NullId,

                Path = path,

                Created = f.CreationTime,
                Modified = f.LastWriteTime,
                Name = Path.GetFileNameWithoutExtension(f.Name),
                Url = AdamContext.Site.ContentPath + directoryName + f.Name
            };
        }

        #endregion
    }
}
