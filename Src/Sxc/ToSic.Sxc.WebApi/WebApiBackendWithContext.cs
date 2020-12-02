using System;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;


namespace ToSic.Sxc.WebApi
{
    public abstract class BlockWebApiBackendWithContext<T>: WebApiBackendBase<BlockWebApiBackendWithContext<T>> where T: class
    {
        private readonly Lazy<CmsManager> _cmsManagerLazy;
        private readonly IContextResolver _ctxResolver;
        protected IContextOfApp ContextOfAppOrBlock;
        protected CmsManager CmsManager;

        protected BlockWebApiBackendWithContext(IServiceProvider sp, Lazy<CmsManager> cmsManagerLazy, IContextResolver ctxResolver, string logName) : base(sp, logName)
        {
            _cmsManagerLazy = cmsManagerLazy;
            _ctxResolver = ctxResolver;
        }

        public T Init(string appName, IContextOfApp context, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            ContextOfAppOrBlock = context;
            CmsManager = context.AppState == null ? null : _cmsManagerLazy.Value.Init(context.AppState, context.UserMayEdit, Log);

            return this as T;
        }

        protected void ThrowIfNotAllowedInApp(List<Grants> requiredGrants, IAppIdentity alternateApp = null)
        {
            var permCheck = ServiceProvider.Build<MultiPermissionsApp>().Init(ContextOfAppOrBlock, alternateApp ?? ContextOfAppOrBlock.AppState, Log);
            if (!permCheck.EnsureAll(requiredGrants, out var error))
                throw HttpException.PermissionDenied(error);
        }
    }
}
