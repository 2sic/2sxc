using Oqtane.Models;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Server.Blocks;


[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IOqtSxcViewBuilder
{
    /// <summary>
    /// Render must always be the first thing to be called - to ensure that afterward both headers and html are known.
    /// </summary>
    OqtViewResultsDto Render(Alias alias, Site site, Page page, Module module, bool preRender);

    Page Page { get; }
}