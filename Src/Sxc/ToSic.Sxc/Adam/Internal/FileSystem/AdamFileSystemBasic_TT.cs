using System.IO;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Helpers;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Adam.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class AdamFileSystemBasic<TFolder, TFile>: ServiceBase, IAdamFileSystem<TFolder, TFile>
{
    #region Setup

    protected AdamFileSystemBasic(IAdamPaths adamPaths, string logPrefix) : base($"{logPrefix}.FilSys")
    {
        ConnectLogs([
            _adamPaths = adamPaths,
            FsHelpers = new(adamPaths)
        ]);
    }

    protected readonly AdamFileSystemHelpers FsHelpers;
    protected readonly IAdamPaths _adamPaths;

    public void Init(AdamManager<TFolder, TFile> adamManager)
    {
        var l = Log.Fn();
        AdamManager = adamManager;
        _adamPaths.Init(adamManager);
        l.Done();
    }

    protected AdamManager<TFolder, TFile> AdamManager;

    #endregion



    /// <inheritdoc />
    public virtual void Rename(IFile file, string newName) => Log.Do(() => FsHelpers.TryToRenameFile(_adamPaths.PhysicalPath(file.Path), newName));

    /// <inheritdoc />
    public virtual void Delete(IFile file) => Log.Do(() => File.Delete(_adamPaths.PhysicalPath(file.Path)));


    public int MaxUploadKb() => AdamConstants.MaxUploadKbDefault;
    public abstract File<TFolder, TFile> GetFile(TFile fileId);
    public abstract List<File<TFolder, TFile>> GetFiles(IFolder folder);

    public abstract File<TFolder, TFile> Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName);
    public abstract void AddFolder(string path);
    public abstract bool FolderExists(string path);
    public abstract Folder<TFolder, TFile> GetFolder(TFolder folderId);
    public abstract List<Folder<TFolder, TFile>> GetFolders(IFolder folder);
    public abstract void Rename(IFolder folder, string newName);
    public abstract void Delete(IFolder folder);
    public abstract Folder<TFolder, TFile> Get(string path);
    public string GetUrl(string folderPath) => _adamPaths.Url(folderPath.ForwardSlash());

}