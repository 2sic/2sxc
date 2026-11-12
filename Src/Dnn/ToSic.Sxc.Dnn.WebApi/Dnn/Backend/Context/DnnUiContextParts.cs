using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Abstractions.Portals;
using System.Web;
using ToSic.Eav.Apps;
using ToSic.Eav.WebApi.Sys.Context;
using ToSic.Eav.WebApi.Sys.Dto;
using ToSic.Sxc.Backend.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.WebApi.Sys.ExternalLinks;

namespace ToSic.Sxc.Dnn.WebApi.Context;

internal sealed class DnnUiContextParts(Generator<SxcUiContextParts, IAppReader?> uiDefaults, IContextCompleteService ctxService, ExternalLinksService externalLinksService)
    : ServiceBase("Dnn.UiCtxP", connect: [uiDefaults, ctxService, externalLinksService]), IUiContextParts
{
    #region Constructor / DI
    public void Setup(IAppReader? options) => AppReader = options;

    private IAppReader? AppReader { get; set; }

    private ModuleInfo Module => (ctxService.BlockContextOrNull()?.Module as IWrapper<ModuleInfo>)?.GetContents();

    private IUiContextParts UiDefaults => field ??= uiDefaults.New(AppReader);

    #endregion

    public ContextLanguageInfoDto GetLanguage(Ctx flags) => UiDefaults.GetLanguage(flags);

    public ContextResourceWithApp? GetSystem(Ctx flags) =>
        (UiDefaults.GetSystem(flags) ?? new()) with
        {
            Url = VirtualPathUtility.ToAbsolute("~/"),
        };

    public ContextResourceWithApp GetSite(Ctx flags)
    {
        var portal = PortalSettings.Current;
        return (UiDefaults.GetSite(flags) ?? new()) with
        {
            Id = portal.PortalId,
            Url = "//" + (portal.PortalAlias as IPortalAliasInfo)?.HttpAlias + "/"
        };
    }

    public WebResourceDto? GetPage(Ctx flags) =>
        Module == null ? null
            : new WebResourceDto
            {
                Id = Module.TabID,
                // todo: maybe page url
                // used to be on ps.ActiveTab.FullUrl;
                // but we can't get it from there directly
            };

    public ContextEnableDto GetEnable(CtxEnable ctx) => UiDefaults.GetEnable(ctx);

    public ContextAppDto? GetApp(Ctx flags)
    {
        var appDto = UiDefaults.GetApp(flags);
        // If no app is selected yet, then there is no information to return
        if (appDto == null)
            return null;
        appDto = appDto with { GettingStartedUrl = GetGettingStartedUrl() };
        try
        {
            var roots = DnnJsApiService.GetApiRoots();
            return appDto with { Api = roots.AppApiRoot };
        } catch { /* ignore */ }
        return appDto;
    }

    public ContextUserDto GetUser(Ctx flags) => UiDefaults.GetUser(flags);

    public IList<FeatureDto> GetFeatures(Ctx flags) => UiDefaults.GetFeatures(flags);

    /// <summary>
    /// build a getting-started url which is used to correctly show the user infos like
    /// warnings related to his dnn or 2sxc version
    /// infos based on his languages
    /// redirects based on the app he's looking at, etc.
    /// </summary>
    /// <returns></returns>
    public string GetGettingStartedUrl()
    {
        if (AppReader?.Specs is not { } specs)
            return "";

        var gsUrl = externalLinksService.LinkToDestination(
            ExternalSxcDestinations.GettingStarted,
            ctxService.Context.Site,
            Module.ModuleID,
            specs,
            Module.DesktopModule.ModuleName == "2sxc");
        return gsUrl;
    }
}
