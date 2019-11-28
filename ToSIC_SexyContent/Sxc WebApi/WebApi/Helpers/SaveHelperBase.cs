using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.WebApi
{
    /// <summary>
    /// All save helpers usually need the sxc-instance and the log
    /// </summary>
    internal abstract class SaveHelperBase: HasLog
    {
        internal ICmsBlock CmsInstance;

        protected SaveHelperBase(ICmsBlock cmsInstance, ILog parentLog, string logName) : base(logName, parentLog)
        {
            CmsInstance = cmsInstance;
        }

    }
}
