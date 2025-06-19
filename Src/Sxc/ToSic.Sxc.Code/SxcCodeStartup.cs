using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Code.Internal.SourceCode;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;

// ReSharper disable once CheckNamespace
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

        services.TryAddTransient<IExecutionContextFactory, ExecutionContextFactory>();

        // Code / Dynamic Code
        services.TryAddTransient<ExecutionContext.MyServices>();

        // Code Fallbacks if not registered by the platform
        services.TryAddTransient<ExecutionContext, ExecutionContextUnknown>();
        services.TryAddTransient(typeof(ExecutionContext<,>), typeof(ExecutionContextUnknown<,>));

        // v13 DynamicCodeService
        services.TryAddTransient<DynamicCodeService.MyServices>();
        services.TryAddTransient<DynamicCodeService.MyScopedServices>();  // new v15
        services.TryAddTransient<IDynamicCodeService, DynamicCodeService>();
        // note: unclear why this exists, since it will always have a real DynCode Service with the previous TryAdd
        services.TryAddTransient<IDynamicCodeService, DynamicCodeServiceUnknown>();

        //services.TryAddTransient<CodeDataFactory>();
        services.TryAddTransient<ICodeDataFactory, CodeDataFactory>();
        services.TryAddTransient<CodeDataServices>();

        // Temporary solution for the TokenEngine
        services.TryAddTransient<ITokenEngine, TokenEngine>();

        // CmsContext / MyContext
        services.TryAddTransient<ICmsContext, CmsContext>();
        // Module fallback context
        services.TryAddTransient<IModule, ModuleUnknown>();
        services.TryAddTransient<IPage, Page>();
        services.TryAddTransient<Page>();
        services.TryAddSingleton<IPlatform, PlatformUnknown>();

        // v15
        services.TryAddTransient<CodeCreateDataSourceSvc>();

        return services;
    }


        
}