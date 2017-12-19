using ToSic.Eav.Logging.Simple;

namespace ToSic.SexyContent.Installer
{
    public class VersionBase
    {
        protected Log Log;

        protected Logger logger;
        protected string _version;
        internal VersionBase(string version, Logger sharedLogger, string logName, Log parentLog = null)
        {
            logger = sharedLogger;
            _version = version;
            logger.LogStep(_version, "Initialized version-upgrade object for " + this.GetType().Name);

            Log = new Log(logName, parentLog);
        }


    }
}