using Oqtane.Modules;
using ToSic.Sxc.Oqt.App;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Web.ContentSecurityPolicy;

namespace ToSic.Sxc.Oqt.Client.Services
{
    public class OqtPageChangesSupportService : IOqtPageChangesSupportService, IService
    {
        public int ApplyHttpHeaders(OqtViewResultsDto result, ModuleProBase page) => 0; // dummy

        public CspOfPage PageCsp(bool enforced, ModuleProBase page) => new(); // dummy


    }
}
