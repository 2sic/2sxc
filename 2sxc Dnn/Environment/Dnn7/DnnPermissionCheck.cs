using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;

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


        protected IInstanceInfo Instance { get; }
        protected ModuleInfo Module => ((EnvironmentInstance<ModuleInfo>) Instance)?.Original;
        protected PortalSettings Portal { get; }

        protected IApp App { get; }

        protected IAppIdentity AppIdentity;

        public DnnPermissionCheck(
            Log parentLog,
            IContentType targetType = null,
            IEntity targetItem = null,
            IInstanceInfo instance = null,
            IApp app = null,
            IEnumerable<IEntity> permissions1 = null,
            PortalSettings portal = null,
            IAppIdentity appIdentity = null
            )
            : base(parentLog, targetType, targetItem, app?.Metadata.Permissions, permissions1)
        {
            Log.Add("constructor");
            AppIdentity = appIdentity ?? app;
            App = app;
            Instance = instance;
            Portal = portal;
        }



        protected override IUser User => new DnnUser();

        protected override bool EnvironmentAllows(List<Grants> grants)
        {
            Log.Add("Env allows...");
            var ok = UserIsSuperuser(); // superusers are always ok
            if (!ok && CurrentZoneMatchesTenantZone())
                ok = UserIsTenantAdmin()
                     || UserIsModuleAdmin()
                     || UserIsModuleEditor();
            if (ok) GrantedBecause = ConditionType.EnvironmentGlobal;
            Log.Add($"ok: {ok} because:{GrantedBecause}");
            return ok;
        }


        protected override bool DoesConditionApplyInEnvironment(string condition)
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
        private bool UserIsSuperuser() => PortalSettings.Current?.UserInfo?.IsSuperUser ?? false;

        /// <summary>
        /// Check if user is valid admin of current portal / zone
        /// </summary>
        /// <returns></returns>
        public bool UserIsTenantAdmin() => Portal?.UserInfo?.IsInRole(Portal?.AdministratorRoleName) ?? false;

        /// <summary>
        /// Verify that we're in the same zone, allowing admin/module checks
        /// </summary>
        /// <returns></returns>
        private bool CurrentZoneMatchesTenantZone()
        {
            // but is the current portal also the one we're asking about?
            var env = Eav.Factory.Resolve<IEnvironment>();
            if (Portal == null) return false; // this is the case when running out-of http-context
            if (AppIdentity == null) return true; // this is the case when an app hasn't been selected yet, so it's an empty module, must be on current portal
            var pZone = env.ZoneMapper.GetZoneId(Portal.PortalId);
            return pZone == AppIdentity.ZoneId; // must match, to accept user as admin
        }

        private bool UserIsModuleEditor()
            => Module != null && ModulePermissionController
                   .HasModuleAccess(SecurityAccessLevel.Edit, "", Module);

        private bool UserIsModuleAdmin() 
            => Module != null && ModulePermissionController.CanAdminModule(Module);
    }
}