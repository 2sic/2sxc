using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;

namespace ToSic.SexyContent.WebApi.SaveHelpers
{
    /// <summary>
    /// All save helpers usually need the sxc-instance and the log
    /// </summary>
    internal abstract class SaveHelperBase: HasLog
    {
        internal SxcInstance SxcInstance;

        protected SaveHelperBase(SxcInstance sxcInstance, Log parentLog, string logName) : base(logName, parentLog)
        {
            SxcInstance = sxcInstance;
        }

    }
}
