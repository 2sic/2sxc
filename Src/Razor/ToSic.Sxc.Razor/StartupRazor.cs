using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Razor.DotNetOverrides;
using ToSic.Sxc.Web.Internal.DotNet;
using RuntimeViewCompiler = ToSic.Sxc.Razor.DotNetOverrides.RuntimeViewCompiler;

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

        // 2sxc Razor Parts
        services.TryAddTransient<IRazorCompiler, RazorCompiler>();
        services.TryAddTransient<IRazorRenderer, RazorRenderer>();
        services.TryAddTransient<IRazorEngine, RazorEngine>();

        // debugging
        services.Replace(ServiceDescriptor.Singleton<IViewCompilerProvider, RuntimeViewCompilerProvider>());
        //services.Replace(ServiceDescriptor.Singleton<IViewCompiler, RuntimeViewCompiler>());
        services.TryAddSingleton<CSharpCompiler>();
        services.TryAddSingleton<RazorReferenceManager, RazorReferenceManagerEnhanced>();
        services.TryAddSingleton<RuntimeCompilationFileProvider>();
        services.TryAddTransient<HotBuildReferenceManager>();

        return services;
    }
}