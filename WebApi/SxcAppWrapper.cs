using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using ToSic.Eav;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.WebApi
{
    public class SxcAppWrapper
    {
        public App App { get; private set; }


        public SxcAppWrapper(int appId)
        {
            this.App = Environment.Dnn7.Factory.App(appId) as App;
        }


        public IDictionary<int, IEntity> GetEntities()
        {
            return DataSource.GetInitialDataSource(App.ZoneId, App.AppId).List;
        }

        public IEnumerable<ZoneHelpers.CulturesWithActiveState> GetLanguages()
        {
            return ZoneHelpers.GetCulturesWithActiveState(App.OwnerPortalSettings.PortalId, App.ZoneId);
        }

        public IEnumerable<ZoneHelpers.CulturesWithActiveState> GetActiveLanguages()
        {
            return GetLanguages().Where(c => c.Active);
        }

        public IEnumerable<IContentType> GetContentTypes(string scope = "2SexyContent")
        {
            return App.TemplateManager.GetAvailableContentTypes(scope, true);
        }

        public IEnumerable<Template> GetTemplates()
        {
            return App.TemplateManager.GetAllTemplates();
        }

        public IEnumerable<Template> GetRazorTemplates()
        {
            return GetTemplates().Where(t => t.IsRazor);
        }

        public IEnumerable<Template> GetTokenTemplates()
        {
            return GetTemplates().Where(t => !t.IsRazor);
        }

        public string GetVersion()
        {
            return App.Configuration == null ? "" : App.Configuration.Version;
        }

        public string GetNameWithoutSpecialChars()
        {
            return Regex.Replace(App.Name, "[^a-zA-Z0-9-_]", "");
        }

        public string GetCultureCode()
        {
            return App.OwnerPortalSettings.CultureCode;
        }
    }
}