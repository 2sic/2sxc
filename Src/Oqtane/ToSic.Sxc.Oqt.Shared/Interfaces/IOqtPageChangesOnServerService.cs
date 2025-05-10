using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Shared.Interfaces;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IOqtPageChangesOnServerService
{
    int ApplyHttpHeaders(OqtViewResultsDto result, IOqtHybridLog page);
}