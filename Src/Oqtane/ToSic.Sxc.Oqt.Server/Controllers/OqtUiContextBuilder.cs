using Oqtane.Shared;
using ToSic.Eav.Context;
using ToSic.Sxc.Backend.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Sys.Integration.Paths;
using ToSic.Sxc.WebApi.Sys.ExternalLinks;
using OqtPageOutput = ToSic.Sxc.Oqt.Server.Blocks.Output.OqtPageOutput;

namespace ToSic.Sxc.Oqt.Server.Controllers;

internal class OqtUiContextBuilder(
    ILinkPaths linkPaths,
    IContextOfSiteService ctxSvc,
    SiteState siteState,
    ExternalLinksService externalLinksService,
    UiContextBuilderBase.Dependencies deps)
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

        result.Id = ctxSvc.Context.Site.Id;
        result.Url = "//" + ctxSvc.Context.Site.UrlRoot;
        return result;
    }

    protected override WebResourceDto GetPage() =>
        new()
        {
            Id = (ctxSvc as IContextOfBlock)?.Page.Id ?? Eav.Sys.EavConstants.NullId,
        };

    protected override ContextAppDto GetApp(Ctx flags)
    {
        var appDto = base.GetApp(flags);
        if (appDto != null) appDto.Api = OqtPageOutput.GetSiteRoot(siteState.Alias);
        return appDto;
    }

    protected override string GetGettingStartedUrl()
    {
        var blockCtx = ctxSvc as IContextOfBlock; // may be null!

        var gsUrl = externalLinksService.LinkToDestination(
            ExternalSxcDestinations.GettingStarted,
            Services.SiteCtxSvc.Context.Site,
            blockCtx?.Module.Id ?? 0,
            AppSpecsOrNull,
            true
        );
        return gsUrl;
    }
}