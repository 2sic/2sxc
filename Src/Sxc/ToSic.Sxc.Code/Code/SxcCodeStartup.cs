using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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


        return services;
    }


        
}