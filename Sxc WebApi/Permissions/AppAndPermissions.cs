using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Formats;
using ToSic.SexyContent.Environment.Dnn7;
using Factory = ToSic.Eav.Factory;

namespace ToSic.SexyContent.WebApi.Permissions
{
    public class AppAndPermissions: HasLog
    {
        public App App { get; }

        protected SxcInstance SxcInstance;

        private DnnTenant Tenant => new DnnTenant(PortalSettings.Current);
        private IEnvironment Environment => Factory.Resolve<IEnvironmentFactory>().Environment(Log);

        private int ContextZoneId => Environment.ZoneMapper.GetZoneId(Tenant.Id);

        private int ZoneId { get; }
        private readonly PortalSettings _portalForSecurityCheck;

        public PermissionCheckBase Checker;

        public AppAndPermissions(SxcInstance sxcInstance, int appId, Log parentLog) : base("Api.Perms", parentLog, "init")
        {
            SxcInstance = sxcInstance;
            ZoneId = SystemManager.ZoneIdOfApp(appId);
            App = new App(Tenant, ZoneId, appId, parentLog: Log);
            var samePortal = ContextZoneId == Tenant.Id;
            _portalForSecurityCheck = samePortal ? PortalSettings.Current : null;
        }



        internal void EnsureOrThrow(List<Grants> grants, List<ItemIdentifier> items)
        {
            var appMan = new AppRuntime(App.AppId, Log);

            // build list of type names
            var typeNames = items.Select(item =>
            {
                var typeName = item.ContentTypeName;
                return !string.IsNullOrEmpty(typeName) || item.EntityId == 0
                    ? typeName
                    : appMan.Entities.Get(item.EntityId).Type.StaticName;
            }).ToList();

            // make sure we have at least one entry, so the checks will work
            if (typeNames.Count == 0)
                typeNames.Add(null);

            // go through all the groups, assign relevant info so that we can then do get-many
            //PermissionCheckBase set = null;

            // this will run at least once with null, and the last one will be returned in the set
            typeNames.ForEach(tn => EnsureOrThrow(grants, tn));

            //return set;
        }


        internal void EnsureOrThrow(List<Grants> grants = null, string typeName = null)
        {
            var set = TypeChecker(typeName);
            if(!set.UserMay(grants))
                throw new HttpResponseException(HttpStatusCode.Forbidden);
        }


        /// <summary>
        /// Creates a permission checker for an app
        /// Optionally you can provide a type-name, which will be 
        /// included in the permission check
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        internal PermissionCheckBase TypeChecker(string typeName)
        {
            // now do relevant security checks
            var type = typeName == null
                ? null
                : new AppRuntime(ZoneId, App.AppId, Log)
                    .ContentTypes.Get(typeName);

            // user has edit permissions on this app, and it's the same app as the user is coming from
            var checker = new DnnPermissionCheck(Log,
                instance: SxcInstance.EnvInstance,
                app: App,
                portal: _portalForSecurityCheck,
                targetType: type);

            Checker = checker;
            return checker;
        }
    }
}
