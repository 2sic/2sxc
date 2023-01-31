using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.Security;
using ToSic.Eav.WebApi.Errors;
using ToSic.Lib.DI;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Lib.Services;

namespace ToSic.Sxc.WebApi
{
    public abstract class BlockWebApiBackendBase : ServiceBase
    {
        private readonly Generator<MultiPermissionsApp> _multiPermissionsApp;
        public IContextResolver CtxResolver { get; }
        protected readonly LazySvc<CmsManager> CmsManagerLazy;

        protected IContextOfApp ContextOfBlock =>
            _contextOfAppOrBlock ?? (_contextOfAppOrBlock = CtxResolver.BlockRequired());
        private IContextOfApp _contextOfAppOrBlock;
        #region Block-Context Requiring properties

        public IBlock Block => _block ?? (_block = CtxResolver.RealBlockRequired());
        private IBlock _block;

        protected CmsManager CmsManagerOfBlock => _cmsManager ?? (_cmsManager = CmsManagerLazy.Value.Init(Block.Context));
        private CmsManager _cmsManager;

        #endregion


        protected BlockWebApiBackendBase(
            Generator<MultiPermissionsApp> multiPermissionsApp,
            LazySvc<CmsManager> cmsManagerLazy,
            IContextResolver ctxResolver, string logName
            ) : base(logName)
        {
            ConnectServices(
                _multiPermissionsApp = multiPermissionsApp,
                CtxResolver = ctxResolver,
                CmsManagerLazy = cmsManagerLazy
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
