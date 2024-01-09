using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Query;
using ToSic.Eav.LookUp;
using ToSic.Eav.Services;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ServiceBase = ToSic.Lib.Services.ServiceBase;

namespace ToSic.Sxc.DataSources.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
    internal IBlockRun GetContextDataSource(IBlock block, ILookUpEngine configLookUp)
    {
        var wrapLog = Log.Fn<IBlockRun>($"mid:{block.Context.Module.Id}, userMayEdit:{block.Context.UserMayEdit}, view:{block.View?.Name}");
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

        var contextDataSource = dsFactory.Create<ContextData>(attach: viewDataSourceUpstream, options: new DataSourceOptions(appIdentity: block, lookUp: configLookUp));
        contextDataSource.SetBlock(blockDataSource);

        // Take Publish-Properties from the View-Template
        if (view != null)
        {
            // Note: Deprecated feature in v13, remove ca. 14 - should warn
            // TODO: #WarnDeprecated
#if NETFRAMEWORK
            if (contextDataSource is Internal.Compatibility.IBlockDataSource old)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                old.Publish.Enabled = view.PublishData;
                old.Publish.Streams = view.StreamsToPublish;
#pragma warning restore CS0618 // Type or member is obsolete
            }
#endif
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