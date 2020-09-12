using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcFileSystem: HasLog, IAdamFileSystem<string, string>
    {
        #region Constructor / DI / Init

        public MvcFileSystem(IHttp http) : base("Dnn.FilSys")
        {
            _http = http;
        }

        private readonly IHttp _http;

        public IAdamFileSystem<string, string> Init(AdamAppContext<string, string> adamContext)
        {
            AdamContext = adamContext;
            return this;
        }


        protected AdamAppContext<string, string> AdamContext;

        #endregion
        // #todo MVC
        public int MaxUploadKb() => 25000;

        public File<string, string> GetFile(string fileId)
        {
            throw new NotImplementedException();
        }

        public List<File<string, string>> GetFiles(IFolder folder)
        {
            var dir = Directory.GetFiles(folder.Path);
            return dir.Select(FileToAdam).ToList();
        }

        public void Rename(IFile file, string newName)
        {
            throw new NotImplementedException();
        }

        public void Delete(IFile file)
        {
            throw new NotImplementedException();
        }

        public File<string, string> Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName)
        {
            throw new NotImplementedException();
        }

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
            path = Path.Combine(AdamContext.Tenant.ContentPath, path);
            path = path.Replace("//", "/").Replace("\\\\", "\\");
            return _http.MapPath(path);
        }

        public Folder<string, string> GetFolder(string folderId)
        {
            throw new NotImplementedException();
        }

        public List<Folder<string, string>> GetFolders(IFolder folder)
        {
            var dir = Directory.GetDirectories(folder.Path);
            return dir.Select(AdamFolder).ToList();
        }

        public void Rename(IFolder folder, string newName)
        {
            throw new NotImplementedException();
        }

        public void Delete(IFolder folder)
        {
            throw new NotImplementedException();
        }

        public Folder<string, string> Get(string path) => AdamFolder(path);

        #region DnnToAdam
        private Folder<string, string> AdamFolder(string path)
        {
            var f = new DirectoryInfo(PathOnDrive(path));

            return new Folder<string, string>(AdamContext)
            {
                Path = path,
                SysId = "todo", // Constants.NullId,
                ParentSysId = "todo",// Constants.NullId,

                Name = f.Name,
                Created = f.CreationTime,
                Modified = f.LastWriteTime,

                Url = AdamContext.Tenant.ContentPath + path,
                // note: there are more properties in the DNN data, but we don't use it,
                // because it will probably never be cross-platform
            };
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
                SysId = "todo mvc", // Constants.NullId,
                Folder = directoryName,
                ParentSysId = "todo mvc",// Constants.NullId,

                Path = path,

                Created = f.CreationTime,
                Modified = f.LastWriteTime,
                Name = Path.GetFileNameWithoutExtension(f.Name),
                Url = // f.StorageLocation == 0 ?
                    AdamContext.Tenant.ContentPath + directoryName + f.Name
                    // : FileLinkClickController.Instance.GetFileLinkClick(f),
                // note: there are more properties in the DNN data, but we don't use it,
                // because it will probably never be cross-platform
            };
        }

        #endregion
    }
}
