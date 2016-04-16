namespace ToSic.SexyContent.Installer
{
    public class VersionBase
    {
        protected Logger logger;
        protected string _version;
        internal VersionBase(string version, Logger sharedLogger)
        {
            logger = sharedLogger;
            _version = version;
            logger.LogStep(_version, "Initialized version-upgrade object for " + this.GetType().Name);
        }


    }
}