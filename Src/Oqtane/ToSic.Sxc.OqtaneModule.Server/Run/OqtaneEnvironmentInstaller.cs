using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.OqtaneModule.Server.Run
{
    public class OqtaneEnvironmentInstaller: HasLog, IEnvironmentInstaller
    {
        public string UpgradeMessages()
        {
            // for now, always assume installation worked
            return null;
        }

        private bool IsUpgradeRunning => false;

        public bool ResumeAbortedUpgrade()
        {
            // don't do anything for now
            // throw new NotImplementedException();
            return true;
        }

        public string GetAutoInstallPackagesUiUrl(ITenant tenant, IContainer container, bool forContentApp)
        {
            return "mvc not implemented #todo #mvc";
        }

        public OqtaneEnvironmentInstaller() : base("Mvc.Instll")
        {
        }

        public IEnvironmentInstaller Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }
    }
}
