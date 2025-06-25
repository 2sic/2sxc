using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context.Sys;
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
    : ServiceWithContext(SxcLogName + ".CmsCtx",
        connect: [siteCtxFallback, pageLazy, appReaders, platform]), ICmsContext
{
    #region Internal context

    /// <summary>
    /// Provide the Execution Context to the parts
    /// </summary>
    internal new IExecutionContext ExCtx => base.ExCtx;

    // Note: Internal so it can be used for View<T, T>
    internal IBlock BlockInternal => _realBlock.Get(() => ExCtx.GetState<IBlock>())!;
    private readonly GetOnce<IBlock?> _realBlock = new();

    internal IContextOfBlock? CtxBlockOrNull => _ctxBlock.Get(() => BlockInternal.Context);
    private readonly GetOnce<IContextOfBlock?> _ctxBlock = new();

    internal IContextOfSite CtxSite => CtxBlockOrNull ?? siteCtxFallback;

    [field: AllowNull, MaybeNull]
    private IAppReader SiteAppReader => field ??= appReaders.GetZonePrimary(CtxSite.Site.ZoneId);

    #endregion

    public ICmsPlatform Platform { get; } = platform;

    [field: AllowNull, MaybeNull]
    public ICmsSite Site => field
        ??= new CmsSite(this, SiteAppReader);

    [field: AllowNull, MaybeNull]
    public ICmsPage Page => field
        ??= new CmsPage(this, SiteAppReader.Metadata, pageLazy);

    [field: AllowNull, MaybeNull]
    public ICmsCulture Culture => field
        ??= new CmsCulture(this);

    [field: AllowNull, MaybeNull]
    public ICmsModule Module => field
        ??= new CmsModule(this, BlockInternal.Context?.Module ?? new ModuleUnknown(null!), BlockInternal);

    [field: AllowNull, MaybeNull]
    public ICmsUser User => field
        ??= CreateCurrent();

    private ICmsUser CreateCurrent()
    {
        var userSvc = ExCtx.GetService<IUserService>(reuse: true);
        var userModel = userSvc.GetCurrentUser();
        return new CmsUser(this, userModel, SiteAppReader.Metadata);
    }

    [field: AllowNull, MaybeNull]
    public ICmsView View => field
        ??= new CmsView(this, BlockInternal);

    [field: AllowNull, MaybeNull]
    public ICmsBlock Block => field
        ??= new CmsBlock(BlockInternal);
}