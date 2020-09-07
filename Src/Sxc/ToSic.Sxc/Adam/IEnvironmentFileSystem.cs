using System.Collections.Generic;

namespace ToSic.Sxc.Adam
{
    public interface IEnvironmentFileSystem
    {
        IEnvironmentFileSystem Init(AdamAppContext adamContext);

        #region Files

        IFile GetFile(int fileId);

        List<File> GetFiles(int folderId);

        void Rename(IFile file, string newName);

        void Delete(IFile file);

        #endregion

        #region Folders

        void AddFolder(int tenantId, string path);
        bool FolderExists(int tenantId, string path);

        Folder GetFolder(int folderId);

        List<Folder> GetFolders(int folderId);

        void Rename(IFolder folder, string newName);

        void Delete(IFolder folder);

        #endregion


        Eav.Apps.Assets.Folder Get(int tenantId, string path);
    }
}