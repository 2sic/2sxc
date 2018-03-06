using System.Collections.Generic;
using ToSic.Eav.Apps.Assets;

namespace ToSic.SexyContent.Adam
{
    public interface IEnvironmentFileSystem
    {
        void AddFolder(int tennantId, string path);
        bool FolderExists(int tennantId, string path);
        List<AdamFile> GetFiles(int folderId, AdamBrowseContext adamBrowseContext);
        List<AdamFolder> GetFolders(int folderId, AdamBrowseContext adamBrowseContext);

        Folder Get(int tennantId, string path, AdamBrowseContext fsh);
    }
}