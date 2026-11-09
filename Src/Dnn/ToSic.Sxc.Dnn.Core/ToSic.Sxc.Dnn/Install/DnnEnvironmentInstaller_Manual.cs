using System.Web;
using static ToSic.Sxc.Dnn.DnnSxcSettings.Installation;

namespace ToSic.Sxc.Dnn.Install;

partial class DnnEnvironmentInstaller
{

    public bool ResumeAbortedUpgrade()
    {
        var l = Log.Fn<bool>();
        if (IsUpgradeRunning)
        {
            l.A("Upgrade is still running");
            throw l.Done(new Exception("There seems to be an upgrade running - please wait. If you still see this message after 3-4 minutes, please restart the web application."));
        }

        var logger = new DnnInstallLoggerForVersion(_installLogger, "");
        logger.LogUnimportant("FinishAbortedUpgrade starting");
        logger.LogAuto("Will handle " + UpgradeVersionList.Length + " versions");
        // Run upgrade again for all versions that do not have a corresponding logfile
        foreach (var upgradeVersion in UpgradeVersionList)
        {
            var complete = IsUpgradeComplete(upgradeVersion, true, "- check for FinishAbortedUpgrade");
            logger.LogAuto($"Upgrade marker for {upgradeVersion}: {(complete ? "exists" : "not found")}");
            if (!complete)
                UpgradeModule(upgradeVersion, false);
        }

        logger.LogUnimportant("FinishAbortedUpgrade done");

        _installLogger.CloseLogFiles();
        // Restart application
        HttpRuntime.UnloadAppDomain();
        return l.ReturnTrue("ok");
    }

}