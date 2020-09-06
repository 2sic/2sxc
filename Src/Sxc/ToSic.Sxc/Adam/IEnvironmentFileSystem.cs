using System.Collections.Generic;

namespace ToSic.Sxc.Adam
{
    public interface IEnvironmentFileSystem
    {
        IEnvironmentFileSystem Init(AdamAppContext adamContext);

        void AddFolder(int tenantId, string path);
        bool FolderExists(int tenantId, string path);
        List<File> GetFiles(int folderId/*, AdamAppContext appContext*/);

        #region Folders

        Folder GetFolder(int folderId);
        List<Folder> GetFolders(int folderId/*, AdamAppContext appContext*/);
        #endregion
        Eav.Apps.Assets.Folder Get(int tenantId, string path/*, AdamAppContext appContext*/);
    }
}