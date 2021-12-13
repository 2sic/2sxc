using DotNetNuke.Entities.Portals;

namespace ToSic.Sxc.Dnn.Install
{
    public partial class DnnEnvironmentInstaller
    {
        public string UpgradeMessages()
        {
            // Upgrade success check - show message if upgrade did not run successfully
            if (UpgradeComplete) return null;

            return IsUpgradeRunning
                ? "It looks like an upgrade is currently running.Please wait for the operation to complete(the upgrade may take a few minutes)."
                : PortalSettings.Current.UserInfo.IsSuperUser
                    ? "Module upgrade did not complete (<a href='http://2sxc.org/en/help/tag/install' target='_blank'>read more</a>). Click to complete: <br><a class='dnnPrimaryAction' onclick='$2sxc.system.finishUpgrade(this)'>complete upgrade</a>"
                    : "Module upgrade did not complete successfully. Please login as host user to finish the upgrade.";
        }
    }
}
