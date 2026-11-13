using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Blocks.Sys.Work;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend.Usage;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class UsageBackend(
    AppWorkContextService appCtxSvc,
    AppWorkChain<WorkBlocks> appBlocks,
    AppWorkChain<WorkViewsGet> workViews,
    Generator<MultiPermissionsApp, MultiPermissionsApp.Options> appPermissions,
    ISxcCurrentContextService ctxService)
    : ServiceBase("Bck.Usage", connect: [appCtxSvc, appPermissions, ctxService, workViews, appBlocks])
{
    public (ICollection<IView> Views, ICollection<BlockConfiguration> BlockConfigs) ViewUsage(int appId, Guid guid)
    {
        var l = Log.Fn<(ICollection<IView>, ICollection<BlockConfiguration>)>($"{appId}, {guid}");
        var context = ctxService.ShareAppStateOptimized(appId);

        // extra security to only allow zone change if host user
        var permCheck = appPermissions.New(new() { SiteContext = context, App = context.AppReaderRequired });
        if (permCheck.EnsureAll(GrantSets.ReadSomething) is { Allowed: false } pcResult)
            throw new PermissionDeniedException(pcResult.ErrorMessage); // HttpException.PermissionDenied(pcResult.ErrorMessage);

        var appWorkCtxPlus = appCtxSvc.ContextNew(appId);
        var appViews = workViews.New(appWorkCtxPlus);
        // treat view as a list - in case future code will want to analyze many views together
        var views = new List<IView> { appViews.Get(guid) };

        var blocks = appBlocks.New(appWorkCtxPlus).AllWithView();

        return l.Return((views, blocks), $"Found {blocks.Count} content blocks");
    }
}