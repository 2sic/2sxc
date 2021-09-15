using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Oqtane.Enums;
using Oqtane.Security;
using ToSic.Eav.Context;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Oqt.Server.Run;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Deliver a list of pages from the current platform (Dnn or Oqtane)
    /// </summary>
    [PublicApi]
    [VisualQuery(
        ExpectsDataOfType = VqExpectsDataOfType,
        GlobalName = VqGlobalName,
        HelpLink = VqHelpLink,
        Icon = VqIcon,
        NiceName = VqNiceName,
        Type = VqType,
        UiHint = VqUiHint)]
    public class Pages : CmsBases.PagesBase
    {
        private readonly IPageRepository _pages;
        private readonly SiteState _siteState;
        private readonly IUserPermissions _userPermissions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Pages(IPageRepository pages, SiteState siteState, IUserPermissions userPermissions, IHttpContextAccessor httpContextAccessor)
        {
            _pages = pages;
            _siteState = siteState;
            _userPermissions = userPermissions;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override List<TempPageInfo> GetPagesInternal()
        {
            var user = _httpContextAccessor?.HttpContext?.User;
            var pages = _pages.GetPages(_siteState.Alias.SiteId)
                .Where(page => _userPermissions.IsAuthorized(user, PermissionNames.View, page.Permissions))
                .ToList();
            
            return pages.Select(p => new TempPageInfo()
            {
                Id = p.PageId,
                ParentId = p.ParentId ?? -1,
                Title = p.Title,
                Name = p.Name,
                Visible = p.IsNavigation,
                Path = p.Path,
                Url = $"//{_siteState.Alias.Name}/{p.Path}",
                Created = p.CreatedOn,
                Modified = p.ModifiedOn,
            }).ToList();
        }
    }
}
