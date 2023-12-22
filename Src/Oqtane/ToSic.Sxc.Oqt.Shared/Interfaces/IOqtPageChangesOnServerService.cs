using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Shared.Interfaces;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IOqtPageChangesOnServerService
{
    int ApplyHttpHeaders(OqtViewResultsDto result, IOqtHybridLog page);
}