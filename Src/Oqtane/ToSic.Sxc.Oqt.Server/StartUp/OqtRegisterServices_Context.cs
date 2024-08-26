using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Context;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Blocks.Output;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Web.Internal.JsContext;

namespace ToSic.Sxc.Oqt.Server.StartUp;

partial class OqtRegisterServices
{

    /// <summary>
    /// Context information like the ISite, IUser etc.
    /// </summary>
    private static IServiceCollection AddSxcOqtContext(this IServiceCollection services)
    {
        // Context: Things which are relevant for determining the context
        services.TryAddScoped<ISite, OqtSite>();
        services.TryAddScoped<IPage, OqtPage>();
        services.TryAddScoped<IUser, OqtUser>();

        services.TryAddTransient<IModule, OqtModule>();

        // TODO: @STV - this could cause a bug - maybe better not to mix this? OqtSite transient?
        services.TryAddTransient<IZoneCultureResolver, OqtSite>();
        //services.TryAddTransient<IZoneCultureResolver>(x => x.GetRequiredService<ISite>()); // eventual alternative to line above

        services.TryAddScoped<OqtSecurity>();
        services.TryAddScoped<IJsApiService, OqtJsApiService>();

        return services;
    }
}