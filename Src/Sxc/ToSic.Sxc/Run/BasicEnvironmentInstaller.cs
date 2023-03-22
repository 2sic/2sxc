using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Eav.Run;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Services;
using ToSic.Sxc.Context;


namespace ToSic.Sxc.Run
{
    public class BasicEnvironmentInstaller: ServiceBase, IEnvironmentInstaller, IIsUnknown, IPlatformAppInstaller
    {
        public BasicEnvironmentInstaller(WarnUseOfUnknown<BasicEnvironmentInstaller> _) : base($"{LogScopes.NotImplemented}.Instll")
        {
        }

        public string UpgradeMessages()
        {
            // for now, always assume installation worked
            return null;
        }

        public bool ResumeAbortedUpgrade()
        {
            // don't do anything for now
            return true;
        }

        public string GetAutoInstallPackagesUiUrl(ISite site, IModule module, bool forContentApp)
        {
            return "mvc not implemented #todo #mvc";
        }

    }
}
