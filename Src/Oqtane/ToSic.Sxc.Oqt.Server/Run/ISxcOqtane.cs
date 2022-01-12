using Oqtane.Models;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Server.Run
{
    // TODO: @STV - I don't think we need this interface any more, pls check and probably remove

    public interface ISxcOqtane
    {
        OqtViewResultsDto Prepare(Alias alias, Site site, Page page, Module module);
    }
}
