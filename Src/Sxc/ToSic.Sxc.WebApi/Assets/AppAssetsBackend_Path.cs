using System;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.WebApi.Assets
{
    internal partial class AppAssetsBackend
    {
        private string ResolveAppPath(int appId, bool global, bool allowFullAccess)
        {
            var thisApp = Factory.Resolve<Apps.App>().InitNoData(new AppIdentity(Eav.Apps.App.AutoLookupZone, appId), Log);

            if (global && !allowFullAccess)
                throw new NotSupportedException("only host user may access global files");

            var appPath = TemplateHelpers.GetTemplatePathRoot(
                global
                    ? Settings.TemplateLocations.HostFileSystem
                    : Settings.TemplateLocations.PortalFileSystem
                , thisApp); // get root in global system

            appPath = _http.MapPath(appPath);
            return appPath;
        }
    }
}
