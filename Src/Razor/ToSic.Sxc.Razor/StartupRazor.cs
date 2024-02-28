using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Razor.DbgWip;
using ToSic.Sxc.Web.Internal.DotNet;

namespace ToSic.Sxc.Razor;

// ReSharper disable once InconsistentNaming
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public static class StartupRazor
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddSxcRazor(this IServiceCollection services)
    {
        // .net Core parts
        services.TryAddTransient<IHttp, HttpNetCore>();

        // Razor Parts
        services.TryAddTransient<IRazorCompiler, RazorCompiler>();
        services.TryAddTransient<IAppCodeRazorCompiler, AppCodeRazorCompiler>();
        services.TryAddTransient<IRazorRenderer, RazorRenderer>();
        services.TryAddTransient<IRazorEngine, NetCoreRazorEngine>();



        // debugging
        //services.TryAddTransient<RazorReferenceManager>();
        services.Replace(ServiceDescriptor.Singleton<IViewCompilerProvider, RuntimeViewCompilerProvider>());
        services.TryAddSingleton<IViewCompiler, RuntimeViewCompiler>();
        services.TryAddSingleton<CSharpCompiler>();
        services.TryAddSingleton<RazorReferenceManager, RazorReferenceManagerEnhanced>();
        services.TryAddSingleton<RuntimeCompilationFileProvider>();
        services.TryAddTransient<HotBuildReferenceManager>();

        return services;
    }
}