using Oqtane.Shared;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Shared.Interfaces;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IOqtPrerenderService
{
    string GetPrerenderHtml(bool isPrerendered, OqtViewResultsDto viewResults, SiteState siteState, string themeType);
}