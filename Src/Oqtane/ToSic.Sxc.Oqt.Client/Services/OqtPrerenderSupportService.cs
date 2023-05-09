using System.Net.Http;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;

namespace ToSic.Sxc.Oqt.Client.Services
{
    public class OqtPrerenderSupportService : IOqtPrerenderSupportService, IService
    {
        public bool Executed // dummy
        {
            get => false;
            set { }
        }

        public bool HasUserAgentSignature() => false; // dummy
    }
}
