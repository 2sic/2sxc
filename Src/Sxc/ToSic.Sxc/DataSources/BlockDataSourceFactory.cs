using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Query;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Eav.LookUp;
using ToSic.Eav.Services;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Data;
using ServiceBase = ToSic.Lib.Services.ServiceBase;

namespace ToSic.Sxc.DataSources
{
    public class BlockDataSourceFactory: ServiceBase
    {
        #region Constructor

        public BlockDataSourceFactory(LazySvc<IDataSourcesService> dataSourceFactory, LazySvc<Query> queryLazy): base("Sxc.BDsFct")
        {
            ConnectServices(
                _dataSourceFactory = dataSourceFactory,
                _queryLazy = queryLazy
            );
        }
        private readonly LazySvc<IDataSourcesService> _dataSourceFactory;
        private readonly LazySvc<Query> _queryLazy;

        #endregion


        [PrivateApi]
        internal IContextData GetContextDataSource(IBlock block, ILookUpEngine configLookUp)
        {
            var wrapLog = Log.Fn<IContextData>($"mid:{block.Context.Module.Id}, userMayEdit:{block.Context.UserMayEdit}, view:{block.View?.Name}");
            var view = block.View;

            // Get ModuleDataSource
            var dsFactory = _dataSourceFactory.Value;
            var initialSource = dsFactory.CreateDefault(new DataSourceOptions(appIdentity: block, lookUp: configLookUp));
            var blockDataSource = dsFactory.Create<CmsBlock>(attach: initialSource);

            blockDataSource.OverrideView = view;
            blockDataSource.UseSxcInstanceContentGroup = true;

            // If the Template has a Data-Pipeline, use an empty upstream to attach later, else use the ModuleDataSource created above
            var viewDataSourceUpstream = view?.Query == null
                ? blockDataSource
                : null;
            Log.A($"use query upstream:{viewDataSourceUpstream != null}");

            var contextDataSource = dsFactory.Create<Block>(attach: viewDataSourceUpstream, options: new DataSourceOptions(appIdentity: block, lookUp: configLookUp));
            contextDataSource.SetBlock(blockDataSource);

            // Take Publish-Properties from the View-Template
            if (view != null)
            {
                // Note: Deprecated feature in v13, remove ca. 14 - should warn
                // TODO: #WarnDeprecated
                contextDataSource.Publish.Enabled = view.PublishData;
                contextDataSource.Publish.Streams = view.StreamsToPublish;

                Log.A($"use template, & query#{view.Query?.Id}");
                // Append Streams of the Data-Query (this doesn't require a change of the viewDataSource itself)
                if (view.Query != null)
                {
                    Log.A("Generate query");
                    var query = _queryLazy.Value.Init(block.App.ZoneId, block.App.AppId, view.Query.Entity, configLookUp, contextDataSource);
                    Log.A("attaching");
                    contextDataSource.SetOut(query);
                }
            }
            else
                Log.A("no template override");

            return wrapLog.ReturnAsOk(contextDataSource);
        }
    }
}
