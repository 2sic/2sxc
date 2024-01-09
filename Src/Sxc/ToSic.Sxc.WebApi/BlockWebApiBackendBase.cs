using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.Security;
using ToSic.Eav.WebApi.Errors;
using ToSic.Lib.DI;
using ToSic.Sxc.Blocks;
using ToSic.Lib.Services;
using ToSic.Eav.Apps.Work;
using IContextResolver = ToSic.Sxc.Context.Internal.IContextResolver;

namespace ToSic.Sxc.WebApi;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class BlockWebApiBackendBase : ServiceBase
{
    protected BlockWebApiBackendBase(
        Generator<MultiPermissionsApp> multiPermissionsApp,
        AppWorkContextService appWorkCtxService,
        IContextResolver ctxResolver,
        string logName
    ) : base(logName)
    {
        ConnectServices(
            _multiPermissionsApp = multiPermissionsApp,
            CtxResolver = ctxResolver,
            AppWorkCtxService = appWorkCtxService
        );
    }


    public AppWorkContextService AppWorkCtxService { get; }
    private readonly Generator<MultiPermissionsApp> _multiPermissionsApp;
    public IContextResolver CtxResolver { get; }

    protected IContextOfApp ContextOfBlock =>
        _contextOfAppOrBlock ??= CtxResolver.BlockContextRequired();
    private IContextOfApp _contextOfAppOrBlock;
    #region Block-Context Requiring properties

    public IBlock Block => _block ??= CtxResolver.BlockRequired();
    private IBlock _block;

    protected IAppWorkCtx AppWorkCtx => _appWorkCtx ??= AppWorkCtxService.Context(Block.Context.AppState);
    private IAppWorkCtx _appWorkCtx;
    protected IAppWorkCtxPlus AppWorkCtxPlus => _appWorkCtxPlus ??= AppWorkCtxService.ToCtxPlus(AppWorkCtx);
    private IAppWorkCtxPlus _appWorkCtxPlus;
    protected IAppWorkCtxWithDb AppWorkCtxDb => _appWorkCtxDb ??= AppWorkCtxService.CtxWithDb(AppWorkCtx.AppState);
    private IAppWorkCtxWithDb _appWorkCtxDb;

    #endregion


    protected void ThrowIfNotAllowedInApp(List<Grants> requiredGrants, IAppIdentity alternateApp = null)
    {
        var permCheck = _multiPermissionsApp.New().Init(ContextOfBlock, alternateApp ?? ContextOfBlock.AppState);
        if (!permCheck.EnsureAll(requiredGrants, out var error))
            throw HttpException.PermissionDenied(error);
    }
}