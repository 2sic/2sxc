using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Eav.Security;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.LookUp;
using App = ToSic.Sxc.Apps.App;
using Factory = ToSic.Eav.Factory;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Security
{
    internal class MultiPermissionsApp: MultiPermissionsBase
    {
        /// <summary>
        /// The current app which will be used and can be re-used externally
        /// </summary>
        public IApp App { get; }

        //internal readonly IBlockBuilder BlockBuilder;
        internal readonly IInstanceContext Context;

        //protected readonly PortalSettings PortalForSecurityCheck;
        protected readonly ITenant TenantForSecurityCheck;

        protected readonly bool SamePortal;

        public MultiPermissionsApp(IBlockBuilder blockBuilder, IInstanceContext context, int appId, ILog parentLog) :
            this(blockBuilder, context, new AppIdentity(SystemRuntime.ZoneIdOfApp(appId), appId), parentLog) { }

        protected MultiPermissionsApp(IBlockBuilder blockBuilder, IInstanceContext context, IAppIdentity appIdentity,  ILog parentLog) 
            : base("Api.Perms", parentLog)
        {
            var wrapLog = Log.Call($"..., appId: {appIdentity.AppId}, ...");
            // old
            //BlockBuilder = blockBuilder;
            // new
            Context = context;
            App = Factory.Resolve<App>().Init(appIdentity, ConfigurationProvider.Build(blockBuilder, true),
                false, Log);

            SamePortal = Context.Tenant.ZoneId == App.ZoneId;
            // old
            //PortalForSecurityCheck = SamePortal ? PortalSettings.Current : null;
            // new
            TenantForSecurityCheck =
                SamePortal ? Context.Tenant : Factory.Resolve<IZoneMapper>().Init(Log).Tenant(App.ZoneId);
            wrapLog($"ready for z/a:{appIdentity.ZoneId}/{appIdentity.AppId} t/z:{App.Tenant.Id}/{Context.Tenant.ZoneId} same:{SamePortal}");
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
