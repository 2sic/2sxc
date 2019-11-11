using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Apps
{
    /// <inheritdoc />
    /// <summary>
    /// Views manager for the app engine - in charge of importing / modifying templates at app-level
    /// </summary>
    public class CmsManagerBase: ManagerBase
    {

        protected internal readonly CmsManager CmsManager;

        public CmsManagerBase(CmsManager cmsManager, ILog parentLog, string logRename) : base(cmsManager, parentLog, logRename)
        {
            CmsManager = cmsManager;
        }


    }
}
