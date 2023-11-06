using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.Security;
using ToSic.Eav.WebApi.Errors;
using ToSic.Lib.DI;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Lib.Services;
using ToSic.Eav.Apps.Work;

namespace ToSic.Sxc.WebApi
{
    public abstract class BlockWebApiBackendBase : ServiceBase
    {
        public LazySvc<AppWorkSxc> AppSysSxc { get; }
        private readonly Generator<MultiPermissionsApp> _multiPermissionsApp;
        public Sxc.Context.IContextResolver CtxResolver { get; }

        protected IContextOfApp ContextOfBlock =>
            _contextOfAppOrBlock ?? (_contextOfAppOrBlock = CtxResolver.BlockContextRequired());
        private IContextOfApp _contextOfAppOrBlock;
        #region Block-Context Requiring properties

        public IBlock Block => _block ?? (_block = CtxResolver.BlockRequired());
        private IBlock _block;

        protected IAppWorkCtx AppWorkCtx => _appWorkCtx ?? (_appWorkCtx = AppSysSxc.Value.AppWork.CtxSvc.Context(Block.Context.AppState));
        private IAppWorkCtx _appWorkCtx;
        protected IAppWorkCtxPlus AppWorkCtxPlus => _appWorkCtxPlus ?? (_appWorkCtxPlus = AppSysSxc.Value.AppWork.ToCtxPlus(AppWorkCtx));
        private IAppWorkCtxPlus _appWorkCtxPlus;
        protected IAppWorkCtxWithDb AppWorkCtxDb => _appWorkCtxDb ?? (_appWorkCtxDb = AppSysSxc.Value.AppWork.CtxWithDb(AppWorkCtx.AppState));
        private IAppWorkCtxWithDb _appWorkCtxDb;

        #endregion


        protected BlockWebApiBackendBase(
            Generator<MultiPermissionsApp> multiPermissionsApp,
            LazySvc<AppWorkSxc> appSysSxc,
            Sxc.Context.IContextResolver ctxResolver,
            string logName
            ) : base(logName)
        {
            ConnectServices(
                AppSysSxc = appSysSxc,
                _multiPermissionsApp = multiPermissionsApp,
                CtxResolver = ctxResolver
            );
        }

        protected void ThrowIfNotAllowedInApp(List<Grants> requiredGrants, IAppIdentity alternateApp = null)
        {
            var permCheck = _multiPermissionsApp.New().Init(ContextOfBlock, alternateApp ?? ContextOfBlock.AppState);
            if (!permCheck.EnsureAll(requiredGrants, out var error))
                throw HttpException.PermissionDenied(error);
        }
    }
}
