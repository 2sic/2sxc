using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtEnvironmentInstaller: HasLog, IEnvironmentInstaller
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

        public string GetAutoInstallPackagesUiUrl(ISite site, IModule container, bool forContentApp)
        {
            return "oqtane not implemented #todo #mvc";
        }

        public OqtEnvironmentInstaller() : base($"{OqtConstants.OqtLogPrefix}.Instll")
        {
        }

        public IEnvironmentInstaller Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }
    }
}
