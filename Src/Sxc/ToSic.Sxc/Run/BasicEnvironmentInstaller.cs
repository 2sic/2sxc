using ToSic.Eav;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Context;
using ToSic.Sxc.Run.Context;

namespace ToSic.Sxc.Run
{
    public class BasicEnvironmentInstaller: HasLog, IEnvironmentInstaller
    {
        public BasicEnvironmentInstaller() : base($"{LogNames.NotImplemented}.Instll")
        {
        }

        public IEnvironmentInstaller Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }


        public string UpgradeMessages()
        {
            // for now, always assume installation worked
            return null;
        }

        public bool ResumeAbortedUpgrade()
        {
            // don't do anything for now
            // throw new NotImplementedException();
            return true;
        }

        public string GetAutoInstallPackagesUiUrl(ISite site, IModuleInternal module, bool forContentApp)
        {
            return "mvc not implemented #todo #mvc";
        }

    }
}
