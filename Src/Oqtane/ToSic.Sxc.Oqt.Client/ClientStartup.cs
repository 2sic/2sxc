using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Oqtane.Services;
using ToSic.Sxc.Oqt.Client.Services;
using ToSic.Sxc.Oqt.Client.Services.NoOp;
using ToSic.Sxc.Oqt.Shared.Interfaces;

namespace ToSic.Sxc.Oqt.Client;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ClientStartup : IClientStartup
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void ConfigureServices(IServiceCollection services)
    {
        // Must register services here, because can not use IService for registration when service has DI interface in another assembly (eg. ToSic.Sxc.Oqtane.Shared.dll)
        services.TryAddScoped<OqtPageChangeService>();
        services.TryAddScoped<OqtSxcRenderService>();
        services.TryAddScoped<IOqtDebugStateService, OqtDebugStateService>();

        // No Operation Service
        services.AddScoped<IOqtPageChangesOnServerService, OqtPageChangesOnServerNoOpService>();
        services.TryAddScoped<IOqtPrerenderService, OqtPrerenderNoOpService>();
    }
}