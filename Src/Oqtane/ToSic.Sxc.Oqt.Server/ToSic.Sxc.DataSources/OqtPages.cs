using System;
using Microsoft.AspNetCore.Http;
using Oqtane.Repository;
using Oqtane.Security;
using Oqtane.Shared;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Helpers;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Deliver a list of pages from the current platform (Dnn or Oqtane)
    /// </summary>
    [PrivateApi]
    [VisualQuery(
        ExpectsDataOfType = VqExpectsDataOfType,
        GlobalName = VqGlobalName,
        HelpLink = VqHelpLink,
        Icon = VqIcon,
        NiceName = VqNiceName,
        Type = VqType,
        UiHint = VqUiHint)]
    public class OqtPages : Pages
    {
        private readonly IPageRepository _pages;
        private readonly SiteState _siteState;
        private readonly IUserPermissions _userPermissions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILazySvc<ILinkPaths> _linkPathsLazy;

        public OqtPages(Dependencies dependencies, IPageRepository pages, SiteState siteState, IUserPermissions userPermissions, IHttpContextAccessor httpContextAccessor, ILazySvc<ILinkPaths> linkPathsLazy)
        :base(dependencies)
        {
            ConnectServices(
                _pages = pages,
                _siteState = siteState,
                _userPermissions = userPermissions,
                _httpContextAccessor = httpContextAccessor,
                _linkPathsLazy = linkPathsLazy
            );
        }

        private ILinkPaths LinkPaths => _linkPathsLazy.Value;

        protected override List<TempPageInfo> GetPagesInternal()
        {
            var user = _httpContextAccessor?.HttpContext?.User;
            var pages = _pages.GetPages(_siteState.Alias.SiteId)
                .Where(page => _userPermissions.IsAuthorized(user, PermissionNames.View, page.Permissions))
                .ToList();

            var parts = new UrlParts(LinkPaths.GetCurrentRequestUrl());

            return pages.Select(p => new TempPageInfo()
            {
                Id = p.PageId,
                ParentId = p.ParentId ?? -1,
                Title = p.Title,
                Name = p.Name,
                Visible = p.IsNavigation,
                Path = p.Path,
                Url = $"{parts.Protocol}{_siteState.Alias.Name}/{p.Path}".TrimLastSlash(),
                Created = p.CreatedOn,
                Modified = p.ModifiedOn,
            }).ToList();
        }
    }
}
