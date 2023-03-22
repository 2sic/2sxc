using DotNetNuke.Entities.Portals;
using System.IO;
using System.Web.Hosting;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using static ToSic.Sxc.Dnn.DnnSxcSettings.Installation;

namespace ToSic.Sxc.Dnn.Install
{
    public partial class DnnEnvironmentInstaller
    {
        public string UpgradeMessages()
        {
            // Upgrade success check - show message if upgrade did not run successfully
            if (UpgradeComplete(true)) return null;

            return IsUpgradeRunning
                ? "It looks like an upgrade is currently running. Please wait for the operation to complete, the upgrade may take a few minutes."
                : PortalSettings.Current.UserInfo.IsSuperUser
                    ? "Module upgrade did not complete (<a href='http://2sxc.org/en/help/tag/install' target='_blank'>read more</a>). " +
                      "Click to complete: <br><a class='dnnPrimaryAction' onclick='$2sxc.system.finishUpgrade(this)'>complete upgrade</a>"
                    : "Module upgrade did not complete successfully. Please login as host user to finish the upgrade.";
        }

        private bool UpgradeComplete(bool forceCloseFileIfComplete = false) => UpgradeCompleteCache.Get(() => IsUpgradeComplete(LastVersionWithServerChanges, "- first check", forceCloseFileIfComplete));
        private static readonly GetOnce<bool> UpgradeCompleteCache = new GetOnce<bool>();

        private bool IsUpgradeComplete(string version, string note = "", bool forceCloseFileIfComplete = false) => Log.Func(timer: true, message: note, func: () =>
        {
            _installLogger.LogStep(version, "IsUpgradeComplete checking " + note, false);
            var logFilePath = HostingEnvironment.MapPath(DnnConstants.LogDirectory + version + ".resources");
            var complete = File.Exists(logFilePath);
            _installLogger.LogStep(version, "IsUpgradeComplete: " + complete, false);
            if (complete && forceCloseFileIfComplete)
                _installLogger.CloseLogFiles();
            return complete;
        });

        // cache the status
        private static bool? _running;
        /// <summary>
        /// Set / Check if it's running, by storing the static info but also creating/releasing a lock-file
        /// We need the lock file in case another system would try to read the status, which doesn't share
        /// this running static instance
        /// </summary>
        private bool IsUpgradeRunning
        {
            get => _running ?? (_running = new DnnFileLock().IsSet).Value;
            set
            {
                try
                {
                    _installLogger.LogStep("", "set upgrade running - " + value);

                    if (value)
                        new DnnFileLock().Set();
                    else
                        new DnnFileLock().Release();
                    _installLogger.LogStep("", "set upgrade running - " + value + " - done");
                }
                catch
                {
                    _installLogger.LogStep("", "set upgrade running - " + value + " - error!");
                }
                finally
                {
                    _running = value;
                }
            }
        }
    }
}
