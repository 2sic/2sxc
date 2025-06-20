using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Cms.Internal;
using ToSic.Eav.Sys;
using ToSic.Sxc.Context.Internal;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Cms.Internal;
using ToSic.Eav.DataSource;
using ToSic.Eav.Sys;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.LookUp.Internal;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks.Internal;
internal class BlockSpecsHelper
{
    internal static BlockSpecs Init(BlockSpecs specs, IContextOfBlock context, IAppIdentity appId)
    {
        specs = specs with
        {
            Context = context,
            ZoneId = appId.ZoneId,
            AppId = appId.AppId,
        };
        return specs;
    }

    public static BlockSpecs CompleteInit(BlockOfBase parent, BlockServices Services, IBlock? parentBlockOrNull, IBlockIdentifier blockId, int blockNumberUnsureIfNeeded, ILog Log)
    {
        var l = Log.Fn<BlockSpecs>();

        var Specs = parent.Specs with
        {
            ParentBlockOrNull = parentBlockOrNull,
            RootBlock = parentBlockOrNull?.RootBlock ?? parent, // if parent is null, this is the root block

            // Note: this is "just" the module id, not the block id
            //ParentId = Context.Module.Id,
            ContentBlockId = blockNumberUnsureIfNeeded,
        };

        l.A($"parent#{Specs.ParentId}, content-block#{Specs.ContentBlockId}, z#{Specs.ZoneId}, a#{Specs.AppId}");

        switch (Specs.AppId)
        {
            // If specifically no app found, end initialization here
            // Means we have no data, and no BlockBuilder
            case AppConstants.AppIdNotFound or EavConstants.NullId:
                return l.Return(Specs, "stop: app & data are missing");
            // If no app yet, stop now with BlockBuilder created
            case KnownAppsConstants.AppIdEmpty:
                return l.Return(Specs, $"stop a:{Specs.AppId}, container:{Specs.ParentId}, content-group:{Specs.ContentBlockId}");
        }

        l.A("Real app specified, will load App object with Data");

        // note: requires EditAllowed, which isn't ready till App is created
        // 2dm #???
        var appWorkCtxPlus = Services.WorkViews.CtxSvc.ContextPlus(Specs.PureIdentity());
        var config = Services.AppBlocks
            .New(appWorkCtxPlus)
            .GetOrGeneratePreviewConfig(blockId);

        Specs = Specs with
        {
            Configuration = config,
        };

        // handle cases where the content group is missing - usually because of incomplete import
        if (config.DataIsMissing)
            return l.Return(Specs, $"DataIsMissing a:{Specs.AppId}, container:{Specs.ParentId}, content-group:{config.Id}");

        // Get App for this block
        var app = Services.AppLazy.Value;
        app.Init(Specs.Context.Site, Specs.PureIdentity(), new SxcAppDataConfigSpecs { BlockForLookupOrNull = parent });
        Specs = Specs with
        {
            AppOrNull = app,
        };
        l.A("App created");

        // use the content-group template, which already covers stored data + module-level stored settings
        var view = new BlockViewLoader(Log)
            .PickView(Specs, config.View, Services.WorkViews.New(appWorkCtxPlus));

        if (view == null)
            return l.Return(Specs, $"no view; a:{Specs.AppId}, container:{Specs.ParentId}, content-group:{config.Id}");

        Specs = Specs with
        {
            ViewOrNull = view,
        };
        return l.Return(Specs, $"ok a:{Specs.AppId} , container: {Specs.ParentId}, content-group:{config.Id}");
    }
}
