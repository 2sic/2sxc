using ToSic.Eav.Apps.Sys.AppJson;
using ToSic.Sxc.Sys.Integration.Installation;
using ToSic.Sys.Configuration;

namespace ToSic.Sxc.Dnn.Install;

internal partial class DnnEnvironmentInstaller : ServiceBase, IEnvironmentInstaller
{
    public static bool SaveUnimportantDetails = true;

    private readonly DnnInstallLogger _installLogger;
    private readonly LazySvc<IGlobalConfiguration> _globalConfiguration;
    private readonly LazySvc<IAppJsonConfigurationService> _appJsonService;

    /// <summary>
    /// Instance initializers...
    /// </summary>
    public DnnEnvironmentInstaller(ILogStore logStore, DnnInstallLogger installLogger, LazySvc<IGlobalConfiguration> globalConfiguration, LazySvc<IAppJsonConfigurationService> appJsonService) : base("Dnn.InstCo")
    {
        _appJsonService = appJsonService;
        logStore.Add(LogNames.LogStoreInstallation, Log);
        ConnectLogs([
            _installLogger = installLogger,
            _globalConfiguration = globalConfiguration
        ]);
    }

}