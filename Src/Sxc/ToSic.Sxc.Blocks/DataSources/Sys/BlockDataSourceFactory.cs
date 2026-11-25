using ToSic.Eav.Apps.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Eav.Services;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;

namespace ToSic.Sxc.DataSources.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockDataSourceFactory(LazySvc<IDataSourcesService> dataSourceFactory, Generator<Query, Query.Options> queryGen)
    : ServiceBase("Sxc.BDsFct", connect: [dataSourceFactory, queryGen])
{

    internal IDataSource GetContextDataSourceFromView(BlockSpecs block, ILookUpEngine? configLookUp)
    {
        var view = block.View;
        var l = Log.Fn<IDataSource>($"mid:{block.Context.Module.Id}, userMayEdit:{block.Context.Permissions.IsContentAdmin}, view:{view?.Name}");

        l.A("Will get Default data source");
        var dsFactory = dataSourceFactory.Value;
        var options = new DataSourceOptions
        {
            AppIdentityOrReader = block.PureIdentity(),
            LookUp = configLookUp,
        };
        var initialSource = dsFactory.CreateDefault(options);
        var blockDataSource = dsFactory.Create<CmsBlock>(options with { Attach = initialSource });

        blockDataSource.OverrideView = view;
        blockDataSource.UseSxcInstanceContentGroup = true;

        // If the Template has a Data-Pipeline, use an empty upstream to attach later, else use the ModuleDataSource created above
        var queryEntity = view?.GetQueryEntity();
        var viewDataSourceUpstream = queryEntity == null
            ? blockDataSource
            : null;
        l.A($"use query upstream:{viewDataSourceUpstream != null}");

        l.A($"Will get ModuleDataSource, aka {nameof(ContextData)}");
        var contextDataSource = dsFactory.Create<ContextData>(options: options with { Attach = viewDataSourceUpstream });
        contextDataSource.SetBlock(blockDataSource);

        // Take Publish-Properties from the View-Template
        if (view != null)
        {
            l.A($"use query from  view, query#{queryEntity?.EntityId}");

            // Append Streams of the Data-Query (this doesn't require a change of the viewDataSource itself)
            if (queryEntity == null)
                return l.ReturnAsOk(contextDataSource);

            l.A("Generate query");
            var query = queryGen.New(new(block.App.PureIdentity(), queryEntity, configLookUp));
            l.A("attaching");
            contextDataSource.SetOut(query);
        }
        else
            l.A("no template override");

        return l.ReturnAsOk(contextDataSource);
    }
}