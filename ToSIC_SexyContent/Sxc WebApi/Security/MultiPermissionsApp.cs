using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Eav.Security;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.WebApi;
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

        internal readonly IBlockBuilder BlockBuilder;

        protected readonly PortalSettings PortalForSecurityCheck;

        protected readonly bool SamePortal;

        public MultiPermissionsApp(IBlockBuilder blockBuilder, int appId, ILog parentLog) :
            this(blockBuilder, SystemRuntime.ZoneIdOfApp(appId), appId, parentLog) { }

        protected MultiPermissionsApp(IBlockBuilder blockBuilder, int zoneId, int appId, ILog parentLog) 
            : base("Api.Perms", parentLog)
        {
            var wrapLog = Log.Call($"..., appId: {appId}, ...");
            BlockBuilder = blockBuilder;
            var tenant = new DnnTenant(PortalSettings.Current);
            var environment = Factory.Resolve<IEnvironmentFactory>().Environment(Log);
            var contextZoneId = environment.ZoneMapper.GetZoneId(tenant.Id);
            App = new App(tenant, zoneId, appId, 
                ConfigurationProvider.Build(blockBuilder, true),
                false, Log);
            SamePortal = contextZoneId == zoneId;
            PortalForSecurityCheck = SamePortal ? PortalSettings.Current : null;
            wrapLog($"ready for z/a:{zoneId}/{appId} t/z:{tenant.Id}/{contextZoneId} same:{SamePortal}");
        }

        protected override Dictionary<string, IPermissionCheck> InitializePermissionChecks()
            => InitPermissionChecksForApp();

        protected Dictionary<string, IPermissionCheck> InitPermissionChecksForApp()
            => new Dictionary<string, IPermissionCheck>
            {
                {"App", BuildPermissionChecker()}
            };

        public sealed override bool ZoneIsOfCurrentContextOrUserIsSuper(out HttpResponseException exp)
        {
            var wrapLog = Log.Call();
            var zoneSameOrSuperUser = SamePortal || PortalSettings.Current.UserInfo.IsSuperUser;
            exp = zoneSameOrSuperUser ? null: Http.PermissionDenied(
                $"accessing app {App.AppId} in zone {App.ZoneId} is not allowed for this user");

            wrapLog(zoneSameOrSuperUser ? $"SamePortal:{SamePortal} - ok": "not ok, generate error");

            return zoneSameOrSuperUser;
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
            return new DnnPermissionCheck(Log,
                instance: BlockBuilder?.Container,
                app: App,
                portal: PortalForSecurityCheck,
                targetType: type,
                targetItem: item);
        }

    }
}
