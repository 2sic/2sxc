using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Shared.Interfaces;

public interface IOqtPageChangesSupportService
{
    int ApplyHttpHeaders(OqtViewResultsDto result, IOqtHybridLog page);
    object PageCsp(bool enforced, IOqtHybridLog page);
}