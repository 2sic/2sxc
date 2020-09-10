using System.Collections.Generic;
using System.IO;

namespace ToSic.Sxc.Adam
{
    public interface IAdamFileSystem
    {
        IAdamFileSystem Init(AdamAppContext adamContext);

        #region FileSystem Settings

        int MaxUploadKb();

        #endregion

        #region Files

        IFile GetFile(int fileId);

        List<IFile> GetFiles(IFolder folder);

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

        void AddFolder(string path);
        bool FolderExists(string path);

        Folder GetFolder(int folderId);

        List<Folder> GetFolders(IFolder folder);

        void Rename(IFolder folder, string newName);

        void Delete(IFolder folder);

        #endregion


        Folder Get(string path);
    }
}