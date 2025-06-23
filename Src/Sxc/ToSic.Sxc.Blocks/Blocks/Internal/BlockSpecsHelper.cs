using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Cms.Internal;
using ToSic.Eav.Sys;
using ToSic.Sxc.LookUp.Internal;

namespace ToSic.Sxc.Blocks.Internal;
internal class BlockSpecsHelper
{
    public static BlockSpecs CompleteInit(BlockSpecs currentSpecs, /*BlockOfBase parent,*/ BlockGeneratorHelpers helpers, IBlock? parentOrNull, IBlockIdentifier blockIdentifier, int blockId, ILog log)
    {
        var l = log.Fn<BlockSpecs>();

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
        var appWorkCtxPlus = helpers.WorkViews.CtxSvc.ContextPlus(specs.PureIdentity());
        var config = helpers.AppBlocks
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
        var app = helpers.AppLazy.Value;
        app.Init(specs.Context.Site, specs.PureIdentity(), new SxcAppDataConfigSpecs { BlockForLookupOrNull = specs });
        specs = specs with
        {
            AppOrNull = app,
        };
        l.A("App created");

        // use the content-group template, which already covers stored data + module-level stored settings
        var view = new BlockViewLoader(log)
            .PickView(specs, config.View, helpers.WorkViews.New(appWorkCtxPlus));

        if (view == null)
            return l.Return(specs, $"no view; a:{specs.AppId}, container:{specs.ParentId}, content-group:{config.Id}");

        specs = specs with
        {
            ViewOrNull = view,
        };
        return l.Return(specs, $"ok a:{specs.AppId} , container: {specs.ParentId}, content-group:{config.Id}");
    }
}
