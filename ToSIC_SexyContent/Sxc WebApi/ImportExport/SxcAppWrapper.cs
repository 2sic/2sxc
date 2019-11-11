using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.Eav.Data;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.SxcTemp;

namespace ToSic.SexyContent.WebApi.ImportExport
{
    public class SxcAppWrapper
    {
        public IApp App { get; }


        public SxcAppWrapper(int appId, bool versioningEnabled)
        {
            App = Environment.Dnn7.Factory.App(appId, versioningEnabled);
        }

        public SxcAppWrapper(int zoneId, int appId)
        {
            App = GetApp.LightWithoutData(new DnnTenant(PortalSettings.Current), zoneId, appId, false, null);
        }


        public IEnumerable<IEntity> GetEntities() => DataSource.GetInitialDataSource(App.ZoneId, App.AppId).List;

        // todo: 2dm Views - probably remove this call, as it should go directly through CmsManager
        public IEnumerable<IView> GetTemplates() => new CmsRuntime(App, null).Views.GetAll();

        public IEnumerable<IView> GetRazorTemplates() => GetTemplates().Where(t => t.IsRazor);

        public IEnumerable<IView> GetTokenTemplates() => GetTemplates().Where(t => !t.IsRazor);

        public string GetVersion() => App.Configuration == null ? "" : App.Configuration.Version;

        public string GetNameWithoutSpecialChars() => Regex.Replace(App.Name, "[^a-zA-Z0-9-_]", "");

    }
}