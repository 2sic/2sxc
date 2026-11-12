using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Ui.Context.Sys;
using ToSic.Eav.Ui.Context.Sys.Dto;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Ui.Context.Sys;

/// <remarks>
/// This is `sealed` on purpose, to encourage composition over inheritance.
/// If you want to implement your own version, implement the interface instead of inheriting from this class.
///
/// This service is registered both as the default <see cref="IUiContextPartsService"/>
/// but also with its own name <see cref="SxcUiContextPartsService"/>.
/// Overriding implementations would use <see cref="SxcUiContextPartsService"/> as a dependency,
/// and pre-register as the default <see cref="IUiContextPartsService"/> to override the default implementation.
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
[method: PrivateApi]
public sealed class SxcUiContextPartsService(
    Generator<EavUiContextPartsService, IAppReader> uiDefaults,
    IContextOfSiteService siteCtxSvc,
    IAppPathsMicroSvc appPaths,
    LazySvc<GlobalPaths> globalPaths,
    LazySvc<IUserService> usersSvc)
    : ServiceBase("Sxc.CtxPrt", connect: [uiDefaults, siteCtxSvc, appPaths, globalPaths, usersSvc]), IUiContextPartsService
{
    public void Setup(IAppReader? options) => AppReader = options;

    private IAppReader? AppReader { get; set; }

    private IUiContextPartsService UiDefaults => field ??= uiDefaults.New(AppReader!);

    private ISite Site => field ??= siteCtxSvc.Context.Site;
    
    public ContextLanguageInfoDto? GetLanguage(Ctx flags) => UiDefaults.GetLanguage(flags);

    public ContextResourceWithApp? GetSystem(Ctx flags) => UiDefaults.GetSystem(flags);

    public ContextResourceWithApp? GetSite(Ctx flags) => UiDefaults.GetSite(flags);

    public WebResourceDto GetPage(Ctx flags) => UiDefaults.GetPage(flags);

    public ContextEnableDto GetEnable(CtxEnable ctx) => UiDefaults.GetEnable(ctx);

    public ContextAppDto? GetApp(Ctx flags)
    {
        var result = UiDefaults.GetApp(flags);
        if (result == null || AppReader == null)
            return null;
        var paths = appPaths.Get(AppReader, Site);
        return result with
        {
            Icon = AppThumbnailHelper.GetUrl(AppReader, paths!, globalPaths)
        };
    }

    public ContextUserDto? GetUser(Ctx flags)
    {
        var result = UiDefaults.GetUser(flags);
        if (result == null || AppReader == null)
            return null;

        return result with
        {
            Roles = flags.HasFlag(Ctx.UserRoles) ? usersSvc.Value.GetCurrentUser().Roles.Select(r => r.Name).ToListOpt() : [],
        };
    }

    public IList<FeatureDto> GetFeatures(Ctx flags) => UiDefaults.GetFeatures(flags);
}