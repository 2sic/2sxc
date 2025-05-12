using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;
using ToSic.Sxc.Code.Internal.SourceCode;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;

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

        // v13 DynamicCodeService
        services.TryAddTransient<DynamicCodeService.MyServices>();
        services.TryAddTransient<DynamicCodeService.MyScopedServices>();  // new v15
        services.TryAddTransient<IDynamicCodeService, DynamicCodeService>();
        // note: unclear why this exists, since it will always have a real DynCode Service with the previous TryAdd
        services.TryAddTransient<IDynamicCodeService, DynamicCodeServiceUnknown>();

        //services.TryAddTransient<CodeDataFactory>();
        services.TryAddTransient<ICodeDataFactory, CodeDataFactory>();
        services.TryAddTransient<CodeDataServices>();

        // CmsContext / MyContext
        services.TryAddTransient<ICmsContext, CmsContext>();

        return services;
    }


        
}