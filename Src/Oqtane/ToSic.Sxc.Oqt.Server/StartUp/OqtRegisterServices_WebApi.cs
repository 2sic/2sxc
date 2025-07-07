using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.WebApi.Sys.ApiExplorer;
using ToSic.Sxc.Oqt.Server.WebApi;
using ToSic.Sxc.Oqt.Server.WebApi.Admin;
using ToSic.Sxc.WebApi.Sys.ActionFilters;

namespace ToSic.Sxc.Oqt.Server.StartUp;

partial class OqtRegisterServices
{
    /// <summary>
    /// Things needed by the standard API Controllers to work
    /// </summary>
    private static IServiceCollection AddSxcOqtApiParts(this IServiceCollection services)
    {
        // ApiExplorer helper - inspects a custom WebApi class to figure out what it provides
        services.TryAddTransient<IApiInspector, OqtApiInspector>();

        return services;
    }


    private static IServiceCollection AddOqtaneApiPlumbingAndHelpers(this IServiceCollection services)
    {
        // action filter for exceptions
        services.AddTransient<HttpResponseExceptionFilter>();

        // action filter instead of global option AllowEmptyInputInBodyModelBinding = true
        services.AddTransient<OptionalBodyFilter>();

        services.TryAddTransient<AppAssetsControllerBase.Dependencies>();

        // ViewController deps for AllModulesWithContent
        services.TryAddTransient<Pages.Pages>();

        return services;
    }
}