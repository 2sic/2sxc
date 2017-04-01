using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.SexyContent.Environment.Base;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.WebApi
{
    public class SxcAppWrapper
    {
        public App App { get; }


        public SxcAppWrapper(int appId)
        {
            App = Environment.Dnn7.Factory.App(appId) as App;
        }

        public SxcAppWrapper(int zoneId, int appId)
        {
            App = new App(zoneId, appId, PortalSettings.Current, false);
        }


        public IDictionary<int, IEntity> GetEntities() => DataSource.GetInitialDataSource(App.ZoneId, App.AppId).List;

        //public IEnumerable<Culture> GetLanguages() => ZoneHelpers.CulturesWithState(App.OwnerPortalSettings.PortalId, App.ZoneId);

        //public IEnumerable<Culture> GetActiveLanguages() => GetLanguages().Where(c => c.Active);

        public IEnumerable<IContentType> GetContentTypes(string scope = "2SexyContent") => App.TemplateManager.GetAvailableContentTypes(scope, true);

        public IEnumerable<Template> GetTemplates() => App.TemplateManager.GetAllTemplates();

        public IEnumerable<Template> GetRazorTemplates() => GetTemplates().Where(t => t.IsRazor);

        public IEnumerable<Template> GetTokenTemplates() => GetTemplates().Where(t => !t.IsRazor);

        public string GetVersion() => App.Configuration == null ? "" : App.Configuration.Version;

        public string GetNameWithoutSpecialChars() => Regex.Replace(App.Name, "[^a-zA-Z0-9-_]", "");

        public string GetCultureCode() => App.OwnerPortalSettings.CultureCode;
    }
}