using ToSic.Sxc.Oqt.App;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Client.Services;

public interface IOqtPageChangesSupportService
{
    int ApplyHttpHeaders(OqtViewResultsDto result, ModuleProBase page);
    object PageCsp(bool enforced, ModuleProBase page);
}