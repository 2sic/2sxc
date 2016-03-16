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

        internal static ViewDataSource ForModule(int moduleId, bool showDrafts, Template template, SxcInstance sxc)
        {
            var configurationProvider = DataSources.ConfigurationProvider.GetConfigProviderForModule(moduleId, /*sxc.PortalSettingsOfVisitedPage,*/ sxc.App);

            // Get ModuleDataSource
            var initialSource = DataSource.GetInitialDataSource(sxc.ZoneId, sxc.AppId, showDrafts);
            var moduleDataSource = DataSource.GetDataSource<ModuleDataSource>(sxc.ZoneId, sxc.AppId, initialSource, configurationProvider);
            moduleDataSource.ModuleId = moduleId;
            //if (template != null)
            //{
                //moduleDataSource.OverrideTemplateId = template.TemplateId; // old
            //}
            moduleDataSource.OverrideTemplate = template; // new
            moduleDataSource.UseSxcInstanceContentGroup = true; // new
            moduleDataSource.SxcContext = sxc;

            // If the Template has a Data-Pipeline, use an empty upstream, else use the ModuleDataSource created above
            var viewDataSourceUpstream = (template?.Pipeline == null)
                ? moduleDataSource
                : null;

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