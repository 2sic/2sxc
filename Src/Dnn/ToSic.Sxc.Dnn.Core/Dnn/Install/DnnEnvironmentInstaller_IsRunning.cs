using System.IO;
using System.Web.Hosting;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Dnn.Install
{
    public partial class DnnEnvironmentInstaller
    {
        internal bool IsUpgradeComplete(string version, string note = "")
        {
            var l = Log.Fn<bool>(startTimer: true);
            _installLogger.LogStep(version, "IsUgradeComplete checking " + note, false);
            var logFilePath = HostingEnvironment.MapPath(DnnConstants.LogDirectory + version + ".resources");
            var complete = File.Exists(logFilePath);
            _installLogger.LogStep(version, "IsUgradeComplete: " + complete, false);
            return l.Return(complete);
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
