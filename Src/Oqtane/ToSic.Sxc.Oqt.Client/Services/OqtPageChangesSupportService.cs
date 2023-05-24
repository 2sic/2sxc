using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Client.Services
{
  public class OqtPageChangesSupportService : IOqtPageChangesSupportService
  {
        public int ApplyHttpHeaders(OqtViewResultsDto result, IOqtHybridLog page) => 0; // dummy

        public object PageCsp(bool enforced, IOqtHybridLog page) => new CspOfPage(); // dummy
    }
}