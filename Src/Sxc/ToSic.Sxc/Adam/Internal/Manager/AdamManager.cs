using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.Context;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data;
using CodeDataFactory = ToSic.Sxc.Data.Internal.CodeDataFactory;

namespace ToSic.Sxc.Adam.Internal;

/// <summary>
/// The Manager of ADAM
/// In charge of managing assets inside this app - finding them, creating them etc.
/// </summary>
/// <remarks>
/// It's abstract, because there will be a typed implementation inheriting this
/// </remarks>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class AdamManager: ServiceBase<AdamManager.MyServices>, ICompatibilityLevel
{
    #region MyServices

    public class MyServices: MyServicesBase
    {
        public LazySvc<CodeDataFactory> Cdf { get; }
        public AdamConfiguration AdamConfiguration { get; }

        public MyServices(LazySvc<CodeDataFactory> cdf, AdamConfiguration adamConfiguration)
        {
            ConnectLogs([
                Cdf = cdf,
                AdamConfiguration = adamConfiguration
            ]);
        }
    }

    #endregion

    #region Constructor for inheritance

    protected AdamManager(MyServices services, string logName) : base(services, logName ?? "Adm.Managr")
    {
        // Note: Services are already connected in base class
        Services.Cdf.SetInit(obj => obj.SetFallbacks(AppContext?.Site, CompatibilityLevel, this));
    }

    #endregion

    #region Init

    public virtual AdamManager Init(IContextOfApp ctx, CodeDataFactory cdf, int compatibility)
    {
        var l = Log.Fn<AdamManager>();
        AppContext = ctx;
        Site = AppContext.Site;
        AppWorkCtx = new AppWorkCtx(AppContext.AppReader);
        CompatibilityLevel = compatibility;
        _cdf = cdf;
        return l.Return(this, "ready");
    }
        
    public IAppWorkCtx AppWorkCtx { get; private set; }

    public IContextOfApp AppContext { get; private set; }

    public ISite Site { get; private set; }

    internal CodeDataFactory Cdf => _cdf ??= Services.Cdf.Value;
    private CodeDataFactory _cdf;
    #endregion

    #region Static Helpers

    public static int? CheckIdStringForId(string id)
    {
        if (!id.HasValue()) return null;
        var linkParts = new LinkParts(id);
        if (!linkParts.IsMatch || linkParts.Id == 0) return null;
        return linkParts.Id;
    }


    #endregion

    /// <summary>
    /// Path to the app assets
    /// </summary>
    public string Path => _path ??= Services.AdamConfiguration.PathForApp(AppContext.AppReader.Specs);
    private string _path;


    [PrivateApi]
    public int CompatibilityLevel { get; set; }


    #region Properties the base class already provides, but must be implemented at inheritance

    public abstract IFolder Folder(Guid entityGuid, string fieldName, IField field = default);


    public abstract IFile File(int id);

    public abstract IFolder Folder(int id);

    #endregion

    #region Metadata Maker

    /// <summary>
    /// Get the first metadata entity of an item - or return a fake one instead
    /// </summary>
    internal IMetadata Create(string key, string title, Action<IMetadataOf> mdInit = null)
    {
        var mdOf = AppWorkCtx.AppReader.Metadata.GetMetadataOf(TargetTypes.CmsItem, key, title: title);
        mdInit?.Invoke(mdOf);
        return Cdf.Metadata(mdOf);
    }

    #endregion
}