using Oqtane.Modules;
using ToSic.Sxc.Oqt.App;
using ToSic.Sxc.Oqt.Client.Shared;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Client.Services
{
    public class OqtPageChangesSupportService : IOqtPageChangesSupportService, IService
    {
        public int ApplyHttpHeaders(OqtViewResultsDto result, ModuleProBase page) => 0; // dummy

        public object PageCsp(bool enforced, ModuleProBase page) => new CspOfPage(); // dummy
    }
}