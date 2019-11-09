using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Pipeline;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.LookUp;
using ToSic.SexyContent;
using ToSic.SexyContent.DataSources;

namespace ToSic.Sxc.Blocks
{
    public class BlockDataSource : PassThrough, IBlockDataSource
    {
        public override string LogId => "DS.View";

        public DataPublishing Publish { get; }= new DataPublishing();

        internal static IBlockDataSource ForContentGroupInSxc(ICmsBlock cms, IView overrideView, ITokenListFiller configurationProvider, ILog parentLog, int instanceId = 0)
        {
            var log = new Log("DS.CreateV", parentLog, "will create view data source");
            var showDrafts = cms.UserMayEdit;

            log.Add($"mid#{instanceId}, draft:{showDrafts}, template:{overrideView?.Name}");
            // Get ModuleDataSource
            var initialSource = DataSource.GetInitialDataSource(cms.ZoneId, cms.AppId, showDrafts, configurationProvider, parentLog);
            var moduleDataSource = DataSource.GetDataSource<ModuleDataSource>(cms.ZoneId, cms.AppId, initialSource, configurationProvider, parentLog);
            moduleDataSource.InstanceId = instanceId;

            moduleDataSource.OverrideView = overrideView; // new
            moduleDataSource.UseSxcInstanceContentGroup = true; // new

            // If the Template has a Data-Pipeline, use an empty upstream, else use the ModuleDataSource created above
            var viewDataSourceUpstream = overrideView?.Query == null
                ? moduleDataSource
                : null;
            log.Add($"use pipeline upstream:{viewDataSourceUpstream != null}");

            var viewDataSource = DataSource.GetDataSource<BlockDataSource>(cms.ZoneId, cms.AppId, viewDataSourceUpstream, configurationProvider, parentLog);

            // Take Publish-Properties from the View-Template
            if (overrideView != null)
            {
                viewDataSource.Publish.Enabled = overrideView.PublishData;
                viewDataSource.Publish.Streams = overrideView.StreamsToPublish;

                log.Add($"override template, & pipe#{overrideView.Query?.EntityId}");
                // Append Streams of the Data-Pipeline (this doesn't require a change of the viewDataSource itself)
                if (overrideView.Query != null)
                    new QueryFactory(parentLog).GetAsDataSource(cms.AppId // 2019-11-09, Id not nullable any more // ?? -999
                        , overrideView.Query,
                        configurationProvider, viewDataSource, showDrafts: showDrafts);

            }
            else
                log.Add("no template override");

            return viewDataSource;
        }
    }
}