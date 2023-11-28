using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ToSic.Sxc.Oqt.Shared.Interfaces;

namespace ToSic.Sxc.Oqt.Server.Services;

public class OqtDebugStateService : IOqtDebugStateService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OqtDebugStateService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
        
    public bool IsDebugEnabled => (_httpContextAccessor?.HttpContext?.Items[DebugKey] as bool?) ?? false;

    async public Task<bool> GetDebugAsync() => IsDebugEnabled;

    public void SetDebug(bool value)
    {
        if (_httpContextAccessor?.HttpContext != null)
            _httpContextAccessor.HttpContext.Items[DebugKey] = value;
    }

    private const string DebugKey = "2sxcDebug";
}