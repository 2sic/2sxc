using Oqtane.Models;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Shared.Run
{
    public interface ISxcOqtane
    {
        OqtViewResultsDto Prepare(Alias alias, Site site, Page page, Module module);
    }
}
