using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;

namespace ToSic.Sxc.Dnn.Install
{
    public class InstallVersion
    {
        protected ILog Log;

        protected DnnInstallLogger logger;
        protected string _version;
        internal InstallVersion(string version, DnnInstallLogger sharedLogger, string logName, ILog parentLog = null)
        {
            logger = sharedLogger;
            _version = version;
            logger.LogStep(_version, "Initialized version-upgrade object for " + this.GetType().Name);

            Log = new Log(logName, parentLog);
        }


    }
}