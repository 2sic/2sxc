using ToSic.Eav.Apps.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.Sys;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Sys.Work;
using ToSic.Sxc.Blocks.Sys.Work;
using ToSic.Sxc.DataSources.Sys;
using ToSic.Sxc.LookUp.Sys;

namespace ToSic.Sxc.Blocks.Sys;

public class BlockGeneratorHelpers(GenWorkPlus<WorkViews> workViews, GenWorkPlus<WorkBlocks> appBlocks, LazySvc<BlockDataSourceFactory> bdsFactoryLazy, LazySvc<App> appLazy)
    : ServiceBase("Eav.BlGenH", connect: [bdsFactoryLazy, appLazy, workViews, appBlocks])
{
    internal LazySvc<App> AppLazy { get; } = appLazy;
    public GenWorkPlus<WorkViews> WorkViews { get; } = workViews;
    public GenWorkPlus<WorkBlocks> AppBlocks { get; } = appBlocks;

    public BlockSpecs CompleteInit(BlockSpecs currentSpecs, IBlock? parentOrNull, IBlockIdentifier blockIdentifier, int blockId)
    {
        var l = Log.Fn<BlockSpecs>();

        var specs = currentSpecs with
        {
            ParentBlockOrNull = parentOrNull,
            RootBlock = parentOrNull?.RootBlock!, // if parent is null, this is the root block

            ContentBlockId = blockId,
        };

        l.A($"parent#{specs.ParentId}, content-block#{specs.ContentBlockId}, z#{specs.ZoneId}, a#{specs.AppId}");

        switch (specs.AppId)
        {
            // If specifically no app found, end initialization here
            // Means we have no data, and no BlockBuilder
            case AppConstants.AppIdNotFound or EavConstants.NullId:
                return l.Return(specs, "stop: app & data are missing");
            // If no app yet, stop now with BlockBuilder created
            case KnownAppsConstants.AppIdEmpty:
                return l.Return(specs, $"stop a:{specs.AppId}, container:{specs.ParentId}, content-group:{specs.ContentBlockId}");
        }

        l.A("Real app specified, will load App object with Data");

        // note: requires EditAllowed, which isn't ready till App is created
        // 2dm #???
        var appWorkCtxPlus = WorkViews.CtxSvc.ContextPlus(specs.PureIdentity());
        var config = AppBlocks
            .New(appWorkCtxPlus)
            .GetOrGeneratePreviewConfig(blockIdentifier);

        specs = specs with
        {
            Configuration = config,
        };

        // handle cases where the content group is missing - usually because of incomplete import
        if (config.DataIsMissing)
            return l.Return(specs, $"DataIsMissing a:{specs.AppId}, container:{specs.ParentId}, content-group:{config.Id}");

        // Get App for this block
        var app = AppLazy.Value;
        app.Init(specs.Context.Site, specs.PureIdentity(), new SxcAppDataConfigSpecs { BlockForLookupOrNull = specs });
        specs = specs with
        {
            AppOrNull = app,
        };
        l.A("App created");

        // use the content-group template, which already covers stored data + module-level stored settings
        var view = new BlockViewLoader(Log)
            .PickView(specs, config.View, WorkViews.New(appWorkCtxPlus));

        if (view == null)
            return l.Return(specs, $"no view; a:{specs.AppId}, container:{specs.ParentId}, content-group:{config.Id}");

        specs = specs with
        {
            ViewOrNull = view,
        };

        // Do this after adding view, as it requires the view to continue
        specs = specs with
        {
            Data = GetData(specs),
        };

        return l.Return(specs, $"ok a:{specs.AppId} , container: {specs.ParentId}, content-group:{config.Id}");
    }

    internal IDataSource GetData(BlockSpecs specs)
    {
        //var l = Log.Fn<IDataSource>($"About to load data source with possible app configuration provider. App is probably null: {specs.AppOrNull.Show()}");
        var dataSource = bdsFactoryLazy.Value.GetContextDataSourceFromView(specs, specs.AppOrNull?.ConfigurationProvider);
        //return l.Return(dataSource);
        return dataSource;
    }

}