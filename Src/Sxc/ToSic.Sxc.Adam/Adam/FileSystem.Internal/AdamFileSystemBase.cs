using ToSic.Eav.Apps.Internal;
using ToSic.Lib.Services;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Adam.Manager.Internal;
using ToSic.Sxc.Adam.Paths.Internal;

namespace ToSic.Sxc.Adam.FileSystem.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class AdamFileSystemBase : ServiceBase, IAdamFileSystem
{
    #region Setup

    protected AdamFileSystemBase(IAdamPaths adamPaths, string logPrefix, object[]? connect = default)
        : base($"{logPrefix}.FilSys", connect: [adamPaths, ..connect ?? []])
    {
        AdamPaths = adamPaths;
        ConnectLogs([
            FsHelpers = new(adamPaths)
        ]);
    }

    protected readonly AdamFileSystemHelpers FsHelpers;
    protected readonly IAdamPaths AdamPaths;

    public void Init(AdamManager adamManager)
    {
        var l = Log.Fn();
        AdamManager = adamManager;
        AdamPaths.Init(adamManager);
        l.Done();
    }

    protected AdamManager AdamManager;

    #endregion



    /// <inheritdoc />
    public virtual void Rename(IFile file, string newName)
        => Log.Do(() => FsHelpers.TryToRenameFile(AdamPaths.PhysicalPath(file.Path), newName));

    /// <inheritdoc />
    public virtual void Delete(IFile file) => Log.Do(() => File.Delete(AdamPaths.PhysicalPath(file.Path)));


    public int MaxUploadKb() => AdamConstants.MaxUploadKbDefault;
    //public abstract File<TFolder, TFile> GetFile(TFile fileId);

    public abstract IFile GetFile(AdamAssetIdentifier fileId);

    public abstract List<IFile> GetFiles(IFolder folder);

    public abstract IFile Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName);
    public abstract void AddFolder(string path);
    public abstract bool FolderExists(string path);
    //public abstract Folder<TFolder, TFile> GetFolder(TFolder folderId);
    public abstract IFolder GetFolder(AdamAssetIdentifier folderId);

    public abstract List<IFolder> GetFolders(IFolder folder);
    public abstract void Rename(IFolder folder, string newName);
    public abstract void Delete(IFolder folder);
    public abstract IFolder Get(string path);
    public string GetUrl(string folderPath) => AdamPaths.Url(folderPath.ForwardSlash());

}