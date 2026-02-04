using Custom.Hybrid;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Images.Sys;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Link.Sys;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sxc.Sys.Integration.Paths;
using ToSic.Sys.Utils;
using Page = Oqtane.Models.Page;

namespace ToSic.Sxc.Oqt.Server.Services;

/// <summary>
/// The Oqtane implementation of the <see cref="ILinkService"/>.
/// </summary>
[PrivateApi]
internal class OqtLinkService(
    IPageRepository pageRepository,
    AliasResolver aliasResolver,
    ImgResizeLinker imgLinker,
    LazySvc<ILinkPaths> linkPathsLazy)
    : LinkServiceBase(imgLinker, linkPathsLazy, connect: [pageRepository, aliasResolver])
{
    public Razor12 RazorPage { get; set; }
    private IContextOfBlock _blockCtx;

    private new OqtLinkPaths LinkPaths => (OqtLinkPaths) base.LinkPaths;

    public override void ConnectToRoot(IExecutionContext codeRoot)
    {
        base.ConnectToRoot(codeRoot);
        _blockCtx = codeRoot.GetContextOfBlock();
    }

    protected override string ToApi(string api, string parameters = null) => ApiNavigateUrl(api, parameters);

    protected override string ToPage(int? pageId, string parameters = null, string language = null) =>
        PageNavigateUrl(pageId, parameters);

    // Prepare Api link.
    private string ApiNavigateUrl(string api, string parameters)
    {
        var alias = aliasResolver.Alias;

        var pathWithQueryString = CombineApiWithQueryString(
            LinkPaths.ApiFromSiteRoot(AppFolder, api),
            parameters);

        var relativePath = string.IsNullOrEmpty(alias.Path)
            ? pathWithQueryString
            : $"/{alias.Path}{pathWithQueryString}";

        return relativePath;
    }

    // Prepare Page link.
    private string PageNavigateUrl(int? pageId, string parameters, bool absoluteUrl = true)
    {
        var currentPageId = _blockCtx?.Page?.Id;

        if ((pageId ?? currentPageId) == null)
            throw new($"Error, PageId is unknown, pageId: {pageId}, currentPageId: {currentPageId} .");

        if (pageId.HasValue)
        {
            var page = pageRepository.GetPage(pageId.Value, false);
            if (page != null) return PageUrlBuilder(page, parameters, absoluteUrl);
        }

        // if pageId is invalid, fallback to currentPageId
        var currentPage = pageRepository.GetPage(currentPageId.Value, false);
        var currentPageUrl = PageUrlBuilder(currentPage, parameters, absoluteUrl);

        return CurrentPageUrlWithEventualHashError(pageId, currentPageUrl);
    }

    private string PageUrlBuilder(Page page, string parameters, bool absoluteUrl)
    {
        aliasResolver.InitIfEmpty(page.SiteId);
        var alias = aliasResolver.Alias 
            ?? throw new($"Error, Alias is unknown, pageId: {page.PageId}, siteId: {page.SiteId}."); 

        // for invalid page numbers just skip that part 
        var relativePath =
            Utilities.NavigateUrl(alias.Path, page?.Path ?? string.Empty, QueryParametersForOqtane(parameters)) // NavigateUrl do not works with absolute links
            .PrefixSlash(); // fix for Oqt v5.1.0+ NavigateUrl returns relative path without leading slash

        return absoluteUrl ? $"{LinkPaths.GetCurrentLinkRoot()}{relativePath}" : relativePath;
    }

    /// <summary>
    /// Oqtane Utilities.NavigateUrl and Utilities.ParseParameters
    /// expects queryParameters that starts with '?' so we need to add it.
    /// This is to ensure that that oqtane will work with / in query string,
    /// or oqtane will recognize it as special oqtane UrlParameters
    /// that starts with /!/ separator in page route part of url and
    /// that is currently not expected in 2sxc apps.
    /// </summary>
    /// <param name="queryParameters"></param>
    /// <returns>query params that starts with ?</returns>
    private static string QueryParametersForOqtane(string queryParameters) 
        => string.IsNullOrEmpty(queryParameters) ? string.Empty : $"?{queryParameters?.TrimStart('?')}";
}