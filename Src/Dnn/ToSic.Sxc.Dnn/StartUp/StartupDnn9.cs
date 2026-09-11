using DotNetNuke.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ToSic.Sxc.Dnn.StartUp;

/// <summary>
/// This is the preferred way to start Dependency Injection, but it requires Dnn 9.4+
/// If an older version of Dnn is used, this code will not run
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class StartupDnn9 : IDnnStartup
{
    private const string DnnLoggingControllerType = "DotNetNuke.Instrumentation.DnnLoggingController";

    public void ConfigureServices(IServiceCollection services)
    {
        // Do standard registration of all services
        DnnDi.RegisterServices(services);

        // DNN 10.4+ owns the Microsoft logging pipeline and registers Serilog in its core Startup.
        // Detect the new public controller instead of checking a version, so prerelease/backported builds work too.
        // DNN 10.3's DefaultLoggerFactory implements log4net's ILoggerFactory, not Microsoft's ILoggerFactory.
        // Older DNN versions therefore need this fallback; adding it on newer DNN would write each event twice.
        if (!DnnHasMicrosoftLogging())
            services.AddLogging(logging => logging
                .AddFilter<DnnLoggerProvider>("ToSic.2sxc", LogLevel.Warning)
                .AddProvider(new DnnLoggerProvider()));

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

    private static bool DnnHasMicrosoftLogging()
        => typeof(DotNetNuke.Instrumentation.LoggerSource).Assembly.GetType(DnnLoggingControllerType) != null;
}
