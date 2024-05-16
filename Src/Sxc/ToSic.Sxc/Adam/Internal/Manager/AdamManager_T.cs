using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Sxc.Data;
using CodeDataFactory = ToSic.Sxc.Data.Internal.CodeDataFactory;

namespace ToSic.Sxc.Adam.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AdamManager<TFolderId, TFileId>: AdamManager
{
    private readonly Generator<AdamStorageOfField<TFolderId, TFileId>> _fieldStorageGenerator;
    private readonly LazySvc<IAdamFileSystem<TFolderId, TFileId>> _adamFsLazy;

    #region Constructor / DI
    public AdamManager(
        MyServices services,
        LazySvc<IAdamFileSystem<TFolderId, TFileId>> adamFsLazy,
        Generator<AdamStorageOfField<TFolderId, TFileId>> fieldStorageGenerator)
        : base(services, "Adm.MngrTT")
    {
        ConnectLogs([
            _adamFsLazy = adamFsLazy.SetInit(f => f.Init(this)),
            _fieldStorageGenerator = fieldStorageGenerator
        ]);
    }

    public override AdamManager Init(IContextOfApp ctx, CodeDataFactory cdf, int compatibility)
    {
        base.Init(ctx, cdf, compatibility);
        AdamFs = _adamFsLazy.Value;
        return this;
    }

    public IAdamFileSystem<TFolderId, TFileId> AdamFs { get; protected set; }

    #endregion

    /// <summary>
    /// Root folder object of the app assets
    /// </summary>
    public Folder<TFolderId, TFileId> RootFolder => Folder(Path, true);

    /// <summary>
    /// Verify that a path exists
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    internal bool Exists(string path) => AdamFs.FolderExists(path);

    /// <summary>
    /// Create a path (folder)
    /// </summary>
    /// <param name="path"></param>
    internal void Add(string path) => AdamFs.AddFolder(path);

    internal Folder<TFolderId, TFileId> Folder(string path) => AdamFs.Get(path);

    internal Folder<TFolderId, TFileId> Folder(string path, bool autoCreate)
    {
        var callLog = Log.Fn<Folder<TFolderId, TFileId>>($"{path}, {autoCreate}");
            
        // create all folders to ensure they exist. Must do one-by-one because the environment must have it in the catalog
        var pathParts = path.Split('/');
        var pathToCheck = "";
        foreach (var part in pathParts.Where(p => !string.IsNullOrEmpty(p)))
        {
            pathToCheck += part + "/";
            if (Exists(pathToCheck)) continue;
            if (autoCreate)
                Add(pathToCheck);
            else
            {
                Log.A($"subfolder {pathToCheck} not found");
                return callLog.ReturnNull("not found");
            }
        }

        return callLog.ReturnAsOk(Folder(path));
    }

    #region Type specific results which the base class already offers the interface to

    public Export<TFolderId, TFileId> Export => new(this);

    public override IFolder Folder(Guid entityGuid, string fieldName, IField field = default)
    {
        var folderStorage = _fieldStorageGenerator.New().InitItemAndField(entityGuid, fieldName);
        folderStorage.Init(this);
        var folder = new FolderOfField<TFolderId, TFileId>(this, folderStorage)
        {
            Field = field
        };
        return folder;
    }

    // Note: Signature isn't great yet, as it's int, but theoretically it could be another type.
    public override IFile File(int id) =>
        id is TFileId fileId 
            ? AdamFs.GetFile(fileId) 
            : null;

    // Note: Signature isn't great yet, as it's int, but theoretically it could be another type.
    public override IFolder Folder(int id) =>
        id is TFolderId fileId 
            ? AdamFs.GetFolder(fileId) 
            : null;

    #endregion
}