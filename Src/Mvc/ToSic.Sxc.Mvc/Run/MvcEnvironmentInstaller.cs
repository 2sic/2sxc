using ToSic.Sxc.Interfaces;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcEnvironmentInstaller: IEnvironmentInstaller
    {
        public string UpgradeMessages()
        {
            // for now, always assume installation worked
            return null;
        }

        public bool IsUpgradeRunning => false;

        public void ResumeAbortedUpgrade()
        {
            // don't do anything for now
            // throw new NotImplementedException();
        }
    }
}
