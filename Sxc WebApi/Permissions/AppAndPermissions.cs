using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Configuration;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.ValueProvider;
using ToSic.Eav.WebApi.Formats;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.WebApi.Errors;
using Factory = ToSic.Eav.Factory;

namespace ToSic.SexyContent.WebApi.Permissions
{
    public class AppAndPermissions: HasLog
    {
        public App App { get; }
        public PermissionCheckBase Permissions { get; private set; }

        public SxcInstance SxcInstance { get; }

        private int ZoneId { get; }
        private readonly PortalSettings _portalForSecurityCheck;


        public AppAndPermissions(SxcInstance sxcInstance, int appId, Log parentLog) : base("Api.Perms", parentLog, "init")
        {
            SxcInstance = sxcInstance;
            var tenant = new DnnTenant(PortalSettings.Current);
            var environment = Factory.Resolve<IEnvironmentFactory>().Environment(Log);
            var contextZoneId = environment.ZoneMapper.GetZoneId(tenant.Id);
            ZoneId = SystemManager.ZoneIdOfApp(appId);
            App = new App(tenant, ZoneId, appId, parentLog: Log);
            var samePortal = contextZoneId == tenant.Id;
            _portalForSecurityCheck = samePortal ? PortalSettings.Current : null;
        }

        public void InitAppData()
        {
            if(SxcInstance?.Data == null)
                throw new Exception("Can't use app-data at the moment, because it requires an instance context");

            var showDrafts = Permissions.UserMay(GrantSets.ReadDraft);

            App.InitData(showDrafts,
                SxcInstance.Environment.PagePublishing.IsEnabled(SxcInstance.EnvInstance.Id),
                SxcInstance.Data.ConfigurationProvider);
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
            // this will run at least once with null, and the last one will be returned in the set
            typeNames.ForEach(tn => EnsureOrThrow(grants, tn));

            //return set;
        }


        internal void EnsureOrThrow(List<Grants> grants = null, string typeName = null)
        {
            BuildPermissionChecker(typeName);
            if(!Permissions.UserMay(grants))
                throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        internal void ThrowIfUserIsRestrictedAndFeatureNotEnabled()
        {
            // 1. check if user is restricted
            var userIsRestricted = !Permissions.UserMay(GrantSets.WritePublished);

            // 2. check if feature is enabled
            var feats = new[] { FeatureIds.PublicForms };
            if (userIsRestricted && !Features.Enabled(feats))
                throw Http.PermissionDenied($"low-permission users may not access this - {Features.MsgMissingSome(feats)}");

        }


        /// <summary>
        /// Creates a permission checker for an app
        /// Optionally you can provide a type-name, which will be 
        /// included in the permission check
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        internal void BuildPermissionChecker(string typeName)
        {
            // now do relevant security checks
            var type = typeName == null
                ? null
                : new AppRuntime(ZoneId, App.AppId, Log)
                    .ContentTypes.Get(typeName);

            // user has edit permissions on this app, and it's the same app as the user is coming from
            Permissions = new DnnPermissionCheck(Log,
                instance: SxcInstance.EnvInstance,
                app: App,
                portal: _portalForSecurityCheck,
                targetType: type);
        }
    }
}
