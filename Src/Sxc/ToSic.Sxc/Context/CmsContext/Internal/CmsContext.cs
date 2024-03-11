using ToSic.Eav.Apps;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Context.Internal;

/// <summary>
/// Runtime context information, used in dynamic code. Help the code to detect what environment it's in, what page etc.
/// This lets the code be platform-agnostic, so that it works across implementations (Dnn, Oqtane, NopCommerce)
/// </summary>
[PrivateApi("we only show the interface in the docs")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsContext: ServiceForDynamicCode, ICmsContext
{
    #region Constructor

    /// <summary>
    /// DI Constructor
    /// </summary>
    public CmsContext(
        IPlatform platform, 
        IContextOfSite initialContext, 
        LazySvc<IPage> pageLazy,
        IAppStates appStates,
        LazySvc<ICmsSite> cmsSiteLazy
    ) : base(SxcLogging.SxcLogName + ".CmsCtx")
    {
        ConnectServices(
            _initialContext = initialContext,
            _pageLazy = pageLazy,
            _appStates = appStates,
            _cmsSiteLazy = cmsSiteLazy,
            Platform = platform
        );
    }
    private readonly IContextOfSite _initialContext;
    private readonly LazySvc<IPage> _pageLazy;

    internal IContextOfSite CtxSite => _ctxSite.Get(() => CtxBlockOrNull ?? _initialContext);
    private readonly GetOnce<IContextOfSite> _ctxSite = new();

    private readonly IAppStates _appStates;
    private readonly LazySvc<ICmsSite> _cmsSiteLazy;

    private IAppStateInternal SiteAppState => _siteAppState ??= _appStates.GetPrimaryReader(CtxSite.Site.ZoneId, Log);
    private IAppStateInternal _siteAppState;

    // Note: Internal so it can be used for View<T, T>
    internal IBlock RealBlockOrNull => _realBlock.Get(() => ((ICodeApiServiceInternal)_CodeApiSvc)?._Block);
    private readonly GetOnce<IBlock> _realBlock = new();

    internal IContextOfBlock CtxBlockOrNull => _ctxBlock.Get(() => ((ICodeApiServiceInternal)_CodeApiSvc)?._Block?.Context);
    private readonly GetOnce<IContextOfBlock> _ctxBlock = new();

    #endregion

    public ICmsPlatform Platform { get; }

    public ICmsSite Site => _site.Get(() => ((CmsSite)_cmsSiteLazy.Value).Init(this, SiteAppState));
    private readonly GetOnce<ICmsSite> _site = new();

    public ICmsPage Page => _page ??= new CmsPage(this, SiteAppState, _pageLazy);
    private ICmsPage _page;

    public ICmsCulture Culture => _culture ??= new CmsCulture(this);
    private ICmsCulture _culture;

    public ICmsModule Module => _cmsModule ??= new CmsModule(this, RealBlockOrNull.Context?.Module ?? new ModuleUnknown(null), RealBlockOrNull);
    private ICmsModule _cmsModule;

    public ICmsUser User => _user ??= new CmsUser(this, SiteAppState);
    private ICmsUser _user;

    public ICmsView View => _view ??= new CmsView(this, RealBlockOrNull);
    private ICmsView _view;

    public ICmsBlock Block => _cmsBlock ??= new CmsBlock(RealBlockOrNull);
    private ICmsBlock _cmsBlock;

}