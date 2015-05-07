using System;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Tokens;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using ToSic.Eav.ValueProvider;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    public class TokenReplaceEav: ToSic.Eav.Tokens.TokenReplace// DotNetNuke.Services.Tokens.TokenReplace
    {
        //public Dictionary<string, IValueProvider> ValueSources = new Dictionary<string, IValueProvider>(StringComparer.OrdinalIgnoreCase);
        public int ModuleId;
        public PortalSettings PortalSettings;

        public TokenReplaceEav(App app, int moduleId, PortalSettings portalSettings, IValueCollectionProvider provider) : base()
        {
            InitAppAndPortalSettings(app, moduleId, portalSettings, provider);
        }

        public void InitAppAndPortalSettings(App app, int moduleId, PortalSettings portalSettings, IValueCollectionProvider provider)
        {
            foreach (var valueProvider in provider.Sources)
                ValueSources.Add(valueProvider.Key, valueProvider.Value);

            //// only add these in running inside an http-context. Otherwise leave them away!
            //if (HttpContext.Current != null)
            //{
            //    var request = HttpContext.Current.Request;
            //    ValueSources.Add("querystring", new FilteredNameValueCollectionPropertyAccess(request.QueryString));
            //    ValueSources.Add("server", new FilteredNameValueCollectionPropertyAccess(request.ServerVariables));
            //    ValueSources.Add("form", new FilteredNameValueCollectionPropertyAccess(request.Form));
            //}

            //ValueSources.Add("app", new AppPropertyAccess("app", app));
            ////if (app.Settings != null)
            ////    ValueSources.Add("appsettings", new DynamicEntityPropertyAccess("appsettings", app.Settings));
            ////if (app.Resources != null)
            ////    ValueSources.Add("appresources", new DynamicEntityPropertyAccess("appresources", app.Resources));

            ModuleId = moduleId;
            PortalSettings = portalSettings;  
        }

    }
}