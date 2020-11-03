using System.Collections.Generic;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Eav.Security;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.WebApi
{
    public abstract class BlockWebApiBackendBase<T>: HasLog where T: class
    {
        protected IInstanceContext _context;
        protected IBlock _block;
        protected CmsManager _cmsManager;


        protected BlockWebApiBackendBase(string logName) : base(logName) { }

        public T Init(IInstanceContext context, IBlock block, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _context = context;
            _block = block;
            _cmsManager = _block?.App == null ? null : new CmsManager(_block.App, Log);

            return this as T;
        }

        protected void ThrowIfNotAllowedInApp(List<Grants> requiredGrants, IApp alternateApp = null)
        {
            var permCheck = new MultiPermissionsApp().Init(_context, alternateApp ?? _block.App, Log);
            if (!permCheck.EnsureAll(requiredGrants, out var error))
                throw HttpException.PermissionDenied(error);
        }
    }
}
