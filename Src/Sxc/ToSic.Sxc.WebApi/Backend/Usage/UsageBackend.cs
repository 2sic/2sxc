using ToSic.Eav.Security.Internal;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Errors;
using ToSic.Sxc.Apps.Internal;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Backend.Usage;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class UsageBackend(
    GenWorkPlus<WorkBlocks> appBlocks,
    GenWorkPlus<WorkViews> workViews,
    Generator<MultiPermissionsApp> appPermissions,
    ISxcContextResolver ctxResolver)
    : ServiceBase("Bck.Usage", connect: [appPermissions, ctxResolver, workViews, appBlocks])
{
    public IEnumerable<ViewDto> ViewUsage(int appId, Guid guid, Func<List<IView>, List<BlockConfiguration>, IEnumerable<ViewDto>> finalBuilder)
    {
        var l = Log.Fn<IEnumerable<ViewDto>>($"{appId}, {guid}");
        var context = ctxResolver.GetBlockOrSetApp(appId);

        // extra security to only allow zone change if host user
        var permCheck = appPermissions.New().Init(context, context.AppReader);
        if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
            throw HttpException.PermissionDenied(error);

        var appWorkCtxPlus = appBlocks.CtxSvc.ContextPlus(appId);
        var appViews = workViews.New(appWorkCtxPlus);
        // treat view as a list - in case future code will want to analyze many views together
        var views = new List<IView> { appViews.Get(guid) };

        var blocks = appBlocks.New(appWorkCtxPlus).AllWithView();

        Log.A($"Found {blocks.Count} content blocks");

        var result = finalBuilder(views, blocks);

        return l.ReturnAsOk(result);
    }
}