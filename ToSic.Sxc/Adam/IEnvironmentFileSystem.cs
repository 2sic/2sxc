using System.Collections.Generic;
using ToSic.Eav.Apps.Assets;

namespace ToSic.Sxc.Adam
{
    public interface IEnvironmentFileSystem
    {
        void AddFolder(int tenantId, string path);
        bool FolderExists(int tenantId, string path);
        List<AssetFile> GetFiles(int folderId, AdamAppContext appContext);
        List<AssetFolder> GetFolders(int folderId, AdamAppContext appContext);

        Folder Get(int tenantId, string path, AdamAppContext appContext);
    }
}