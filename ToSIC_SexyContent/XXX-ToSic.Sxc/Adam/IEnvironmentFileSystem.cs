using System.Collections.Generic;

namespace ToSic.Sxc.Adam
{
    public interface IEnvironmentFileSystem
    {
        void AddFolder(int tenantId, string path);
        bool FolderExists(int tenantId, string path);
        List<File> GetFiles(int folderId, AdamAppContext appContext);
        List<Folder> GetFolders(int folderId, AdamAppContext appContext);

        Eav.Apps.Assets.Folder Get(int tenantId, string path, AdamAppContext appContext);
    }
}