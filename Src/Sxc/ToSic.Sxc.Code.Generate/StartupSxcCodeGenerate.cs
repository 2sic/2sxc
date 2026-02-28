using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Data.Processing;
using ToSic.Sxc.Code.Generate.Data;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sxc.Code.Generate.Sys.CSharpBaseClasses;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Run.Startup;

[InternalApi_DoNotUse_MayChangeWithoutNotice]
public static class StartupSxcCodeGenerate
{
    public static IServiceCollection AddSxcCodeGen(this IServiceCollection services)
    {
        // v17 Code Generators
        services.TryAddTransient<CSharpTypedDataModelsGenerator>();  // direct registration
        services.AddTransient<IFileGenerator, CSharpTypedDataModelsGenerator>(); // with interface and no try, so all can be listed in DI
        services.AddTransient<IFileGenerator, RazorViewsGenerator>(); // with interface and no try, so all can be listed in DI
        services.AddTransient<IFileGenerator, CSharpServicesGenerator>(); // with interface and no try, so all can be listed in DI
        services.AddTransient<IFileGenerator, WebApiGenerator>(); // with interface and no try, so all can be listed in DI
        services.TryAddTransient<FileSaver>();

        // v20 Code Generators
        services.TryAddTransient<CSharpCustomModelsGenerator>();  // direct registration
        services.AddTransient<IFileGenerator, CSharpCustomModelsGenerator>(); // with interface and no try, so all can be listed in DI

        // v21.04 Copilot auto-generate on content-type changes
        services.TryAddTransient<CopilotContentTypeDataProcessor>();
        services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IDataProcessor), typeof(CopilotContentTypeDataProcessor)));

        return services;
    }
}
