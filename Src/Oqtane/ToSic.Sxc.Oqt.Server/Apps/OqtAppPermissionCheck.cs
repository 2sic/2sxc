using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Apps
{
    public class OqtPermissionCheck: AppPermissionCheck
    {
        public OqtPermissionCheck(IAppStates appStates, Dependencies dependencies) : base(appStates, dependencies, OqtConstants.OqtLogPrefix)
        { }
    }
}
