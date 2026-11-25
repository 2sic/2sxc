using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneCulture;
using ToSic.Eav.Apps.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Blocks.Output;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Render.JsContext.Sys;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Oqt.Server.StartUp;

partial class OqtRegisterServices
{

    /// <summary>
    /// Context information like the ISite, IUser etc.
    /// </summary>
    private static IServiceCollection AddSxcOqtContext(this IServiceCollection services)
    {
        // Context: Things which are relevant for determining the context
        services.TryAddScoped<IOqtTenantContext, OqtTenantContext>();
        services.TryAddScoped<ISite, OqtSite>();
        services.TryAddScoped<IPage, OqtPage>();
        services.TryAddScoped<IUser, OqtUser>();

        services.TryAddTransient<IModule, OqtModule>();
        services.TryAddTransient<IModuleFactory, OqtModule>(); // new v22
        services.TryAddTransient<OqtContextOfBlockFactory>();

        // Culture lookups must use the initialized site of this execution scope,
        // including background indexing where no request can initialize another OqtSite.
        services.TryAddTransient<IZoneCultureResolver>(sp => sp.GetRequiredService<ISite>());

        services.TryAddScoped<OqtSecurity>();
        services.TryAddScoped<IJsApiService, OqtJsApiService>();

        // must be Transient (scoped cause StackOverflowException infinite recursion)
        services.TryAddTransient<IAppCacheKeyService, OqtAppCacheKeyService>();

        return services;
    }
}
