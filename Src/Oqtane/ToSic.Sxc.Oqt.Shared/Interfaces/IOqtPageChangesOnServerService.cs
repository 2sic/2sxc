using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Shared.Interfaces;

public interface IOqtPageChangesOnServerService
{
    int ApplyHttpHeaders(OqtViewResultsDto result, IOqtHybridLog page);
}