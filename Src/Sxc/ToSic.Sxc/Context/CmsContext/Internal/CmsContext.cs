using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Services;
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
    IContextOfSite siteCtxFallback,
    LazySvc<IPage> pageLazy,
    IAppReaderFactory appReaders,
    LazySvc<ICmsSite> cmsSiteLazy)
    : ServiceForDynamicCode(SxcLogName + ".CmsCtx",
        connect: [siteCtxFallback, pageLazy, appReaders, cmsSiteLazy, platform]), ICmsContext
{
    #region Internal context

    // Note: Internal so it can be used for View<T, T>
    internal IBlock RealBlockOrNull => _realBlock.Get(() => ((ICodeApiServiceInternal)_CodeApiSvc)?._Block);
    private readonly GetOnce<IBlock> _realBlock = new();

    internal IContextOfBlock CtxBlockOrNull => _ctxBlock.Get(() => RealBlockOrNull?.Context);
    private readonly GetOnce<IContextOfBlock> _ctxBlock = new();

    internal IContextOfSite CtxSite => CtxBlockOrNull ?? siteCtxFallback;

    private IAppReader SiteAppState => field ??= appReaders.GetZonePrimary(CtxSite.Site.ZoneId);

    #endregion

    public ICmsPlatform Platform { get; } = platform;

    public ICmsSite Site => _site.Get(() => ((CmsSite)cmsSiteLazy.Value).Init(this, SiteAppState));
    private readonly GetOnce<ICmsSite> _site = new();

    public ICmsPage Page => field ??= new CmsPage(this, SiteAppState.Metadata, pageLazy);

    public ICmsCulture Culture => field ??= new CmsCulture(this);

    public ICmsModule Module => field ??= new CmsModule(this, RealBlockOrNull.Context?.Module ?? new ModuleUnknown(null), RealBlockOrNull);

    public ICmsUser User => field ??= CreateCurrent();

    private ICmsUser CreateCurrent()
    {
        var userSvc = _CodeApiSvc.GetService<IUserService>(reuse: true);
        var userModel = userSvc?.GetCurrentUser();
        return new CmsUser(this, userModel, SiteAppState.Metadata);
    }

    public ICmsView View => field ??= new CmsView(this, RealBlockOrNull);

    public ICmsBlock Block => field ??= new CmsBlock(RealBlockOrNull);
}