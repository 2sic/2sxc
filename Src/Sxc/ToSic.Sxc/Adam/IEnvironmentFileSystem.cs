using System.Collections.Generic;
using System.IO;

namespace ToSic.Sxc.Adam
{
    public interface IEnvironmentFileSystem
    {
        IEnvironmentFileSystem Init(AdamAppContext adamContext);

        #region FileSystem Settings

        int MaxUploadKb();

        #endregion

        #region Files

        IFile GetFile(int fileId);

        List<File> GetFiles(int folderId);

        void Rename(IFile file, string newName);

        void Delete(IFile file);

        IFile Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName);

        ///// <summary>
        ///// When uploading a new file, we must verify that the name isn't used. 
        ///// If it is used, walk through numbers to make a new name which isn't used. 
        ///// </summary>
        ///// <param name="parentFolder"></param>
        ///// <param name="fileName"></param>
        ///// <returns></returns>
        //string FindUniqueFileName(IFolder parentFolder, string fileName);

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