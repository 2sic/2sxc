using System.Globalization;
using System.Threading;
using System.Web;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.Eav.ValueProvider;
using ToSic.SexyContent.Engines.TokenEngine;

namespace ToSic.SexyContent.DataSources
{
    public class ConfigurationProvider
    {
        // note: not sure yet where the best place for this method is, so it's here for now
        // will probably move again some day
        internal static ValueCollectionProvider GetConfigProviderForModule(int moduleId, /*PortalSettings portalSettings,*/ SexyContent.App App)
        {
            var portalSettings = PortalSettings.Current;

            var provider = new ValueCollectionProvider();

            // only add these in running inside an http-context. Otherwise leave them away!
            if (HttpContext.Current != null)
            {
                var request = HttpContext.Current.Request;
                provider.Sources.Add("querystring", new FilteredNameValueCollectionPropertyAccess("querystring", request.QueryString));
                provider.Sources.Add("server", new FilteredNameValueCollectionPropertyAccess("server", request.ServerVariables));
                provider.Sources.Add("form", new FilteredNameValueCollectionPropertyAccess("form", request.Form));
            }

            // Add the standard DNN property sources if PortalSettings object is available
            if (portalSettings != null)
            {
                var dnnUsr = portalSettings.UserInfo;
                var dnnCult = Thread.CurrentThread.CurrentCulture;
                var dnn = new TokenReplaceDnn(App, moduleId, portalSettings, dnnUsr);
                var stdSources = dnn.PropertySources;
                foreach (var propertyAccess in stdSources)
                    provider.Sources.Add(propertyAccess.Key,
                        new ValueProviderWrapperForPropertyAccess(propertyAccess.Key, propertyAccess.Value, dnnUsr, dnnCult));
            }

            provider.Sources.Add("app", new AppPropertyAccess("app", App));

            // add module if it was not already added previously
            if (!provider.Sources.ContainsKey("module"))
            {
                var modulePropertyAccess = new StaticValueProvider("module");
                modulePropertyAccess.Properties.Add("ModuleID", moduleId.ToString(CultureInfo.InvariantCulture));
                provider.Sources.Add(modulePropertyAccess.Name, modulePropertyAccess);
            }
            var _valueCollectionProvider = provider;
            return _valueCollectionProvider;
        }
    }
}