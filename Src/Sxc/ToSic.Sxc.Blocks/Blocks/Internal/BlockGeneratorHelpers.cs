using ToSic.Eav.Apps.Sys;
using ToSic.Eav.DataSource;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.DataSources.Internal;

namespace ToSic.Sxc.Blocks.Internal;

public class BlockGeneratorHelpers(GenWorkPlus<WorkViews> workViews, GenWorkPlus<WorkBlocks> appBlocks, LazySvc<BlockDataSourceFactory> bdsFactoryLazy, LazySvc<App> appLazy)
    : MyServicesBase(connect: [bdsFactoryLazy, appLazy, workViews, appBlocks])
{
    internal LazySvc<App> AppLazy { get; } = appLazy;
    public GenWorkPlus<WorkViews> WorkViews { get; } = workViews;
    public GenWorkPlus<WorkBlocks> AppBlocks { get; } = appBlocks;

    internal IDataSource GetData(BlockSpecs specs)
    {
        //var l = Log.Fn<IDataSource>($"About to load data source with possible app configuration provider. App is probably null: {specs.AppOrNull.Show()}");
        var dataSource = bdsFactoryLazy.Value.GetContextDataSource(specs, specs.AppOrNull?.ConfigurationProvider);
        //return l.Return(dataSource);
        return dataSource;
    }

}