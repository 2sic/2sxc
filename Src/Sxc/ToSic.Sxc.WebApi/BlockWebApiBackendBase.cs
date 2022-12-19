using System;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.Security;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Errors;
using ToSic.Lib.DI;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi
{
    public abstract class BlockWebApiBackendBase<T>: WebApiBackendBase<BlockWebApiBackendBase<T>> where T: class
    {
        public IContextResolver CtxResolver { get; }
        protected readonly LazyInitLog<CmsManager> CmsManagerLazy;

        protected IContextOfApp ContextOfBlock =>
            _contextOfAppOrBlock ?? (_contextOfAppOrBlock = CtxResolver.BlockRequired());
        private IContextOfApp _contextOfAppOrBlock;
        #region Block-Context Requiring properties

        public IBlock Block => _block ?? (_block = CtxResolver.RealBlockRequired());
        private IBlock _block;

        protected CmsManager CmsManagerOfBlock => _cmsManager ?? (_cmsManager = CmsManagerLazy.Value.Init(Block.Context));
        private CmsManager _cmsManager;

        #endregion


        protected BlockWebApiBackendBase(IServiceProvider sp,
            LazyInitLog<CmsManager> cmsManagerLazy,
            IContextResolver ctxResolver, string logName
            ) : base(sp, logName)
            => ConnectServices(
                CtxResolver = ctxResolver,
                CmsManagerLazy = cmsManagerLazy
            );

        protected void ThrowIfNotAllowedInApp(List<Grants> requiredGrants, IAppIdentity alternateApp = null)
        {
            var permCheck = GetService<MultiPermissionsApp>().Init(ContextOfBlock, alternateApp ?? ContextOfBlock.AppState, Log);
            if (!permCheck.EnsureAll(requiredGrants, out var error))
                throw HttpException.PermissionDenied(error);
        }
    }
}
