using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Client.Services.NoOp;

/// <summary>
/// No Operation Service
/// This is NoOp implementation, just to not break Sxc.Oqt.Client code, during service injection.
/// This code is not doing real work, because it is done in Sxc.Oqt.Server
/// </summary>
internal class OqtPageChangesOnServerNoOpService : IOqtPageChangesOnServerService
{
    public int ApplyHttpHeaders(OqtViewResultsDto result, IOqtHybridLog page) => 0;
}