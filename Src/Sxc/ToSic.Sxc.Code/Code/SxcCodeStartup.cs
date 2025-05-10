using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;
using ToSic.Sxc.Code.Internal.SourceCode;

namespace ToSic.Sxc.Code;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcCodeStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcCode(this IServiceCollection services)
    {
        services.TryAddTransient<CodeErrorHelpService>();
        services.TryAddTransient<SourceAnalyzer>();

        services.TryAddTransient<ICodeCustomizer, Customizer.Customizer>();

        services.TryAddTransient<ICodeApiServiceFactory, CodeApiServiceFactory>();

        // Code / Dynamic Code
        services.TryAddTransient<CodeApiService.MyServices>();

        // Code Fallbacks if not registered by the platform
        services.TryAddTransient<CodeApiService, CodeApiServiceUnknown>();
        services.TryAddTransient(typeof(CodeApiService<,>), typeof(CodeApiServiceUnknown<,>));

        return services;
    }


        
}