using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Apps.Work;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Errors;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Internal;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Backend.Usage;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class UsageBackend: ServiceBase
{
    private readonly GenWorkPlus<WorkViews> _workViews;
    private readonly GenWorkPlus<WorkBlocks> _appBlocks;
    private readonly Generator<MultiPermissionsApp> _appPermissions;
    private readonly ISxcContextResolver _ctxResolver;

    public UsageBackend(
        GenWorkPlus<WorkBlocks> appBlocks,
        GenWorkPlus<WorkViews> workViews,
        Generator<MultiPermissionsApp> appPermissions,
        ISxcContextResolver ctxResolver
    ) : base("Bck.Usage")
    {
        ConnectServices(
            _appPermissions = appPermissions,
            _ctxResolver = ctxResolver,
            _workViews = workViews,
            _appBlocks = appBlocks
        );
    }

    public IEnumerable<ViewDto> ViewUsage(int appId, Guid guid, Func<List<IView>, List<BlockConfiguration>, IEnumerable<ViewDto>> finalBuilder)
    {
        var wrapLog = Log.Fn<IEnumerable<ViewDto>>($"{appId}, {guid}");
        var context = _ctxResolver.GetBlockOrSetApp(appId);

        // extra security to only allow zone change if host user
        var permCheck = _appPermissions.New().Init(context, context.AppState);
        if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
            throw HttpException.PermissionDenied(error);

        var appWorkCtxPlus = _appBlocks.CtxSvc.ContextPlus(appId);
        var appViews = _workViews.New(appWorkCtxPlus);
        // treat view as a list - in case future code will want to analyze many views together
        var views = new List<IView> { appViews.Get(guid) };

        var blocks = _appBlocks.New(appWorkCtxPlus).AllWithView();

        Log.A($"Found {blocks.Count} content blocks");

        var result = finalBuilder(views, blocks);

        return wrapLog.ReturnAsOk(result);
    }
}