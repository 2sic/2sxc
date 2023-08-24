using Oqtane.Shared;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Shared.Interfaces
{
    public interface IOqtPrerenderService
    {
        string GetPrerenderHtml(bool isPrerendered, OqtViewResultsDto viewResults, SiteState siteState, string themeType);
    }
}