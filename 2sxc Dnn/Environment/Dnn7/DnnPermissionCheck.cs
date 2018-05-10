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

        protected IZoneIdentity Zone;

        public DnnPermissionCheck(
            Log parentLog,
            IContentType targetType = null,
            IEntity targetItem = null,
            IInstanceInfo instance = null,
            IApp app = null,
            IEnumerable<IEntity> permissions1 = null,
            PortalSettings portal = null,
            IZoneIdentity zone = null
            )
            : base(parentLog, targetType, targetItem, app?.Metadata.Permissions, permissions1)
        {
            Zone = zone ?? app;
            App = app;
            Instance = instance;
            Portal = portal;
        }



        protected override IUser User => new DnnUser();

        protected override bool EnvironmentAllows(List<Grants> grants)
        {
            var ok = UserIsSuperuser() // superusers are always ok
                     || UserIsTenantAdmin()
                     || UserIsModuleAdmin()
                     || UserIsModuleEditor();
            if (ok) GrantedBecause = ConditionType.EnvironmentGlobal;
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


        private bool UserIsSuperuser() => Portal?.UserInfo?.IsSuperUser ?? false;

        public bool UserIsTenantAdmin()
        {
            // is it the current admin?
            if (!(Portal?.UserInfo?.IsInRole(Portal?.AdministratorRoleName) ?? false)) return false;

            // but is the current portal also the one we're asking about?
            var env = Eav.Factory.Resolve<IEnvironment>();
            var pZone = env.ZoneMapper.GetZoneId(Portal.PortalId);
            return pZone == (Zone?.ZoneId ?? 0); // must match, to accept user as admin
        }

        private bool UserIsModuleEditor()
            => Module != null && ModulePermissionController
                   .HasModuleAccess(SecurityAccessLevel.Edit, "", Module);

        private bool UserIsModuleAdmin() 
            => Module != null && ModulePermissionController.CanAdminModule(Module);
    }
}