using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ToSic.Sxc.Oqt.Shared.Interfaces;

namespace ToSic.Sxc.Oqt.Server.Services;

internal class OqtDebugStateService(IHttpContextAccessor httpContextAccessor) : IOqtDebugStateService
{
    public bool IsDebugEnabled => (httpContextAccessor?.HttpContext?.Items[DebugKey] as bool?) ?? false;

    async public Task<bool> GetDebugAsync() => IsDebugEnabled;

    public void SetDebug(bool value)
    {
        if (httpContextAccessor?.HttpContext != null)
            httpContextAccessor.HttpContext.Items[DebugKey] = value;
    }

    private const string DebugKey = "2sxcDebug";

    public string Platform => "Server";
}