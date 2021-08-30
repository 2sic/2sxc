using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ToSic.Sxc.Adam
{
    public partial class AdamFileSystemBasic
    {
        /// <inheritdoc />
        public void AddFolder(string path)
        {
            path = _adamPaths.PhysicalPath(path);
            Directory.CreateDirectory(path);
        }

        /// <inheritdoc />
        public bool FolderExists(string path)
        {
            var serverPath = _adamPaths.PhysicalPath(path);
            return Directory.Exists(serverPath);
        }

        /// <inheritdoc />
        public Folder<string, string> GetFolder(string folderId) => ToAdamFolder(EnsurePhysicalPath(folderId));

        /// <inheritdoc />
        public List<Folder<string, string>> GetFolders(IFolder folder)
        {
            var dir = Directory.GetDirectories(EnsurePhysicalPath(folder.Path));
            return dir.Select(ToAdamFolder).ToList();
        }


        /// <inheritdoc />
        public void Rename(IFolder folder, string newName) => throw new NotSupportedException();

        /// <inheritdoc />
        public void Delete(IFolder folder) => throw new NotSupportedException();

        /// <inheritdoc />
        public Folder<string, string> Get(string path) => ToAdamFolder(path);

    }
}
