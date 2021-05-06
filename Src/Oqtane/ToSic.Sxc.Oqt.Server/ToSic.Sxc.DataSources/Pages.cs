using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;

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
        private readonly ISettingRepository _settings;
        private readonly SiteState _siteState;

        public Pages(IPageRepository pages, ISettingRepository settings, SiteState siteState)
        {
            _pages = pages;
            _settings = settings;
            _siteState = siteState;
        }

        protected override List<TempPageInfo> GetPagesInternal()
        {
            // TODO: Ignoring user and module security for first version.
            return _pages.GetPages(_siteState.Alias.SiteId).Select(p => new TempPageInfo()
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
