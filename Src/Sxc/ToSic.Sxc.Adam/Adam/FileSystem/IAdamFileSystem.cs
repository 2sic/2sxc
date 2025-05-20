namespace ToSic.Sxc.Adam.Internal;

/// <summary>
/// WIP 
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IAdamFileSystem: IHasLog
{
    //void Init(AdamManager<TFolderId, TFileId> adamManager);

    #region FileSystem Settings

    int MaxUploadKb();

    #endregion

    #region Files

    //File<TFolderId, TFileId> GetFile(TFileId fileId);

    /// <summary>
    /// NEW WIP
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    IFile GetFile(AdamAssetIdentifier fileId);

    //List<File<TFolderId, TFileId>> GetFiles(IFolder folder);
    List<IFile> GetFiles(IFolder folder);

    void Rename(IFile file, string newName);

    void Delete(IFile file);

    IFile Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName);


    #endregion

    #region Folders

    void AddFolder(string path);
    bool FolderExists(string path);

    //Folder<TFolderId, TFileId> GetFolder(TFolderId folderId);
    IFolder GetFolder(AdamAssetIdentifier folderId);

    //List<Folder<TFolderId, TFileId>> GetFolders(IFolder folder);
    List<IFolder> GetFolders(IFolder folder);


    void Rename(IFolder folder, string newName);

    void Delete(IFolder folder);

    #endregion

    //Folder<TFolderId, TFileId> Get(string path);
    IFolder Get(string path);

}