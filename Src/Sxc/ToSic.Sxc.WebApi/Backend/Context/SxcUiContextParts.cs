using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Sxc.Apps.Sys.Assets;
using ToSic.Sxc.Apps.Sys.Paths;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Backend.Context;

public class SxcUiContextParts(
    Generator<UiContextPartsEav, IAppReader?> uiDefaults,
    IContextOfSiteService siteCtxSvc,
    IAppPathsMicroSvc appPaths,
    LazySvc<GlobalPaths> globalPaths,
    LazySvc<IUserService> usersSvc)
    : ServiceWithSetup<IAppReader?>("Sxc.CtxPrt", connect: [uiDefaults, siteCtxSvc, appPaths, globalPaths, usersSvc]), IUiContextParts
{
    // The app/template chooser requests context before an app has been selected.
    protected override IAppReader? GetDefaultOptions() => null;

    private IUiContextParts UiDefaults => field ??= uiDefaults.New(MyOptions);

    private ISite Site => field ??= siteCtxSvc.Context.Site;
    protected int ZoneId => siteCtxSvc.Context.Site.ZoneId;


    public ContextLanguageInfoDto? GetLanguage(Ctx flags) => UiDefaults.GetLanguage(flags);

    public ContextResourceWithApp? GetSystem(Ctx flags) => UiDefaults.GetSystem(flags);

    public ContextResourceWithApp? GetSite(Ctx flags) => UiDefaults.GetSite(flags);

    public WebResourceDto GetPage(Ctx flags) => UiDefaults.GetPage(flags);

    public ContextEnableDto GetEnable(CtxEnable ctx) => UiDefaults.GetEnable(ctx);

    public ContextAppDto? GetApp(Ctx flags)
    {
        var result = UiDefaults.GetApp(flags);
        if (result == null || MyOptions == null)
            return null;
        var paths = appPaths.Get(MyOptions, Site);
        return result with
        {
            Icon = AppAssetThumbnail.GetUrl(MyOptions, paths!, globalPaths)
        };
    }

    public ContextUserDto? GetUser(Ctx flags)
    {
        var result = UiDefaults.GetUser(flags);
        if (result == null || MyOptions == null)
            return null;

        return result with
        {
            Roles = flags.HasFlag(Ctx.UserRoles) ? usersSvc.Value.GetCurrentUser().Roles.Select(r => r.Name).ToListOpt() : [],
        };
    }

    public IList<FeatureDto> GetFeatures(Ctx flags) => UiDefaults.GetFeatures(flags);
}