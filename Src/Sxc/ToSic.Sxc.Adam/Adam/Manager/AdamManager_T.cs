using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Adam.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamManager<TFolderId, TFileId>: AdamManager
{
    private readonly Generator<AdamStorageOfField> _fieldStorageGenerator;
    private readonly LazySvc<IAdamFileSystem> _adamFsLazy;

    #region Constructor / DI
    public AdamManager(MyServices services, LazySvc<IAdamFileSystem> adamFsLazy, Generator<AdamStorageOfField> fieldStorageGenerator)
        : base(services, "Adm.MngrTT", connect: [adamFsLazy, fieldStorageGenerator])
    {
        _adamFsLazy = adamFsLazy.SetInit(f => f.Init(this));
        _fieldStorageGenerator = fieldStorageGenerator;
    }

    public override AdamManager Init(IContextOfApp ctx, ICodeDataFactory cdf, int compatibility)
    {
        base.Init(ctx, cdf, compatibility);
        AdamFs = _adamFsLazy.Value;
        return this;
    }


    #endregion

    ///// <summary>
    ///// Root folder object of the app assets
    ///// </summary>
    //public IFolder RootFolder => Folder(Path, true);

    ///// <summary>
    ///// Verify that a path exists
    ///// </summary>
    ///// <param name="path"></param>
    ///// <returns></returns>
    //internal bool Exists(string path) => AdamFs.FolderExists(path);

    ///// <summary>
    ///// Create a path (folder)
    ///// </summary>
    ///// <param name="path"></param>
    //internal void Add(string path) => AdamFs.AddFolder(path);

    //internal /*Folder<TFolderId, TFileId>*/ IFolder Folder(string path)
    //    => AdamFs.Get(path);

    //internal /*Folder<TFolderId, TFileId>*/ IFolder Folder(string path, bool autoCreate)
    //{
    //    var l = Log.Fn<IFolder>($"{path}, {autoCreate}");
            
    //    // create all folders to ensure they exist. Must do one-by-one because the environment must have it in the catalog
    //    var pathParts = path.Split('/');
    //    var pathToCheck = "";
    //    foreach (var part in pathParts.Where(p => !string.IsNullOrEmpty(p)))
    //    {
    //        pathToCheck += part + "/";
    //        if (Exists(pathToCheck)) continue;
    //        if (autoCreate)
    //            Add(pathToCheck);
    //        else
    //        {
    //            Log.A($"subfolder {pathToCheck} not found");
    //            return l.ReturnNull("not found");
    //        }
    //    }

    //    return l.ReturnAsOk(Folder(path));
    //}

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
            ? AdamFs.GetFile(AdamAssetIdentifier.Create(id)) 
            : null;

    // Note: Signature isn't great yet, as it's int, but theoretically it could be another type.
    public override IFolder Folder(int id) =>
        id is TFolderId fileId 
            ? AdamFs.GetFolder(AdamAssetIdentifier.Create(id)) 
            : null;

    #endregion
}