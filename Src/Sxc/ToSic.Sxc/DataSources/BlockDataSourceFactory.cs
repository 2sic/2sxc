using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Eav.LookUp;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Blocks;
using ServiceBase = ToSic.Lib.Services.ServiceBase;

namespace ToSic.Sxc.DataSources
{
    public class BlockDataSourceFactory: ServiceBase
    {
        #region Constructor

        public BlockDataSourceFactory(LazySvc<DataSourceFactory> dataSourceFactory, LazySvc<Query> queryLazy): base("Sxc.BDsFct")
        {
            ConnectServices(
                _dataSourceFactory = dataSourceFactory,
                _queryLazy = queryLazy
            );
        }
        private readonly LazySvc<DataSourceFactory> _dataSourceFactory;
        private readonly LazySvc<Query> _queryLazy;

        #endregion


        [PrivateApi]
        internal IBlockDataSource GetBlockDataSource(IBlock block, ILookUpEngine configurationProvider)
        {
            var view = block.View;
            var showDrafts = block.Context.UserMayEdit;

            var wrapLog = Log.Fn<IBlockDataSource>($"mid:{block.Context.Module.Id}, draft:{showDrafts}, view:{block.View?.Name}");
            // Get ModuleDataSource
            var dsFactory = _dataSourceFactory.Value;
            var initialSource = dsFactory.GetPublishing(block, showDrafts, configurationProvider);
            var moduleDataSource = dsFactory.Create<CmsBlock>(upstream: initialSource);

            moduleDataSource.OverrideView = view;
            moduleDataSource.UseSxcInstanceContentGroup = true;

            // If the Template has a Data-Pipeline, use an empty upstream, else use the ModuleDataSource created above
            var viewDataSourceUpstream = view?.Query == null
                ? moduleDataSource
                : null;
            Log.A($"use query upstream:{viewDataSourceUpstream != null}");

            var viewDataSource = dsFactory.Create<Block>(appIdentity: block, upstream: viewDataSourceUpstream, configLookUp: configurationProvider);

            // Take Publish-Properties from the View-Template
            if (view != null)
            {
                // Note: Deprecated feature in v13, remove ca. 14 - should warn
                viewDataSource.Publish.Enabled = view.PublishData;
                viewDataSource.Publish.Streams = view.StreamsToPublish;

                Log.A($"use template, & query#{view.Query?.Id}");
                // Append Streams of the Data-Query (this doesn't require a change of the viewDataSource itself)
                if (view.Query != null)
                {
                    Log.A("Generate query");
                    var query = _queryLazy.Value.Init(block.App.ZoneId, block.App.AppId, view.Query.Entity, configurationProvider, showDrafts, viewDataSource);
                    Log.A("attaching");
                    viewDataSource.SetOut(query);
                }
            }
            else
                Log.A("no template override");

            return wrapLog.ReturnAsOk(viewDataSource);
        }
    }
}
