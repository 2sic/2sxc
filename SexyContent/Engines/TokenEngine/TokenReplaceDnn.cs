using System;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Tokens;
using System.Collections;
using System.Collections.Generic;
using System.Web;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    public class TokenReplaceDnn : DotNetNuke.Services.Tokens.TokenReplace
    {
        public TokenReplaceDnn(App app, int moduleId)//, PortalSettings portalSettings)
        {
            var xApp = app;
            //ModuleId = moduleId;
            // throw new NotImplementedException();

            //if (HttpContext.Current != null)
            //{
            //    var request = HttpContext.Current.Request;
            //    PropertySource.Add("querystring", new FilteredNameValueCollectionPropertyAccess(request.QueryString));
            //    PropertySource.Add("server", new FilteredNameValueCollectionPropertyAccess(request.ServerVariables));
            //    PropertySource.Add("form", new FilteredNameValueCollectionPropertyAccess(request.Form));
            //}

            //PropertySource.Add("app", new AppPropertyAccess("app", app));
            //if (app.Settings != null)
            //    PropertySource.Add("appsettings", new DynamicEntityPropertyAccess("appsettings", app.Settings));
            //if (app.Resources != null)
            //    PropertySource.Add("appresources", new DynamicEntityPropertyAccess("appresources", app.Resources));

            ModuleId = moduleId;
            //PortalSettings = portalSettings;  
            ReplaceTokens("InitializePropertySources");

        }

        public Dictionary<string, IPropertyAccess> PropertySources
        {
            get { return PropertySource; }
        }

        public void AddPropertySource(string name, IPropertyAccess value)
        {
            PropertySource.Add(name, value);
        }

        public IPropertyAccess RemovePropertySource(string name)
        {
            var source = GetPropertySource(name);
            if (source != null)
            { 
                PropertySource.Remove(name);
            }
            return source;
        }

        public IPropertyAccess GetPropertySource(string name)
        {
            if (PropertySource.ContainsKey(name))
            { 
                return PropertySource[name];
            }
            return null;
        }
    }
}