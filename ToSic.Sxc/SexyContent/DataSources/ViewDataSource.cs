using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Pipeline;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.ValueProvider;

namespace ToSic.SexyContent.DataSources
{
    public class ViewDataSource : PassThrough
    {
        public override string LogId => "DS.View";

        public DataPublishing Publish = new DataPublishing();

        internal static ViewDataSource ForContentGroupInSxc(SxcInstance sxc, Template overrideTemplate, ValueCollectionProvider configurationProvider, Log parentLog, int instanceId = 0)
        {
            var log = new Log("DS.CreateV", parentLog, "will create view data source");
            var showDrafts = Factory.Resolve<IPermissions>().UserMayEditContent(sxc.InstanceInfo);

            log.Add($"mid#{instanceId}, draft:{showDrafts}, template:{overrideTemplate?.Name}");
            // Get ModuleDataSource
            var initialSource = DataSource.GetInitialDataSource(sxc.ZoneId, sxc.AppId, showDrafts, configurationProvider, parentLog);
            var moduleDataSource = DataSource.GetDataSource<ModuleDataSource>(sxc.ZoneId, sxc.AppId, initialSource, configurationProvider, parentLog);
            moduleDataSource.InstanceId = instanceId;

            moduleDataSource.OverrideTemplate = overrideTemplate; // new
            moduleDataSource.UseSxcInstanceContentGroup = true; // new

            // If the Template has a Data-Pipeline, use an empty upstream, else use the ModuleDataSource created above
            var viewDataSourceUpstream = overrideTemplate?.Pipeline == null
                ? moduleDataSource
                : null;
            log.Add($"use pipeline upstream:{viewDataSourceUpstream != null}");

            var viewDataSource = DataSource.GetDataSource<ViewDataSource>(sxc.ZoneId, sxc.AppId, viewDataSourceUpstream, configurationProvider, parentLog);

            // Take Publish-Properties from the View-Template
            if (overrideTemplate != null)
            {
                viewDataSource.Publish.Enabled = overrideTemplate.PublishData;
                viewDataSource.Publish.Streams = overrideTemplate.StreamsToPublish;

                log.Add($"override template, & pipe#{overrideTemplate.Pipeline?.EntityId}");
                // Append Streams of the Data-Pipeline (this doesn't require a change of the viewDataSource itself)
                if (overrideTemplate.Pipeline != null)
                    new DataPipelineFactory(parentLog).GetDataSource(sxc.AppId ?? -999, overrideTemplate.Pipeline,
                        configurationProvider, viewDataSource, showDrafts: showDrafts);

            }
            else
                log.Add("no template override");

            return viewDataSource;
        }
    }
}