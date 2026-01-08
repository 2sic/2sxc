using DotNetNuke.Entities.Portals;
using System.Web.Hosting;
using static ToSic.Sxc.Dnn.DnnSxcSettings.Installation;

namespace ToSic.Sxc.Dnn.Install;

partial class DnnEnvironmentInstaller
{
    public string UpgradeMessages()
    {
        // Upgrade success check - show message if upgrade did not run successfully
        if (UpgradeComplete(false))
            return null;

        return IsUpgradeRunning
            ? "It looks like an upgrade is currently running. Please wait for the operation to complete, the upgrade may take a few minutes."
            : PortalSettings.Current.UserInfo.IsSuperUser
                ? "Module upgrade did not complete (<a href='http://2sxc.org/en/help/tag/install' target='_blank'>read more</a>). " +
                  "Click to complete: <br><a class='dnnPrimaryAction' onclick='$2sxc.system.finishUpgrade(this)'>complete upgrade</a>"
                : "Module upgrade did not complete successfully. Please login as host user to finish the upgrade.";
    }

    private bool UpgradeComplete(bool alwaysLogToFile)
        => UpgradeCompleteCache.Get(() => IsUpgradeComplete(LastVersionWithServerChanges, alwaysLogToFile, "- first check"));
    private static readonly GetOnce<bool> UpgradeCompleteCache = new();

    private bool IsUpgradeComplete(string version, bool alwaysLogToFile, string note = "")
    {
        var l = Log.Fn<bool>(message: $"{note} Log to file even if all is ok: {alwaysLogToFile}", timer: true);
        var logger = new DnnInstallLoggerForVersion(_installLogger, version);

        // 2023-03-23 2dm
        // Previously this created a file on every startup, because it logged trying to find the status.
        // This sometimes resulted in exceptions simply because the file was locked - so to avoid this
        // I'll stop logging to the file by default
        // This should also reduce the amount of files created in the log file.
        // The only downside is that we cannot easily see how often the server restarted.
        var complete = false;
        try
        {
            var versionFilePath = HostingEnvironment.MapPath($"{DnnConstants.LogDirectory}{version}.resources");
            l.A($"Checking upgrade marker: '{versionFilePath}'");

            if (alwaysLogToFile)
                logger.LogUnimportant($"Check upgrade marker file {note}");

            complete = File.Exists(versionFilePath);

            if (alwaysLogToFile || !complete)
            {
                logger.LogAuto($"Upgrade marker {(complete ? "exists" : "not found")}: '{versionFilePath}'");
                logger.LogUnimportant($"Upgrade complete: {complete}");
            }
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            try
            {
                logger.LogAuto("Error checking upgrade marker file (install status unknown)");
            }
            catch { /* ignore */ }
        }
        return l.ReturnAndLog(complete);
    }

    // cache the status
    private static bool? _running;
    /// <summary>
    /// Set / Check if it's running, by storing the static info but also creating/releasing a lock-file
    /// We need the lock file in case another system would try to read the status, which doesn't share
    /// this running static instance
    /// </summary>
    private bool IsUpgradeRunning
    {
        get
        {
            var l = Log.Fn<bool>($"Cached value set: {_running.HasValue}");
            var result = _running ??= new DnnFileLock().IsSet;
            return l.ReturnAndLog(result);
        }

        set
        {
            var logger = new DnnInstallLoggerForVersion(_installLogger, "");
            logger.LogAuto($"Upgrade running flag => {value}");

            try
            {
                var fileLock = new DnnFileLock();
                if (value)
                    fileLock.Set();
                else
                    fileLock.Release();
            }
            catch (Exception ex)
            {
                logger.LogAuto($"Upgrade running flag => {value} (error: {ex.GetType().Name}: {ex.Message})");
                throw;
            }

            _running = value;
            logger.LogAuto($"Upgrade running flag => {value} (done)");
        }
    }
}