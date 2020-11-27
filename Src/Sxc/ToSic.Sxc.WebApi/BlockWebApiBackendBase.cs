using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Run.Context;

namespace ToSic.Sxc.WebApi
{
    public abstract class BlockWebApiBackendBase<T>: HasLog where T: class
    {
        private readonly Lazy<CmsManager> _cmsManagerLazy;
        protected IContextOfBlock _context;
        protected IBlock _block;
        protected CmsManager CmsManager;
        protected IServiceProvider ServiceProvider;

        protected BlockWebApiBackendBase(Lazy<CmsManager> cmsManagerLazy, string logName) : base(logName)
        {
            _cmsManagerLazy = cmsManagerLazy;
            ServiceProvider = _cmsManagerLazy.Value.ServiceProvider;
        }

        public T Init(IContextOfBlock context, IBlock block, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _context = context;
            _block = block;
            CmsManager = _block?.App == null ? null : _cmsManagerLazy.Value.Init(_block.App, Log);

            return this as T;
        }

        protected void ThrowIfNotAllowedInApp(List<Grants> requiredGrants, IApp alternateApp = null)
        {
            var permCheck = ServiceProvider.Build<MultiPermissionsApp>().Init(_context, alternateApp ?? _block.App, Log);
            if (!permCheck.EnsureAll(requiredGrants, out var error))
                throw HttpException.PermissionDenied(error);
        }
    }
}
