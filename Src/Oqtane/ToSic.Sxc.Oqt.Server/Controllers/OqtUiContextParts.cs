using Oqtane.Shared;
using ToSic.Eav.Apps;
using ToSic.Sxc.Backend.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Sys.Integration.Paths;
using ToSic.Sxc.WebApi.Sys.ExternalLinks;
using OqtPageOutput = ToSic.Sxc.Oqt.Server.Blocks.Output.OqtPageOutput;

namespace ToSic.Sxc.Oqt.Server.Controllers;

internal class OqtUiContextParts(
    Generator<SxcUiContextParts, IAppReader?> uiDefaults,
    ILinkPaths linkPaths,
    IContextCompleteService siteCtxSvc,
    SiteState siteState,
    ExternalLinksService externalLinksService)
    : ServiceWithSetup<IAppReader?>("Oqt.UiCtxB", connect: [uiDefaults, linkPaths, siteCtxSvc, siteState, externalLinksService]), IUiContextParts
{
    // The app/template chooser requests context before an app has been selected.
    protected override IAppReader? GetDefaultOptions() => null;

    private IUiContextParts UiDefaults => field ??= uiDefaults.New(MyOptions);

    public ContextLanguageInfoDto GetLanguage(Ctx flags) => UiDefaults.GetLanguage(flags);

    public ContextResourceWithApp GetSystem(Ctx flags) =>
        (UiDefaults.GetSystem(flags) ?? new()) with
        {
            Url = linkPaths.AsSeenFromTheDomainRoot("~/"),
        };

    public ContextResourceWithApp GetSite(Ctx flags) =>
        (UiDefaults.GetSite(flags) ?? new()) with
        {
            Id = siteCtxSvc.Context.Site.Id,
            Url = "//" + siteCtxSvc.Context.Site.UrlRoot,
        };

    public WebResourceDto GetPage(Ctx flags) => UiDefaults.GetPage(flags);

    public ContextEnableDto GetEnable(CtxEnable ctx) => UiDefaults.GetEnable(ctx);

    public WebResourceDto GetPage() =>
        new()
        {
            Id = (siteCtxSvc.AppContextOrNull as IContextOfModule)?.Page.Id ?? Eav.Sys.EavConstants.NullId,
        };

    public ContextAppDto GetApp(Ctx flags)
    {
        var appDto = UiDefaults.GetApp(flags);
        if (appDto == null)
            return null;
        appDto = appDto with
        {
            Api = OqtPageOutput.GetSiteRoot(siteState.Alias),
            GettingStartedUrl = GetGettingStartedUrl()
        };
        return appDto;
    }

    public ContextUserDto GetUser(Ctx flags) => UiDefaults.GetUser(flags);

    public IList<FeatureDto> GetFeatures(Ctx flags) => UiDefaults.GetFeatures(flags);

    public string GetGettingStartedUrl()
    {
        var blockCtx = siteCtxSvc.AppContextOrNull as IContextOfModule; // may be null!

        var gsUrl = externalLinksService.LinkToDestination(
            ExternalSxcDestinations.GettingStarted,
            siteCtxSvc.Context.Site,
            blockCtx?.Module.Id ?? 0,
            MyOptions?.Specs,
            true
        );
        return gsUrl;
    }
}