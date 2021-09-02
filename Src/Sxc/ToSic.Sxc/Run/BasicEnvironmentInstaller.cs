using ToSic.Eav;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Sxc.Context;


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
            return true;
        }

        public string GetAutoInstallPackagesUiUrl(ISite site, IModule module, bool forContentApp)
        {
            return "mvc not implemented #todo #mvc";
        }

    }
}
