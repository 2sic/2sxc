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

        internal static ViewDataSource ForContentGroupInSxc(SxcInstance sxc, Template overrideTemplate, int moduleId = 0)
        {
            bool showDrafts = sxc.Environment.Permissions.UserMayEditContent;
            var configurationProvider = DataSources.ConfigurationProvider.GetConfigProviderForModule(moduleId, /*sxc.PortalSettingsOfVisitedPage,*/ sxc.App);

            // Get ModuleDataSource
            var initialSource = DataSource.GetInitialDataSource(sxc.ZoneId, sxc.AppId, showDrafts);
            var moduleDataSource = DataSource.GetDataSource<ModuleDataSource>(sxc.ZoneId, sxc.AppId, initialSource, configurationProvider);
            moduleDataSource.ModuleId = moduleId;

            //if (template != null)
            //{
                //moduleDataSource.OverrideTemplateId = template.TemplateId; // old
            //}
            moduleDataSource.OverrideTemplate = overrideTemplate; // new
            moduleDataSource.UseSxcInstanceContentGroup = true; // new
            moduleDataSource.SxcContext = sxc;

            // If the Template has a Data-Pipeline, use an empty upstream, else use the ModuleDataSource created above
            var viewDataSourceUpstream = (overrideTemplate?.Pipeline == null)
                ? moduleDataSource
                : null;

            var viewDataSource = DataSource.GetDataSource<ViewDataSource>(sxc.ZoneId, sxc.AppId, viewDataSourceUpstream, configurationProvider);

            // Take Publish-Properties from the View-Template
            if (overrideTemplate != null)
            {
                viewDataSource.Publish.Enabled = overrideTemplate.PublishData;
                viewDataSource.Publish.Streams = overrideTemplate.StreamsToPublish;

                // Append Streams of the Data-Pipeline (this doesn't require a change of the viewDataSource itself)
                if (overrideTemplate.Pipeline != null)
                    DataPipelineFactory.GetDataSource(sxc.AppId.Value, overrideTemplate.Pipeline.EntityId, configurationProvider, viewDataSource);
            }

            return viewDataSource;
        }
    }
}