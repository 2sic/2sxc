using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using System.Web;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Dto;
using ToSic.Lib.Data;
using ToSic.Sxc.Backend.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Integration.Installation;

namespace ToSic.Sxc.Dnn.WebApi.Context;

internal sealed class DnnUiContextBuilder(
    ISxcContextResolver ctxResolver,
    RemoteRouterLink remoteRouterLink,
    UiContextBuilderBase.MyServices deps)
    : UiContextBuilderBase(deps)
{
    #region Constructor / DI

    private readonly PortalSettings _portal = PortalSettings.Current;

    private ModuleInfo Module => (ctxResolver.BlockContextOrNull()?.Module as IWrapper<ModuleInfo>)?.GetContents();

    #endregion

    protected override ContextResourceWithApp GetSystem(Ctx flags)
    {
        var result = base.GetSystem(flags);
        result.Url = VirtualPathUtility.ToAbsolute("~/");
        return result;
    }

    protected override ContextResourceWithApp GetSite(Ctx flags)
    {
        var result = base.GetSite(flags);
        result.Id = _portal.PortalId;
        result.Url = "//" + _portal.PortalAlias.HTTPAlias + "/";
        return result;
    }

    protected override WebResourceDto GetPage() =>
        Module == null ? null
            : new WebResourceDto
            {
                Id = Module.TabID,
                // todo: maybe page url
                // used to be on ps.ActiveTab.FullUrl;
                // but we can't get it from there directly
            };

    protected override ContextAppDto GetApp(Ctx flags)
    {
        var appDto = base.GetApp(flags);
        // If no app is selected yet, then there is no information to return
        if (appDto == null) return null;

        try
        {
            var roots = DnnJsApiService.GetApiRoots();
            appDto.Api = roots.AppApiRoot;
        } catch { /* ignore */ }
        return appDto;
    }

    /// <summary>
    /// build a getting-started url which is used to correctly show the user infos like
    /// warnings related to his dnn or 2sxc version
    /// infos based on his languages
    /// redirects based on the app he's looking at, etc.
    /// </summary>
    /// <returns></returns>
    protected override string GetGettingStartedUrl()
    {
        if (AppSpecsOrNull is not { } app) return "";

        var gsUrl = remoteRouterLink.LinkToRemoteRouter(
            RemoteDestinations.GettingStarted,
            Services.SiteCtx.Site,
            Module.ModuleID,
            app,
            Module.DesktopModule.ModuleName == "2sxc");
        return gsUrl;
    }
}