using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.Context;
using ToSic.Eav.Metadata;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
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
public abstract class AdamManager: ServiceBase<AdamManager.MyServices>, ICompatibilityLevel
{
    #region MyServices

    public class MyServices(LazySvc<ICodeDataFactory> cdf, AdamConfiguration adamConfiguration)
        : MyServicesBase(connect: [cdf, adamConfiguration])
    {
        public LazySvc<ICodeDataFactory> Cdf { get; } = cdf;
        public AdamConfiguration AdamConfiguration { get; } = adamConfiguration;
    }

    #endregion

    #region Constructor for inheritance

    protected AdamManager(MyServices services, string logName, object[]? connect = null)
        : base(services, logName ?? "Adm.Managr", connect: connect)
    {
        // Note: Services are already connected in base class
        Services.Cdf.SetInit(obj => obj.SetFallbacks(AppContext?.Site, CompatibilityLevel, this));
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
    public IAdamFileSystem AdamFs { get; protected set; }

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

    internal /*Folder<TFolderId, TFileId>*/ IFolder Folder(string path)
        => AdamFs.Get(path);

    internal /*Folder<TFolderId, TFileId>*/ IFolder Folder(string path, bool autoCreate)
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


    #endregion

    #region Properties the base class already provides, but must be implemented at inheritance

    public abstract IFolder Folder(Guid entityGuid, string fieldName, IField field = default);


    public abstract IFile File(int id);

    public abstract IFolder Folder(int id);

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