using ToSic.Eav.Apps;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Context.Internal;

/// <summary>
/// Runtime context information, used in dynamic code. Help the code to detect what environment it's in, what page etc.
/// This lets the code be platform-agnostic, so that it works across implementations (Dnn, Oqtane, NopCommerce)
/// </summary>
[PrivateApi("we only show the interface in the docs")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsContext(
    IPlatform platform,
    IContextOfSite initialContext,
    LazySvc<IPage> pageLazy,
    IAppReaderFactory appReaders,
    LazySvc<ICmsSite> cmsSiteLazy)
    : ServiceForDynamicCode(SxcLogName + ".CmsCtx",
        connect: [initialContext, pageLazy, appReaders, cmsSiteLazy, platform]), ICmsContext
{
    #region Constructor

    internal IContextOfSite CtxSite => _ctxSite.Get(() => CtxBlockOrNull ?? initialContext);
    private readonly GetOnce<IContextOfSite> _ctxSite = new();

    private IAppReader SiteAppState => _siteAppState ??= appReaders.GetZonePrimary(CtxSite.Site.ZoneId);
    private IAppReader _siteAppState;

    // Note: Internal so it can be used for View<T, T>
    internal IBlock RealBlockOrNull => _realBlock.Get(() => ((ICodeApiServiceInternal)_CodeApiSvc)?._Block);
    private readonly GetOnce<IBlock> _realBlock = new();

    internal IContextOfBlock CtxBlockOrNull => _ctxBlock.Get(() => ((ICodeApiServiceInternal)_CodeApiSvc)?._Block?.Context);
    private readonly GetOnce<IContextOfBlock> _ctxBlock = new();

    #endregion

    public ICmsPlatform Platform { get; } = platform;

    public ICmsSite Site => _site.Get(() => ((CmsSite)cmsSiteLazy.Value).Init(this, SiteAppState));
    private readonly GetOnce<ICmsSite> _site = new();

    public ICmsPage Page => _page ??= new CmsPage(this, SiteAppState.Metadata, pageLazy);
    private ICmsPage _page;

    public ICmsCulture Culture => _culture ??= new CmsCulture(this);
    private ICmsCulture _culture;

    public ICmsModule Module => _cmsModule ??= new CmsModule(this, RealBlockOrNull.Context?.Module ?? new ModuleUnknown(null), RealBlockOrNull);
    private ICmsModule _cmsModule;

    public ICmsUser User => _user ??= new CmsUser(this, SiteAppState.Metadata);
    private ICmsUser _user;

    public ICmsView View => _view ??= new CmsView(this, RealBlockOrNull);
    private ICmsView _view;

    public ICmsBlock Block => _cmsBlock ??= new CmsBlock(RealBlockOrNull);
    private ICmsBlock _cmsBlock;

}