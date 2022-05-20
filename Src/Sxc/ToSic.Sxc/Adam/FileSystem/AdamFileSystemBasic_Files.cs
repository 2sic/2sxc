using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Adam
{
    public partial class AdamFileSystemBasic
    {
        /// <inheritdoc />
        public File<string, string> GetFile(string fileId)
        {
            var dir = EnsurePhysicalPath(fileId);
            return ToAdamFile(dir);
        }

        /// <inheritdoc />
        public List<File<string, string>> GetFiles(IFolder folder)
        {
            var dir = Directory.GetFiles(EnsurePhysicalPath(folder.Path));
            return dir.Select(ToAdamFile).ToList();
        }

        /// <inheritdoc />
        public void Rename(IFile file, string newName)
        {
            var callLog = Log.Call();
            TryToRenameFile(_adamPaths.PhysicalPath(file.Path), newName);
            callLog(null);
        }

        /// <inheritdoc />
        public void Delete(IFile file)
        {
            var callLog = Log.Call();
            File.Delete(_adamPaths.PhysicalPath(file.Path));
            callLog(null);
        }

        /// <inheritdoc />
        public File<string, string> Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName)
        {
            var callLog = Log.Fn<File<string, string>>($"..., ..., {fileName}, {ensureUniqueName}");
            if (ensureUniqueName) fileName = FindUniqueFileName(parent, fileName);
            var fullContentPath = _adamPaths.PhysicalPath(parent.Path);
            Directory.CreateDirectory(fullContentPath);
            var filePath = Path.Combine(fullContentPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                body.CopyTo(stream);
            }
            var fileInfo = GetFile(filePath);

            return callLog.Return(fileInfo, "ok");
        }


        protected bool TryToRenameFile(string originalWithPath, string newName)
        {
            var callLog = Log.Fn<bool>($"{newName}");
            
            if (!File.Exists(originalWithPath))
                return callLog.Return(false, $"Can't rename because source file does not exist {originalWithPath}");

            AdamPathsBase.ThrowIfPathContainsDotDot(newName);
            var path = FindParentPath(originalWithPath);
            var newFilePath = Path.Combine(path, newName);
            if (File.Exists(newFilePath))
                return callLog.Return(false, $"Can't rename because file with new name exists {newFilePath}");

            File.Move(originalWithPath, newFilePath);
            return callLog.Return(true, $"File renamed");
        }


        private static string FindParentPath(string path)
        {
            var cleanedPath = path.Backslash().TrimEnd('\\');
            var lastSlash = cleanedPath.LastIndexOf('\\');
            return lastSlash == -1 ? "" : cleanedPath.Substring(0, lastSlash);
        }

    }
}
