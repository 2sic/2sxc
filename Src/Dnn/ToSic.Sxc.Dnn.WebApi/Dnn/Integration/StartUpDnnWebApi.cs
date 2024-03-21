using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using ToSic.Eav.WebApi.ApiExplorer;
using ToSic.Eav.WebApi.Context;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Dnn.Backend;
using ToSic.Sxc.Dnn.Backend.Admin;
using ToSic.Sxc.Dnn.WebApi.Context;
using ToSic.Sxc.Dnn.WebApi.Internal.HttpJson;

namespace ToSic.Sxc.Dnn.Integration;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public static class StartUpDnnWebApi
{
    public static IServiceCollection AddDnnWebApi(this IServiceCollection services)
    {
        // Settings / WebApi stuff
        services.TryAddTransient<IUiContextBuilder, DnnUiContextBuilder>();
        services.TryAddTransient<IApiInspector, DnnApiInspector>();

        // new #2160
        services.TryAddTransient<AdamSecurityChecksBase, DnnAdamSecurityChecks>();

        services.TryAddTransient<DnnGetBlock>();

        services.TryAddTransient<DnnAppFolderUtilities>(); // v14.12-01

        // new v15
        services.TryAddTransient<ApiControllerMyServices>();

        // new v17
        services.TryAddTransient<AppApiControllerSelectorService>();

        return services;
    }

    public static void Configure()
    {
        // Configure Newtonsoft Time zone handling
        GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

        // System.Text.Json supports ISO 8601-1:2019, including the RFC 3339 profile
        GlobalConfiguration.Configuration.Formatters.Add(JsonFormatters.SystemTextJsonMediaTypeFormatter);

    }
}