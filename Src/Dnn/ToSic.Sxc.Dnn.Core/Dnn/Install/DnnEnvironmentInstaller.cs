using ToSic.Eav.Internal.Configuration;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Dnn.Install;

internal partial class DnnEnvironmentInstaller: ServiceBase, IEnvironmentInstaller
{
    public static bool SaveUnimportantDetails = true;

    private readonly DnnInstallLogger _installLogger;
    private readonly LazySvc<IGlobalConfiguration> _globalConfiguration;

    /// <summary>
    /// Instance initializers...
    /// </summary>
    public DnnEnvironmentInstaller(ILogStore logStore, DnnInstallLogger installLogger, LazySvc<IGlobalConfiguration> globalConfiguration) : base("Dnn.InstCo")
    {
        logStore.Add(LogNames.LogStoreInstallation, Log);
        ConnectServices(
            _installLogger = installLogger,
            _globalConfiguration = globalConfiguration
        );
    }
        
}