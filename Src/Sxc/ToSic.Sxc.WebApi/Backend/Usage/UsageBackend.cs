using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Blocks.Sys.Work;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend.Usage;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class UsageBackend(
    GenWorkPlus<WorkBlocks> appBlocks,
    GenWorkPlus<WorkViews> workViews,
    Generator<MultiPermissionsApp, MultiPermissionsApp.Options> appPermissions,
    ISxcCurrentContextService ctxService)
    : ServiceBase("Bck.Usage", connect: [appPermissions, ctxService, workViews, appBlocks])
{
    public IEnumerable<ViewDto> ViewUsage(int appId, Guid guid, Func<ICollection<IView>, ICollection<BlockConfiguration>, IEnumerable<ViewDto>> finalBuilder)
    {
        var l = Log.Fn<IEnumerable<ViewDto>>($"{appId}, {guid}");
        var context = ctxService.GetExistingAppOrSet(appId);

        // extra security to only allow zone change if host user
        var permCheck = appPermissions.New(new() { SiteContext = context, App = context.AppReaderRequired });
        if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
            throw HttpException.PermissionDenied(error);

        var appWorkCtxPlus = appBlocks.CtxSvc.ContextPlus(appId);
        var appViews = workViews.New(appWorkCtxPlus);
        // treat view as a list - in case future code will want to analyze many views together
        var views = new List<IView> { appViews.Get(guid) };

        var blocks = appBlocks.New(appWorkCtxPlus).AllWithView();

        l.A($"Found {blocks.Count} content blocks");

        var result = finalBuilder(views, blocks);

        return l.ReturnAsOk(result);
    }
}