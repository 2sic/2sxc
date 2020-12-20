using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Eav.Helpers;

namespace ToSic.Sxc.Adam
{
    public partial class AdamFileSystemBasic
    {
        public File<string, string> GetFile(string fileId)
        {
            var dir = EnsurePhysicalPath(fileId);
            return ToAdamFile(dir);
        }

        public List<File<string, string>> GetFiles(IFolder folder)
        {
            var dir = Directory.GetFiles(EnsurePhysicalPath(folder.Path));
            return dir.Select(ToAdamFile).ToList();
        }

        public void Rename(IFile file, string newName)
        {
            var callLog = Log.Call();
            TryToRenameFile(_adamPaths.PhysicalPath(file.Path), newName);
            callLog(null);
        }


        public void Delete(IFile file)
        {
            var callLog = Log.Call();
            File.Delete(_adamPaths.PhysicalPath(file.Path));
            callLog(null);
        }

        public File<string, string> Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName)
        {
            var callLog = Log.Call<File<string, string>>($"..., ..., {fileName}, {ensureUniqueName}");
            if (ensureUniqueName) fileName = FindUniqueFileName(parent, fileName);
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


        protected bool TryToRenameFile(string originalWithPath, string newName)
        {
            var callLog = Log.Call<bool>($"{newName}");

            if (!File.Exists(originalWithPath))
                return callLog($"Can't rename because source file does not exist {originalWithPath}", false);

            AdamPathsBase.ThrowIfPathContainsDotDot(newName);
            var path = FindParentPath(originalWithPath);
            var newFilePath = Path.Combine(path, newName);
            if (File.Exists(newFilePath))
                return callLog($"Can't rename because file with new name exists {newFilePath}", false);

            File.Move(originalWithPath, newFilePath);
            return callLog($"File renamed", true);
        }


        private static string FindParentPath(string path)
        {
            var cleanedPath = path.Backslash().TrimEnd('\\');
            var lastSlash = cleanedPath.LastIndexOf('\\');
            return lastSlash == -1 ? "" : cleanedPath.Substring(0, lastSlash);
        }

    }
}
