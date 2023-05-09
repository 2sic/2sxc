using Microsoft.AspNetCore.Http;
using ToSic.Sxc.Oqt.Client.Services;

namespace ToSic.Sxc.Oqt.Server.Services
{
    public class OqtDebugService : IOqtDebugService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OqtDebugService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //public static bool Debug;

        public bool Debug // persist state across circuits (blazor server only)
        {
            get => (_httpContextAccessor?.HttpContext?.Items[DebugKey] as bool?) ?? false;
            set
            {
                if (_httpContextAccessor?.HttpContext != null)
                    _httpContextAccessor.HttpContext.Items[DebugKey] = value;
            }
        }
        private const string DebugKey = "Debug";
    }
}
