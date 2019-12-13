using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// The main data source for Blocks. Internally often uses <see cref="CmsBlock"/> to find what it should provide.
    /// It's based on the <see cref="PassThrough"/> data source, because it's just a coordination-wrapper.
    /// </summary>
    [PublicApi]
    public class Block : PassThrough, IBlockDataSource
    {
        [PrivateApi]
        public override string LogId => "Sxc.BlckDs";

        [PrivateApi("older use case, probably don't publish")]
        public DataPublishing Publish { get; }= new DataPublishing();

        internal static IBlockDataSource ForContentGroupInSxc(ICmsBlock cms, IView overrideView, ILookUpEngine configurationProvider, ILog parentLog, int instanceId = 0)
        {
            var log = new Log("DS.CreateV", parentLog, "will create view data source");
            var showDrafts = cms.UserMayEdit;

            log.Add($"mid#{instanceId}, draft:{showDrafts}, template:{overrideView?.Name}");
            // Get ModuleDataSource
            var initialSource = DataSource.GetInitialDataSource(cms.Block/*.ZoneId, cms.Block.AppId*/, showDrafts, configurationProvider, parentLog);
            var moduleDataSource = DataSource.GetDataSource<CmsBlock>(/*cms.Block.ZoneId, cms.Block.AppId,*/ initialSource, /*configurationProvider,*/ parentLog);
            moduleDataSource.InstanceId = instanceId;

            moduleDataSource.OverrideView = overrideView;
            moduleDataSource.UseSxcInstanceContentGroup = true;

            // If the Template has a Data-Pipeline, use an empty upstream, else use the ModuleDataSource created above
            var viewDataSourceUpstream = overrideView?.Query == null
                ? moduleDataSource
                : null;
            log.Add($"use pipeline upstream:{viewDataSourceUpstream != null}");

            var viewDataSource = DataSource.GetDataSource<Block>(cms.Block/*.ZoneId, cms.Block.AppId*/, viewDataSourceUpstream, configurationProvider, parentLog);

            // Take Publish-Properties from the View-Template
            if (overrideView != null)
            {
                viewDataSource.Publish.Enabled = overrideView.PublishData;
                viewDataSource.Publish.Streams = overrideView.StreamsToPublish;

                log.Add($"override template, & pipe#{overrideView.Query?.Id}");
                // Append Streams of the Data-Pipeline (this doesn't require a change of the viewDataSource itself)
                if (overrideView.Query != null)
                {
                    // var queryDef = new QueryDefinition(overrideView.QueryRaw, cms.Block.AppId, parentLog);

                    new QueryBuilder(parentLog).GetAsDataSource(overrideView.Query,// queryDef,  /*cms.Block.AppId, overrideView.Query,*/
                        configurationProvider, null, viewDataSource, showDrafts: showDrafts);}

            }
            else
                log.Add("no template override");

            return viewDataSource;
        }
    }
}