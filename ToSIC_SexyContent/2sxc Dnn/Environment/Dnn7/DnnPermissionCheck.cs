using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Blocks;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Data;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Permissions;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.SexyContent.Environment.Dnn7
{
    /// <summary>
    /// Permissions object which checks if the user is allowed to do soemthing based on specific permission
    /// This checks permissions based on EAV data related to an entity - so pure EAV, no DNN
    /// </summary>
    public class DnnPermissionCheck: PermissionCheckBase
    {
        public string CustomPermissionKey = ""; // "CONTENT";

        private readonly string _salPrefix = "SecurityAccessLevel.".ToLower();


        protected ICmsBlock Instance { get; }
        protected ModuleInfo Module => ((CmsBlock<ModuleInfo>) Instance)?.Original;
        protected PortalSettings Portal { get; }

        protected IApp App { get; }

        protected IAppIdentity AppIdentity;

        public DnnPermissionCheck(
            ILog parentLog,
            IContentType targetType = null,
            IEntity targetItem = null,
            ICmsBlock instance = null,
            IApp app = null,
            IEnumerable<IEntity> permissions1 = null,
            PortalSettings portal = null,
            IAppIdentity appIdentity = null
            )
            : base(parentLog, targetType, targetItem, app?.Metadata.Permissions, permissions1)
        {
            var logWrap = Log.New("DnnPermissionCheck", $"..., {targetItem?.EntityId}, app: {app?.AppId}, ");
            AppIdentity = appIdentity ?? app;
            App = app;
            Instance = instance;
            Portal = portal;
            logWrap(null);
        }



        protected override IUser User => new DnnUser();

        protected override bool EnvironmentAllows(List<Grants> grants)
        {
            var logWrap = Log.Call("EnvironmentAllows", () => $"[{string.Join(",", grants)}]");
            var ok = UserIsSuperuser(); // superusers are always ok
            if (!ok && CurrentZoneMatchesTenantZone())
                ok = UserIsTenantAdmin()
                     || UserIsModuleAdmin()
                     || UserIsModuleEditor();
            if (ok) GrantedBecause = ConditionType.EnvironmentGlobal;
            logWrap($"{ok} because:{GrantedBecause}");
            return ok;
        }


        protected override bool VerifyConditionOfEnvironment(string condition)
        {
            if (!condition.ToLower().StartsWith(_salPrefix)) return false;

            var salWord = condition.Substring(_salPrefix.Length);
            var sal = (SecurityAccessLevel) Enum.Parse(typeof(SecurityAccessLevel), salWord);
            // check anonymous - this is always valid, even if not in a module context
            if (sal == SecurityAccessLevel.Anonymous)
                return true;

            // check within module context
            if (Module != null)
                return ModulePermissionController
                    .HasModuleAccess(sal, CustomPermissionKey, Module);

            Log.Add("trying to check permission " + _salPrefix + ", but don't have module in context");
            return false;
        }

        /// <summary>
        /// Check if user is super user
        /// </summary>
        /// <returns></returns>
        private bool UserIsSuperuser() => Log.Intercept("UserIsSuperuser", 
            () => PortalSettings.Current?.UserInfo?.IsSuperUser ?? false);

        /// <summary>
        /// Check if user is valid admin of current portal / zone
        /// </summary>
        /// <returns></returns>
        public bool UserIsTenantAdmin() => Log.Intercept("UserIsSuperuser", 
            () => Portal?.UserInfo?.IsInRole(Portal?.AdministratorRoleName) ?? false);

        /// <summary>
        /// Verify that we're in the same zone, allowing admin/module checks
        /// </summary>
        /// <returns></returns>
        private bool CurrentZoneMatchesTenantZone()
        {
            var wrapLog = Log.Call("CurrentZoneMatchesTenantZone");
            // but is the current portal also the one we're asking about?
            var env = Eav.Factory.Resolve<IAppEnvironment>();
            if (Portal == null) return false; // this is the case when running out-of http-context
            if (AppIdentity == null) return true; // this is the case when an app hasn't been selected yet, so it's an empty module, must be on current portal
            var pZone = env.ZoneMapper.GetZoneId(Portal.PortalId);
            var result = pZone == AppIdentity.ZoneId; // must match, to accept user as admin
            wrapLog($"{result}");
            return result;
        }

        private bool UserIsModuleEditor()
            => Log.Intercept("UserIsModuleEditor", () => Module != null && ModulePermissionController
                   .HasModuleAccess(SecurityAccessLevel.Edit, "", Module));

        private bool UserIsModuleAdmin()
            => Log.Intercept("UserIsModuleAdmin", 
                () => Module != null && ModulePermissionController.CanAdminModule(Module));
    }
}