using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.SexyContent.Environment.Dnn7;

namespace ToSic.SexyContent.WebApi.ImportExport
{
    public class SxcAppWrapper
    {
        public App App { get; }


        public SxcAppWrapper(int appId, bool versioningEnabled)
        {
            App = Environment.Dnn7.Factory.App(appId, versioningEnabled) as App;
        }

        public SxcAppWrapper(int zoneId, int appId)
        {
            App = new App(zoneId, appId, new DnnTenant(PortalSettings.Current), false);
        }


        public IEnumerable<Eav.Interfaces.IEntity> GetEntities() => DataSource.GetInitialDataSource(App.ZoneId, App.AppId).List;

        public IEnumerable<Template> GetTemplates() => App.TemplateManager.GetAllTemplates();

        public IEnumerable<Template> GetRazorTemplates() => GetTemplates().Where(t => t.IsRazor);

        public IEnumerable<Template> GetTokenTemplates() => GetTemplates().Where(t => !t.IsRazor);

        public string GetVersion() => App.Configuration == null ? "" : App.Configuration.Version;

        public string GetNameWithoutSpecialChars() => Regex.Replace(App.Name, "[^a-zA-Z0-9-_]", "");

    }
}