using System;
using ToSic.Eav;
using ToSic.Eav.Apps;

namespace ToSic.Sxc.WebApi.Assets
{
    internal partial class AppAssetsBackend
    {
        private string ResolveAppPath(int appId, bool global, bool allowFullAccess)
        {
            var thisApp = Factory.Resolve<Apps.App>().InitNoData(new AppIdentity(Eav.Apps.App.AutoLookupZone, appId), Log);

            if (global && !allowFullAccess)
                throw new NotSupportedException("only host user may access global files");

            return _tmplHelpers.Init(thisApp, Log).AppPathRoot(global);
        }
    }
}
