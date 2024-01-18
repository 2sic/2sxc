using DotNetNuke.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace ToSic.Sxc.Dnn.StartUp;

/// <summary>
/// This is the preferred way to start Dependency Injection, but it requires Dnn 9.4+
/// If an older version of Dnn is used, this code will not run
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class StartupDnn9 : IDnnStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Do standard registration of all services
        DnnDi.RegisterServices(services);

        // Give it the Dnn 9 Global Service Provider
        // This is critical, because we need the global service provider (which will be created after this code runs)
        // When we do start-up and use singletons.
        // Otherwise singletons won't be properly registered. 
        // https://github.com/dnnsoftware/Dnn.Platform/blob/9f83285a15d23203cbaad72d62add864ab5b8c7f/DNN%20Platform/DotNetNuke.Web/Common/LazyServiceProvider.cs#L28
        IServiceProvider GetPreparedServiceProvider() => typeof(DotNetNuke.Common.Globals)
            .GetProperty("DependencyProvider", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            ?.GetValue(null) as IServiceProvider;

        // Now activate the Service Provider, because some Dnn code still needs the static implementation
        DnnStaticDi.StaticDiReady(GetPreparedServiceProvider);
    }
}