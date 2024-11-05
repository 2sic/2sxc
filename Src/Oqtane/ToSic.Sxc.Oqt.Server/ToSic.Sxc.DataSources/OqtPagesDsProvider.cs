using System;
using Microsoft.AspNetCore.Http;
using Oqtane.Repository;
using Oqtane.Security;
using Oqtane.Shared;
using ToSic.Eav.Helpers;
using ToSic.Lib.Coding;
using ToSic.Lib.DI;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.Integration.Paths;
using ToSic.Sxc.Web.Internal.Url;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources;

/// <summary>
/// Deliver a list of pages from the current platform (Dnn or Oqtane)
/// </summary>
[PrivateApi]
internal class OqtPagesDsProvider : PagesDataSourceProvider
{
    private const int OqtLevelOffset = 1;

    #region Constructor / DI

    private readonly IPageRepository _pages;
    private readonly SiteState _siteState;
    private readonly IUserPermissions _userPermissions;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LazySvc<ILinkPaths> _linkPathsLazy;

    public OqtPagesDsProvider(IPageRepository pages, SiteState siteState, IUserPermissions userPermissions, IHttpContextAccessor httpContextAccessor, LazySvc<ILinkPaths> linkPathsLazy)
        :base("Oqt.Pages")
    {
        ConnectLogs([
            _pages = pages,
            _siteState = siteState,
            _userPermissions = userPermissions,
            _httpContextAccessor = httpContextAccessor,
            _linkPathsLazy = linkPathsLazy
        ]);
    }

    #endregion

    public override List<PageDataRaw> GetPagesInternal(
        NoParamOrder noParamOrder = default,
        bool includeHidden = default,
        bool includeDeleted = default,
        bool includeAdmin = default,
        bool includeSystem = default,
        bool includeLinks = default,
        bool requireViewPermissions = true,
        bool requireEditPermissions = true)
    {
        var l = Log.Fn<List<PageDataRaw>>();
        var user = _httpContextAccessor?.HttpContext?.User;
        var pages = _pages.GetPages(_siteState.Alias.SiteId)
            .Where(page => _userPermissions.IsAuthorized(user, PermissionNames.View, page.Permissions))
            .ToList();

        var parts = new UrlParts(_linkPathsLazy.Value.GetCurrentRequestUrl());

        return l.ReturnAsOk(pages.Select(p => new PageDataRaw
        {
            // In v14
            Id = p.PageId,
            Guid = Guid.Empty,
            ParentId = p.ParentId ?? NoParent,
            Title = p.Title,
            Name = p.Name,
            Visible = p.IsNavigation,
            Path = p.Path,
            Url = $"{parts.Protocol}{_siteState.Alias.Name}/{p.Path}".TrimLastSlash(),
            Created = p.CreatedOn,
            Modified = p.ModifiedOn,

            // New in 15.01
            Clickable = p.IsClickable,
            HasChildren = p.HasChildren,
            IsDeleted = p.IsDeleted,
            Level = p.Level + OqtLevelOffset,
            Order = p.Order,
            // New in 15.02
            LinkTarget = "", // TODO
        }).ToList());
    }
}