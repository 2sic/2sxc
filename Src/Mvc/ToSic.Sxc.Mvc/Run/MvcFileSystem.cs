using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Web;
using File = ToSic.Sxc.Adam.File;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcFileSystem: HasLog, IAdamFileSystem
    {
        #region Constructor / DI / Init

        public MvcFileSystem(IHttp http) : base("Dnn.FilSys")
        {
            _http = http;
        }

        private IHttp _http;

        public IAdamFileSystem Init(AdamAppContext adamContext)
        {
            AdamContext = adamContext;
            return this;
        }


        protected AdamAppContext AdamContext;

        #endregion
        // #todo MVC
        public int MaxUploadKb() => 25000;

        public IFile GetFile(int fileId)
        {
            throw new NotImplementedException();
        }

        public List<File> GetFiles(Eav.Apps.Assets.Folder folder)
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

        public IFile Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName)
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

        public Folder GetFolder(int folderId)
        {
            throw new NotImplementedException();
        }

        public List<Folder> GetFolders(Eav.Apps.Assets.Folder folder)
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

        public Folder Get(string path) => AdamFolder(path);

        #region DnnToAdam
        private Folder AdamFolder(string path)
        {
            var f = new DirectoryInfo(PathOnDrive(path));

            return new Folder(AdamContext)
            {
                Path = path,
                Id = Constants.NullId,

                ParentId = Constants.NullId,

                Name = f.Name,
                Created = f.CreationTime,
                Modified = f.LastWriteTime,

                Url = AdamContext.Tenant.ContentPath + path,
                // note: there are more properties in the DNN data, but we don't use it,
                // because it will probably never be cross-platform
            };
        }


        private File FileToAdam(string path)
        {
            var f = new FileInfo(PathOnDrive(path));
            var directoryName = f.Directory.Name;
            return new File(AdamContext)
            {
                FullName = f.Name,
                Extension = f.Extension,
                Size = Convert.ToInt32(f.Length),
                Id = Constants.NullId,
                Folder = directoryName,
                FolderId = Constants.NullId,

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
