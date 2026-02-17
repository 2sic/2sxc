using ToSic.Eav.Apps.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Eav.Services;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.DataSources.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockDataSourceFactory(LazySvc<IDataSourcesService> dataSourceFactory, Generator<Query> queryLazy)
    : ServiceBase("Sxc.BDsFct", connect: [dataSourceFactory, queryLazy])
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
        var blockDataSource = dsFactory.Create<CmsBlock>(/*attach: initialSource*/ options with { Attach = initialSource });

        blockDataSource.OverrideView = view;
        blockDataSource.UseSxcInstanceContentGroup = true;

        // If the Template has a Data-Pipeline, use an empty upstream to attach later, else use the ModuleDataSource created above
        var viewDataSourceUpstream = view?.Query == null
            ? blockDataSource
            : null;
        l.A($"use query upstream:{viewDataSourceUpstream != null}");

        l.A($"Will get ModuleDataSource, aka {nameof(ContextData)}");
        var contextDataSource = dsFactory.Create<ContextData>(/*attach: viewDataSourceUpstream,*/
            options: options with { Attach = viewDataSourceUpstream });
        contextDataSource.SetBlock(blockDataSource);

        // Take Publish-Properties from the View-Template
        if (view != null)
        {
            l.A($"use query from  view, query#{view.Query?.Id}");

            // Append Streams of the Data-Query (this doesn't require a change of the viewDataSource itself)
            if (view.Query == null)
                return l.ReturnAsOk(contextDataSource);

            l.A("Generate query");
            var query = queryLazy.New();
            query.Init(block.App.ZoneId, block.App.AppId, (view.Query as ICanBeEntity).Entity, configLookUp, contextDataSource);
            l.A("attaching");
            contextDataSource.SetOut(query);
        }
        else
            l.A("no template override");

        return l.ReturnAsOk(contextDataSource);
    }
}