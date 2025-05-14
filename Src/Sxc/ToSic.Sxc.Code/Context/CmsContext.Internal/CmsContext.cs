using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Context.Internal;

/// <summary>
/// Runtime context information, used in dynamic code. Help the code to detect what environment it's in, what page etc.
/// This lets the code be platform-agnostic, so that it works across implementations (Dnn, Oqtane, NopCommerce)
/// </summary>
[PrivateApi("we only show the interface in the docs")]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CmsContext(
    IPlatform platform,
    IContextOfSite siteCtxFallback,
    LazySvc<IPage> pageLazy,
    IAppReaderFactory appReaders)
    : ServiceForDynamicCode(SxcLogName + ".CmsCtx",
        connect: [siteCtxFallback, pageLazy, appReaders, platform]), ICmsContext
{
    #region Internal context

    // Note: Internal so it can be used for View<T, T>
    internal IBlock RealBlockOrNull => _realBlock.Get(() => ((IExCtxBlock)_CodeApiSvc)?.Block);
    private readonly GetOnce<IBlock> _realBlock = new();

    internal IContextOfBlock CtxBlockOrNull => _ctxBlock.Get(() => RealBlockOrNull?.Context);
    private readonly GetOnce<IContextOfBlock> _ctxBlock = new();

    internal IContextOfSite CtxSite => CtxBlockOrNull ?? siteCtxFallback;

    private IAppReader SiteAppReader => field ??= appReaders.GetZonePrimary(CtxSite.Site.ZoneId);

    #endregion

    public ICmsPlatform Platform { get; } = platform;

    public ICmsSite Site => field ??= new CmsSite(this, SiteAppReader);

    public ICmsPage Page => field ??= new CmsPage(this, SiteAppReader.Metadata, pageLazy);

    public ICmsCulture Culture => field ??= new CmsCulture(this);

    public ICmsModule Module => field ??= new CmsModule(this, RealBlockOrNull.Context?.Module ?? new ModuleUnknown(null), RealBlockOrNull);

    public ICmsUser User => field ??= CreateCurrent();

    private ICmsUser CreateCurrent()
    {
        var userSvc = _CodeApiSvc.GetService<IUserService>(reuse: true);
        var userModel = userSvc?.GetCurrentUser();
        return new CmsUser(this, userModel, SiteAppReader.Metadata);
    }

    public ICmsView View => field ??= new CmsView(this, RealBlockOrNull);

    public ICmsBlock Block => field ??= new CmsBlock(RealBlockOrNull);
}