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

        internal static ViewDataSource ForModule(int moduleId, bool showDrafts, Template template, InstanceContext sxc)
        {
            var configurationProvider = DataSources.ConfigurationProvider.GetConfigProviderForModule(moduleId, /*sxc.PortalSettingsOfVisitedPage,*/ sxc.App);

            // Get ModuleDataSource
            var initialSource = DataSource.GetInitialDataSource(sxc.ZoneId, sxc.AppId, showDrafts);
            var moduleDataSource = DataSource.GetDataSource<ModuleDataSource>(sxc.ZoneId, sxc.AppId, initialSource, configurationProvider);
            moduleDataSource.ModuleId = moduleId;
            if (template != null)
                moduleDataSource.OverrideTemplateId = template.TemplateId;
            moduleDataSource.SxcContext = sxc;

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
    }
}