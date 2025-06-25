using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sxc.Code.Generate.Sys.CSharpBaseClasses;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Startup;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcCodeGenerateStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcCodeGen(this IServiceCollection services)
    {
        // v17 Code Generators
        services.TryAddTransient<CSharpDataModelsGenerator>();  // direct registration
        services.AddTransient<IFileGenerator, CSharpDataModelsGenerator>(); // with interface and no try, so all can be listed in DI
        services.AddTransient<IFileGenerator, RazorViewsGenerator>(); // with interface and no try, so all can be listed in DI
        services.AddTransient<IFileGenerator, CSharpServicesGenerator>(); // with interface and no try, so all can be listed in DI
        services.AddTransient<IFileGenerator, WebApiGenerator>(); // with interface and no try, so all can be listed in DI
        services.TryAddTransient<FileSaver>();

        // v20 Code Generators
        services.TryAddTransient<CSharpCustomModelsGenerator>();  // direct registration
        services.AddTransient<IFileGenerator, CSharpCustomModelsGenerator>(); // with interface and no try, so all can be listed in DI

        return services;
    }
}