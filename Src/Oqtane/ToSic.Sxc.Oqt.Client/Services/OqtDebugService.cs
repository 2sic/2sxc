using System.Net.Http;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;

namespace ToSic.Sxc.Oqt.Client.Services
{
    public class OqtDebugService : IOqtDebugService, IService
    {
        public bool Debug // dummy
        {
            get => false;
            set { }
        }
    }
}
