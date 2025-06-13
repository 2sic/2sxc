using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class BlockWebApiBackendBase(
    Generator<MultiPermissionsApp> multiPermissionsApp,
    AppWorkContextService appWorkCtxService,
    ISxcCurrentContextService ctxService,
    string logName,
    object[] connect = default)
    : ServiceBase(logName, connect: [..connect ?? [], multiPermissionsApp, ctxService, appWorkCtxService])
{
    public AppWorkContextService AppWorkCtxService { get; } = appWorkCtxService;
    public ISxcCurrentContextService CtxService { get; } = ctxService;

    protected IContextOfApp ContextOfBlock => field ??= CtxService.BlockContextRequired();

    #region Block-Context Requiring properties

    public IBlock Block => field ??= CtxService.BlockRequired();

    protected IAppWorkCtx AppWorkCtx => field ??= AppWorkCtxService.Context(Block.Context.AppReaderRequired);
    protected IAppWorkCtxPlus AppWorkCtxPlus => field ??= AppWorkCtxService.ToCtxPlus(AppWorkCtx);
    protected IAppWorkCtxWithDb AppWorkCtxDb => field ??= AppWorkCtxService.CtxWithDb(AppWorkCtx.AppReader);

    #endregion


    protected void ThrowIfNotAllowedInApp(List<Grants> requiredGrants, IAppIdentity alternateApp = null)
    {
        var permCheck = multiPermissionsApp.New().Init(ContextOfBlock, alternateApp ?? ContextOfBlock.AppReaderRequired);
        if (!permCheck.EnsureAll(requiredGrants, out var error))
            throw HttpException.PermissionDenied(error);
    }
}