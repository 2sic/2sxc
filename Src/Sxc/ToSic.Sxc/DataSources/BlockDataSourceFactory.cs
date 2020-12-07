using System;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.DataSources
{
    public class BlockDataSourceFactory: HasLog<BlockDataSourceFactory>
    {
        private readonly DataSourceFactory _dataSourceFactory;
        private readonly Lazy<Query> _queryLazy;

        public BlockDataSourceFactory(DataSourceFactory dataSourceFactory, Lazy<Query> queryLazy): base("Sxc.BDsFct")
        {
            _dataSourceFactory = dataSourceFactory;
            _queryLazy = queryLazy;
        }


        [PrivateApi]
        internal IBlockDataSource GetBlockDataSource(IBlock block, ILookUpEngine configurationProvider)
        {
            // var log = new Log("DS.CreateV", parentLog, "will create view data source");
            var view = block.View;
            var showDrafts = block.Context.UserMayEdit;

            Log.Add($"mid#{block.Context.Module.Id}, draft:{showDrafts}, template:{block.View?.Name}");
            // Get ModuleDataSource
            var dsFactory = _dataSourceFactory.Init(Log); // new DataSource(log);
            //var block = builder.Block;
            var initialSource = dsFactory.GetPublishing(block, showDrafts, configurationProvider);
            var moduleDataSource = dsFactory.GetDataSource<CmsBlock>(initialSource);
            //moduleDataSource.InstanceId = instanceId;

            moduleDataSource.OverrideView = view;
            moduleDataSource.UseSxcInstanceContentGroup = true;

            // If the Template has a Data-Pipeline, use an empty upstream, else use the ModuleDataSource created above
            var viewDataSourceUpstream = view?.Query == null
                ? moduleDataSource
                : null;
            Log.Add($"use pipeline upstream:{viewDataSourceUpstream != null}");

            var viewDataSource = dsFactory.GetDataSource<Block>(block, viewDataSourceUpstream, configurationProvider);

            // Take Publish-Properties from the View-Template
            if (view != null)
            {
                viewDataSource.Publish.Enabled = view.PublishData;
                viewDataSource.Publish.Streams = view.StreamsToPublish;

                Log.Add($"use template, & pipe#{view.Query?.Id}");
                // Append Streams of the Data-Pipeline (this doesn't require a change of the viewDataSource itself)
                if (view.Query != null)
                {
                    Log.Add("Generate query");
                    var query = _queryLazy.Value.Init(block.App.ZoneId, block.App.AppId, view.Query.Entity, configurationProvider, showDrafts, viewDataSource, Log);
                    Log.Add("attaching");
                    viewDataSource.SetOut(query);
                    //viewDataSource.Out = query.Out;
                }
            }
            else
                Log.Add("no template override");

            return viewDataSource;
        }
    }
}
