using Oqtane.Shared;
using ToSic.Eav.Context;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Backend.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Integration.Installation;
using ToSic.Sxc.Integration.Paths;
using OqtPageOutput = ToSic.Sxc.Oqt.Server.Blocks.Output.OqtPageOutput;

namespace ToSic.Sxc.Oqt.Server.Controllers;

internal class OqtUiContextBuilder(
    ILinkPaths linkPaths,
    IContextOfSite ctx,
    SiteState siteState,
    RemoteRouterLink remoteRouterLink,
    UiContextBuilderBase.MyServices deps)
    : UiContextBuilderBase(deps)
{
    protected override ContextResourceWithApp GetSystem(Ctx flags)
    {
        var result = base.GetSystem(flags);

        result.Url = linkPaths.AsSeenFromTheDomainRoot("~/");
        return result;
    }

    protected override ContextResourceWithApp GetSite(Ctx flags)
    {
        var result = base.GetSite(flags);

        result.Id = ctx.Site.Id;
        result.Url = "//" + ctx.Site.UrlRoot;
        return result;
    }

    protected override WebResourceDto GetPage() =>
        new()
        {
            Id = (ctx as IContextOfBlock)?.Page.Id ?? Eav.Constants.NullId,
        };

    protected override ContextAppDto GetApp(Ctx flags)
    {
        var appDto = base.GetApp(flags);
        if (appDto != null) appDto.Api = OqtPageOutput.GetSiteRoot(siteState.Alias);
        return appDto;
    }

    protected override string GetGettingStartedUrl()
    {
        var blockCtx = ctx as IContextOfBlock; // may be null!

        var gsUrl = remoteRouterLink.LinkToRemoteRouter(
            RemoteDestinations.GettingStarted,
            Services.SiteCtx.Site,
            blockCtx?.Module.Id ?? 0,
            AppSpecsOrNull,
            true
        );
        return gsUrl;
    }
}