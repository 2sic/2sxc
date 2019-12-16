using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;

namespace ToSic.Sxc.Dnn.Install
{
    public class VersionBase
    {
        protected ILog Log;

        protected Logger logger;
        protected string _version;
        internal VersionBase(string version, Logger sharedLogger, string logName, ILog parentLog = null)
        {
            logger = sharedLogger;
            _version = version;
            logger.LogStep(_version, "Initialized version-upgrade object for " + this.GetType().Name);

            Log = new Log(logName, parentLog);
        }


    }
}