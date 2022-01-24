using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.WebApi.Assets
{
    public partial class AppAssetsBackend
    {
        private string ResolveAppPath(int appId, bool global, bool allowFullAccess)
        {
            var thisApp = _serviceProvider.Build<Apps.App>().InitNoData(new AppIdentity(AppConstants.AutoLookupZone, appId), Log);

            if (global && !allowFullAccess)
                throw new NotSupportedException("only host user may access global files");

            return _appPathHelpers.Init(thisApp, Log).AppPathRoot(global);
        }
    }
}
