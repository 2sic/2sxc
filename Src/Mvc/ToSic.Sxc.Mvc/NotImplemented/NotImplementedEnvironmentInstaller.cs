using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Mvc.NotImplemented
{
    public class NotImplementedEnvironmentInstaller: HasLog, IEnvironmentInstaller
    {
        public NotImplementedEnvironmentInstaller() : base("Mvc.Instll")
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

        public string GetAutoInstallPackagesUiUrl(ISite site, IContainer container, bool forContentApp)
        {
            return "mvc not implemented #todo #mvc";
        }

    }
}
