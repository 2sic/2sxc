using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Oqtane.Services;
using ToSic.Sxc.Oqt.Client.Services;
using ToSic.Sxc.Oqt.Shared.Interfaces;

namespace ToSic.Sxc.Oqt.Client
{
  public class ClientStartup : IClientStartup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      // Must register services here, because can not use IService for registration when service has DI interface in another assembly (eg. ToSic.Sxc.Oqtane.Shared.dll)
      services.TryAddScoped<OqtPageChangeService>();
      services.TryAddScoped<IOqtPageChangesSupportService, OqtPageChangesSupportService>();
      services.TryAddScoped<IOqtDebugStateService, OqtDebugStateService>();
      services.TryAddScoped<IOqtPrerenderService, OqtPrerenderService> ();
      services.TryAddScoped<OqtSxcRenderService>();
    }
  }
}
