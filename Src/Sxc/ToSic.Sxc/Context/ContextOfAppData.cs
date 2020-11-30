using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Permissions;

namespace ToSic.Sxc.Context
{
    public class ContextOfAppData: ContextOfSite, IContextOfAppData, IHasLog
    {
        public ILog Log { get; } = new Log("Sxc.CtxApp");


        public ContextOfAppData(IServiceProvider serviceProvider, ISite site, IUser user) : base(serviceProvider, site, user)
        {
        }

        public void InitApp(IAppIdentity appIdentity, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            AppIdentity = appIdentity;
            _showDrafts = null;
        }
        internal IAppIdentity AppIdentity;

        public bool EditAllowed
        {
            get
            {
                if (_showDrafts.HasValue) return _showDrafts.Value;
                if (AppIdentity == null) throw new Exception("AppIdentity can't be null");
                return (_showDrafts = ServiceProvider.Build<AppPermissionCheck>()
                    .ForAppInInstance(this, AppIdentity, Log).UserMay(GrantSets.WriteSomething)).Value;
            }
        }
        private bool? _showDrafts;

    }
}
