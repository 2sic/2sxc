using System.Collections.Generic;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Eav.Security;
using Factory = ToSic.Eav.Factory;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.WebApi.Security
{
    internal class MultiPermissionsApp: MultiPermissionsBase
    {
        /// <summary>
        /// The current app which will be used and can be re-used externally
        /// </summary>
        public IApp App { get; }

        internal readonly IInstanceContext Context;

        protected readonly ITenant TenantForSecurityCheck;

        protected readonly bool SamePortal;

        public MultiPermissionsApp(IInstanceContext context, IApp app, ILog parentLog) 
            : base("Api.Perms", parentLog)
        {
            var wrapLog = Log.Call($"..., appId: {app.AppId}, ...");
            Context = context;
            App = app;

            SamePortal = Context.Tenant.ZoneId == App.ZoneId;
            TenantForSecurityCheck = SamePortal ? Context.Tenant : Factory.Resolve<IZoneMapper>().Init(Log).TenantOfZone(App.ZoneId);
            wrapLog($"ready for z/a:{app.ZoneId}/{app.AppId} t/z:{App.Tenant.Id}/{Context.Tenant.ZoneId} same:{SamePortal}");
        }

        protected override Dictionary<string, IPermissionCheck> InitializePermissionChecks()
            => InitPermissionChecksForApp();

        protected Dictionary<string, IPermissionCheck> InitPermissionChecksForApp()
            => new Dictionary<string, IPermissionCheck>
            {
                {"App", BuildPermissionChecker()}
            };

        public bool ZoneIsOfCurrentContextOrUserIsSuper(out string error)
        {
            var wrapLog = Log.Call<bool>();
            var zoneSameOrSuperUser = SamePortal || Context.User.IsSuperUser;
            error = zoneSameOrSuperUser ? null: $"accessing app {App.AppId} in zone {App.ZoneId} is not allowed for this user";
            return wrapLog(zoneSameOrSuperUser ? $"SamePortal:{SamePortal} - ok": "not ok, generate error", zoneSameOrSuperUser);
        }



        /// <summary>
        /// Creates a permission checker for an app
        /// Optionally you can provide a type-name, which will be 
        /// included in the permission check
        /// </summary>
        /// <returns></returns>
        protected IPermissionCheck BuildPermissionChecker(IContentType type = null, IEntity item = null)
        {
            Log.Add($"BuildPermissionChecker(type:{type?.Name}, item:{item?.EntityId})");

            // user has edit permissions on this app, and it's the same app as the user is coming from
            return Factory.Resolve<AppPermissionCheck>().ForParts(Context.Clone(TenantForSecurityCheck),
                // new DnnContext(TenantForSecurityCheck, Context.Container, Context.User),
                App, type, item, Log);
        }

    }
}
