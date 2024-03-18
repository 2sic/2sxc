using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Code.Generate.Internal;

namespace ToSic.Sxc.Code.Generate.Startup;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public static class RegisterSxcCodeGenServices
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddSxcCodeGen(this IServiceCollection services)
    {
        // v17 Code Generators
        services.TryAddTransient<CSharpDataModelsGenerator>();  // direct registration
        services.AddTransient<IFileGenerator, CSharpDataModelsGenerator>(); // with interface and no try, so all can be listed in DI
        services.TryAddTransient<FileSaver>();

        return services;
    }
}