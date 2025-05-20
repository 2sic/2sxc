namespace ToSic.Sxc.Adam.Internal;

/// <summary>
/// WIP 
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IAdamFileSystem: IHasLog
{
    void Init(AdamManager adamManager);

    #region FileSystem Settings

    int MaxUploadKb();

    #endregion

    #region Files

    /// <summary>
    /// NEW WIP
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    IFile GetFile(AdamAssetIdentifier fileId);

    List<IFile> GetFiles(IFolder folder);

    void Rename(IFile file, string newName);

    void Delete(IFile file);

    IFile Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName);


    #endregion

    #region Folders

    /// <summary>
    /// Create a path (folder)
    /// </summary>
    /// <param name="path"></param>
    void AddFolder(string path);

    /// <summary>
    /// Verify that a path exists
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    bool FolderExists(string path);

    IFolder GetFolder(AdamAssetIdentifier folderId);

    List<IFolder> GetFolders(IFolder folder);


    void Rename(IFolder folder, string newName);

    void Delete(IFolder folder);

    #endregion

    IFolder Get(string path);

}