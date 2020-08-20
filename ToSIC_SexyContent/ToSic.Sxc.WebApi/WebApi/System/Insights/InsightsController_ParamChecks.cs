

namespace ToSic.Sxc.WebApi.System
{
    public partial class InsightsController 
    {
        private void ThrowIfNotSuperuser()
        {
            if (!PortalSettings.UserInfo.IsSuperUser)
                throw Http.PermissionDenied("requires Superuser permissions");
        }
    }
}