using ToSic.Sxc.Oqt.App;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Web.ContentSecurityPolicy;

namespace ToSic.Sxc.Oqt.Client.Services;

public interface IOqtPageChangesSupportService
{
    int ApplyHttpHeaders(OqtViewResultsDto result, ModuleProBase page);
    CspOfPage PageCsp(bool enforced, ModuleProBase page);
}