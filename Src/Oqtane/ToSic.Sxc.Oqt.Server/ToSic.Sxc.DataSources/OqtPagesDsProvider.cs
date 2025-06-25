using Microsoft.AspNetCore.Http;
using Oqtane.Repository;
using Oqtane.Security;
using Oqtane.Shared;
using ToSic.Lib.Coding;
using ToSic.Lib.DI;
using ToSic.Sxc.Cms.Pages.Internal;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.Sys.Integration.Paths;
using ToSic.Sxc.Web.Sys.Url;
using ToSic.Sys.Utils;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources;

/// <summary>
/// Deliver a list of pages from the current platform (Dnn or Oqtane)
/// </summary>
[PrivateApi]
internal class OqtPagesDsProvider(
    IPageRepository pages,
    SiteState siteState,
    IUserPermissions userPermissions,
    IHttpContextAccessor httpContextAccessor,
    LazySvc<ILinkPaths> linkPathsLazy)
    : PagesDataSourceProvider("Oqt.Pages",
        connect: [pages, siteState, userPermissions, httpContextAccessor, linkPathsLazy])
{
    private const int OqtLevelOffset = 1;

    public override List<PageModelRaw> GetPagesInternal(
        NoParamOrder noParamOrder = default,
        bool includeHidden = default,
        bool includeDeleted = default,
        bool includeAdmin = default,
        bool includeSystem = default,
        bool includeLinks = default,
        bool requireViewPermissions = true,
        bool requireEditPermissions = true)
    {
        var l = Log.Fn<List<PageModelRaw>>();
        var user = httpContextAccessor?.HttpContext?.User;
        var allowed = pages
            .GetPages(siteState.Alias.SiteId)
            .Where(page => userPermissions.IsAuthorized(user, PermissionNames.View, page.PermissionList))
            .ToList();

        var parts = new UrlParts(linkPathsLazy.Value.GetCurrentRequestUrl());

        var converted = allowed
            .Select(p => new PageModelRaw
            {
                // In v14
                Id = p.PageId,
                Guid = Guid.Empty,
                ParentId = p.ParentId ?? NoParent,
                Title = p.Title.UseFallbackIfNoValue(p.Name),
                Name = p.Name,
                IsNavigation = p.IsNavigation,
                Path = p.Path,
                Url = $"{parts.Protocol}{siteState.Alias.Name}/{p.Path}".TrimLastSlash(),
                Created = p.CreatedOn,
                Modified = p.ModifiedOn,

                // New in 15.01
                IsClickable = p.IsClickable,
                HasChildren = p.HasChildren,
                IsDeleted = p.IsDeleted,
                Level = p.Level + OqtLevelOffset,
                Order = p.Order,
                // New in 15.02
                LinkTarget = "", // TODO
            })
            .ToList();

        return l.ReturnAsOk(converted);
    }
}