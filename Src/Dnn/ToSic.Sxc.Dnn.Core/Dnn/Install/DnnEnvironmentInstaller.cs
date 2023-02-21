using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Dnn.Install
{
    public partial class DnnEnvironmentInstaller: ServiceBase, IEnvironmentInstaller
    {
        public static bool SaveUnimportantDetails = true;

        private readonly DnnInstallLogger _installLogger;
        private readonly LazySvc<IAppStates> _appStatesLazy;
        private readonly LazySvc<CmsRuntime> _cmsRuntimeLazy;
        private readonly LazySvc<RemoteRouterLink> _remoteRouterLazy;
        private readonly LazySvc<IGlobalConfiguration> _globalConfiguration;

        /// <summary>
        /// Instance initializers...
        /// </summary>
        public DnnEnvironmentInstaller(ILogStore logStore, 
            DnnInstallLogger installLogger, 
            LazySvc<IAppStates> appStatesLazy, 
            LazySvc<CmsRuntime> cmsRuntimeLazy, 
            LazySvc<RemoteRouterLink> remoteRouterLazy,
            LazySvc<IGlobalConfiguration> globalConfiguration) : base("Dnn.InstCo")
        {
            logStore.Add(LogNames.LogStoreStartUp, Log);
            ConnectServices(
                _installLogger = installLogger,
                _appStatesLazy = appStatesLazy,
                _cmsRuntimeLazy = cmsRuntimeLazy,
                _remoteRouterLazy = remoteRouterLazy,
                _globalConfiguration = globalConfiguration
            );
        }
        
    }
}