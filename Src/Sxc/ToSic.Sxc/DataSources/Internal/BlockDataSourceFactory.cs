using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.LookUp;
using ToSic.Eav.Services;
using ToSic.Lib.DI;
using ToSic.Sxc.Blocks.Internal;
using ServiceBase = ToSic.Lib.Services.ServiceBase;

namespace ToSic.Sxc.DataSources.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class BlockDataSourceFactory(LazySvc<IDataSourcesService> dataSourceFactory, LazySvc<Query> queryLazy)
    : ServiceBase("Sxc.BDsFct", connect: [dataSourceFactory, queryLazy])
{

    internal IBlockInstance GetContextDataSource(IBlock block, ILookUpEngine configLookUp)
    {
        var l = Log.Fn<IBlockInstance>($"mid:{block.Context.Module.Id}, userMayEdit:{block.Context.Permissions.IsContentAdmin}, view:{block.View?.Name}");
        var view = block.View;

        // Get ModuleDataSource
        var dsFactory = dataSourceFactory.Value;
        var initialSource = dsFactory.CreateDefault(new DataSourceOptions(appIdentity: block, lookUp: configLookUp));
        var blockDataSource = dsFactory.Create<CmsBlock>(attach: initialSource);

        blockDataSource.OverrideView = view;
        blockDataSource.UseSxcInstanceContentGroup = true;

        // If the Template has a Data-Pipeline, use an empty upstream to attach later, else use the ModuleDataSource created above
        var viewDataSourceUpstream = view?.Query == null
            ? blockDataSource
            : null;
        l.A($"use query upstream:{viewDataSourceUpstream != null}");

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
            l.A($"use template, & query#{view.Query?.Id}");
            // Append Streams of the Data-Query (this doesn't require a change of the viewDataSource itself)
            if (view.Query != null)
            {
                l.A("Generate query");
                var query = queryLazy.Value.Init(block.App.ZoneId, block.App.AppId, view.Query.Entity, configLookUp, contextDataSource);
                l.A("attaching");
                contextDataSource.SetOut(query);
            }
        }
        else
            l.A("no template override");

        return l.ReturnAsOk(contextDataSource);
    }
}