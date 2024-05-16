using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Internal.Configuration;
using ToSic.Lib.Services;
using ToSic.Sxc.Integration.Installation;

namespace ToSic.Sxc.Dnn.Install;

internal partial class DnnEnvironmentInstaller : ServiceBase, IEnvironmentInstaller
{
    public static bool SaveUnimportantDetails = true;

    private readonly DnnInstallLogger _installLogger;
    private readonly LazySvc<IGlobalConfiguration> _globalConfiguration;
    private readonly LazySvc<IAppJsonService> _appJsonService;

    /// <summary>
    /// Instance initializers...
    /// </summary>
    public DnnEnvironmentInstaller(ILogStore logStore, DnnInstallLogger installLogger, LazySvc<IGlobalConfiguration> globalConfiguration, LazySvc<IAppJsonService> appJsonService) : base("Dnn.InstCo")
    {
        _appJsonService = appJsonService;
        logStore.Add(LogNames.LogStoreInstallation, Log);
        ConnectLogs([
            _installLogger = installLogger,
            _globalConfiguration = globalConfiguration
        ]);
    }

}