using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.WebApi.Assets
{
    public partial class AppAssetsBackend
    {
        private string ResolveAppPath(int appId, bool global, bool allowFullAccess)
        {
            if (global && !allowFullAccess)
                throw new NotSupportedException("only host user may access global files");

            var thisApp = _serviceProvider.Build<Apps.App>().InitNoData(new AppIdentity(AppConstants.AutoLookupZone, appId), Log);

            return thisApp.PhysicalPathSwitch(global);
        }
    }
}
