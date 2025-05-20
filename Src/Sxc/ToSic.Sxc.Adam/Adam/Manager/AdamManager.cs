using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.Context;
using ToSic.Eav.Metadata;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Adam.Manager;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Adam.Internal;

/// <summary>
/// The Manager of ADAM
/// In charge of managing assets inside this app - finding them, creating them etc.
/// </summary>
/// <remarks>
/// It's abstract, because there will be a typed implementation inheriting this
/// </remarks>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamManager: ServiceBase<AdamManager.MyServices>, ICompatibilityLevel
{
    #region MyServices

    public class MyServices(LazySvc<ICodeDataFactory> cdf, AdamConfiguration adamConfiguration,
        LazySvc<IAdamFileSystem> adamFsLazy, Generator<AdamStorageOfField> fieldStorageGenerator, AdamGenericHelper adamGenericHelper)
        : MyServicesBase(connect: [cdf, adamConfiguration, adamFsLazy, fieldStorageGenerator])
    {
        public LazySvc<ICodeDataFactory> Cdf { get; } = cdf;
        public AdamConfiguration AdamConfiguration { get; } = adamConfiguration;
        public LazySvc<IAdamFileSystem> AdamFsLazy { get; } = adamFsLazy;
        public Generator<AdamStorageOfField> FieldStorageGenerator { get; } = fieldStorageGenerator;
        public AdamGenericHelper AdamGenericHelper { get; } = adamGenericHelper;
    }

    #endregion

    #region Constructor and Services Setup

    public AdamManager(MyServices services) : base(services, "Adm.Managr")
    {
        // Note: Services are already connected in base class
        Services.Cdf.SetInit(obj => obj.SetFallbacks(AppContext?.Site, CompatibilityLevel, this));

        Services.AdamFsLazy.SetInit(f => f.Init(this));
    }

    #endregion

    #region Init

    public virtual AdamManager Init(IContextOfApp ctx, ICodeDataFactory cdf, int compatibility)
    {
        var l = Log.Fn<AdamManager>();
        AppContext = ctx;
        Site = ctx.Site;
        AppWorkCtx = new AppWorkCtx(ctx.AppReader);
        CompatibilityLevel = compatibility;
        Cdf = cdf;
        return l.Return(this, "ready");
    }

    public IAdamFileSystem AdamFs => field ??= Services.AdamFsLazy.Value;

    public IAppWorkCtx AppWorkCtx { get; private set; }

    public IContextOfApp AppContext { get; private set; }

    public ISite Site { get; private set; }

    internal ICodeDataFactory Cdf
    {
        get => field ??= Services.Cdf.Value;
        private set;
    }

    #endregion

    /// <summary>
    /// Path to the app assets
    /// </summary>
    public string Path => field ??= Services.AdamConfiguration.PathForApp(AppContext.AppReader.Specs);


    [PrivateApi]
    public int CompatibilityLevel { get; set; }

    #region Folder Stuff

    /// <summary>
    /// Root folder object of the app assets
    /// </summary>
    public IFolder RootFolder => Folder(Path, true);

    internal IFolder Folder(string path)
        => AdamFs.Get(path);

    internal IFolder Folder(string path, bool autoCreate)
    {
        var l = Log.Fn<IFolder>($"{path}, {autoCreate}");

        // create all folders to ensure they exist. Must do one-by-one because the environment must have it in the catalog
        var pathParts = path.Split('/');
        var pathToCheck = "";
        foreach (var part in pathParts.Where(p => !string.IsNullOrEmpty(p)))
        {
            pathToCheck += part + "/";
            if (AdamFs.FolderExists(pathToCheck))
                continue;
            if (autoCreate)
                AdamFs.AddFolder(pathToCheck);
            else
            {
                Log.A($"subfolder {pathToCheck} not found");
                return l.ReturnNull("not found");
            }
        }

        return l.ReturnAsOk(Folder(path));
    }

    public IFolder FolderOfField(Guid entityGuid, string fieldName, IField field = default)
    {
        var folderStorage = Services.FieldStorageGenerator.New().InitItemAndField(entityGuid, fieldName);
        folderStorage.Init(this);
        var folder = Services.AdamGenericHelper.FolderOfField(this, folderStorage, field);
        return folder;
    }
    #endregion

    #region Metadata Maker

    /// <summary>
    /// Get the first metadata entity of an item - or return a fake one instead
    /// </summary>
    internal IMetadata CreateMetadata(string key, string title, Action<IMetadataOf> mdInit = null)
    {
        var mdOf = AppWorkCtx.AppReader.Metadata.GetMetadataOf(TargetTypes.CmsItem, key, title: title);
        mdInit?.Invoke(mdOf);
        return Cdf.Metadata(mdOf);
    }

    #endregion
}