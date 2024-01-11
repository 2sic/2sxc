using System.IO;

namespace ToSic.Sxc.Adam.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IAdamFileSystem<TFolderId, TFileId>: IHasLog
{
    void Init(AdamManager<TFolderId, TFileId> adamManager);

    #region FileSystem Settings

    int MaxUploadKb();

    #endregion

    #region Files

    File<TFolderId, TFileId> GetFile(TFileId fileId);

    List<File<TFolderId, TFileId>> GetFiles(IFolder folder);

    void Rename(IFile file, string newName);

    void Delete(IFile file);

    File<TFolderId, TFileId> Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName);

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

    Folder<TFolderId, TFileId> GetFolder(TFolderId folderId);

    List<Folder<TFolderId, TFileId>> GetFolders(IFolder folder);

    void Rename(IFolder folder, string newName);

    void Delete(IFolder folder);

    #endregion

    Folder<TFolderId, TFileId> Get(string path);
}