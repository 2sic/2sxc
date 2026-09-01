using ToSic.Eav.Apps.Sys.AppJson;
using ToSic.Sxc.Sys.Integration.Installation;
using DotNetNuke.Abstractions.Application;
using ToSic.Sys.Configuration;

namespace ToSic.Sxc.Dnn.Install;

internal partial class DnnEnvironmentInstaller : ServiceBase, IEnvironmentInstaller
{
    public static bool SaveUnimportantDetails = true;

    private readonly DnnInstallLogger _installLogger;
    private readonly LazySvc<IGlobalConfiguration> _globalConfiguration;
    private readonly LazySvc<IAppJsonConfigurationService> _appJsonService;
    private readonly IHostSettingsService _hostSettingsService;

    /// <summary>
    /// Instance initializers...
    /// </summary>
    public DnnEnvironmentInstaller(ILogStore logStore, DnnInstallLogger installLogger, LazySvc<IGlobalConfiguration> globalConfiguration, LazySvc<IAppJsonConfigurationService> appJsonService, IHostSettingsService hostSettingsService)
        : base("Dnn.InstCo", connect: [appJsonService, installLogger, globalConfiguration])
    {
        _appJsonService = appJsonService;
        _installLogger = installLogger;
        _globalConfiguration = globalConfiguration;
        _hostSettingsService = hostSettingsService;
        logStore.Add(LogNames.LogStoreInstallation, Log);
    }

}
