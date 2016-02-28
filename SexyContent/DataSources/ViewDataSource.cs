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
    public class ViewDataSource : PassThrough
    {
        public DataPublishing Publish = new DataPublishing();

        public static ViewDataSource ForModule(int moduleId, bool showDrafts, Template template, SexyContent sxc)
        {
            var configurationProvider = GetConfigProviderForModule(moduleId, /*sxc.PortalSettingsOfVisitedPage,*/ sxc.App);

            // Get ModuleDataSource
            var initialSource = DataSource.GetInitialDataSource(sxc.ZoneId, sxc.AppId, showDrafts);
            var moduleDataSource = DataSource.GetDataSource<ModuleDataSource>(sxc.ZoneId, sxc.AppId, initialSource, configurationProvider);
            moduleDataSource.ModuleId = moduleId;
            if (template != null)
                moduleDataSource.OverrideTemplateId = template.TemplateId;
            moduleDataSource.Sexy = sxc;

            var viewDataSourceUpstream = moduleDataSource;

            // If the Template has a Data-Pipeline, use it instead of the ModuleDataSource created above
            if (template != null && template.Pipeline != null)
                viewDataSourceUpstream = null;

            var viewDataSource = DataSource.GetDataSource<ViewDataSource>(sxc.ZoneId, sxc.AppId, viewDataSourceUpstream, configurationProvider);

            // Take Publish-Properties from the View-Template
            if (template != null)
            {
                viewDataSource.Publish.Enabled = template.PublishData;
                viewDataSource.Publish.Streams = template.StreamsToPublish;

                // Append Streams of the Data-Pipeline (this doesn't require a change of the viewDataSource itself)
                if (template.Pipeline != null)
                    DataPipelineFactory.GetDataSource(sxc.AppId.Value, template.Pipeline.EntityId, configurationProvider, viewDataSource);
            }

            return viewDataSource;
        }

        // note: this shouldn't be in this view specifically, because it could be used
        // in other stuff not related to this view, so refactor when you find time
        internal static ValueCollectionProvider GetConfigProviderForModule(int moduleId, /*PortalSettings portalSettings,*/ ToSic.SexyContent.App App)
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