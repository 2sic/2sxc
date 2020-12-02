using System;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;


namespace ToSic.Sxc.WebApi
{
    public abstract class BlockWebApiBackendBase<T>: WebApiBackendBase<BlockWebApiBackendBase<T>> where T: class
    {
        public IContextResolver CtxResolver { get; }
        protected readonly Lazy<CmsManager> CmsManagerLazy;

        protected IContextOfApp ContextOfBlock =>
            _contextOfAppOrBlock ?? (_contextOfAppOrBlock = CtxResolver.BlockRequired());
        private IContextOfApp _contextOfAppOrBlock;
        #region Block-Context Requiring properties

        protected IBlock RealBlock => _realBlock ?? (_realBlock = CtxResolver.RealBlockRequired());
        private IBlock _realBlock;

        protected CmsManager CmsManagerOfBlock => _cmsManager ?? (_cmsManager = CmsManagerLazy.Value.Init(RealBlock.Context, Log));
        private CmsManager _cmsManager;

        #endregion


        protected BlockWebApiBackendBase(IServiceProvider sp, Lazy<CmsManager> cmsManagerLazy, IContextResolver ctxResolver, string logName) : base(sp, logName)
        {
            CtxResolver = ctxResolver;
            CmsManagerLazy = cmsManagerLazy;
        }

        public new T Init(ILog parentLog)
        {
            Log.LinkTo(parentLog);
            return this as T;
        }

        protected void ThrowIfNotAllowedInApp(List<Grants> requiredGrants, IAppIdentity alternateApp = null)
        {
            var permCheck = ServiceProvider.Build<MultiPermissionsApp>().Init(ContextOfBlock, alternateApp ?? ContextOfBlock.AppState, Log);
            if (!permCheck.EnsureAll(requiredGrants, out var error))
                throw HttpException.PermissionDenied(error);
        }
    }
}
