using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Run;
using ToSic.Eav.Security;

namespace ToSic.Sxc.Dnn.Run
{
    /// <summary>
    /// Permissions object which checks if the user is allowed to do something based on specific permission
    /// This checks permissions based on EAV data related to an entity - so pure EAV, no DNN
    /// </summary>
    public class DnnPermissionCheck: AppPermissionCheck
    {
        public string CustomPermissionKey = ""; // "CONTENT";

        private readonly string _salPrefix = "SecurityAccessLevel.".ToLower();

        /// <summary>
        /// The DNN module on the container
        /// </summary>
        /// <remarks>
        /// In some cases the container is a ContainerNull object without ModuleInfo, so we must really do null-checks
        /// </remarks>
        protected ModuleInfo Module =>
            _module ?? (_module = (Context.Container as Container<ModuleInfo>)?.UnwrappedContents);
        private ModuleInfo _module;


        public DnnPermissionCheck() : base("Dnn.PrmChk") { }


        protected override bool EnvironmentAllows(List<Grants> grants)
        {
            var logWrap = Log.Call(() => $"[{string.Join(",", grants)}]");
            var ok = UserIsSuperuser(); // superusers are always ok
            if (!ok && CurrentZoneMatchesTenantZone())
                ok = UserIsTenantAdmin()
                     || UserIsModuleAdmin()
                     || UserIsModuleEditor();
            if (ok) GrantedBecause = Conditions.EnvironmentGlobal;
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
        /// Verify that we're in the same zone, allowing admin/module checks
        /// </summary>
        /// <returns></returns>
        private bool CurrentZoneMatchesTenantZone()
        {
            var wrapLog = Log.Call();
            // but is the current portal also the one we're asking about?
            var env = Eav.Factory.Resolve<IAppEnvironment>();
            if (Context.Tenant == null || Context.Tenant.Id == AppConstants.AppIdNotFound) return false; // this is the case when running out-of http-context
            if (AppIdentity == null) return true; // this is the case when an app hasn't been selected yet, so it's an empty module, must be on current portal
            var pZone = env.ZoneMapper.GetZoneId(Context.Tenant);
            var result = pZone == AppIdentity.ZoneId; // must match, to accept user as admin
            wrapLog($"{result}");
            return result;
        }

        private bool UserIsModuleEditor()
            => Log.Intercept(nameof(UserIsModuleEditor), 
                () => Module != null && ModulePermissionController
                   .HasModuleAccess(SecurityAccessLevel.Edit, "", Module));

        private bool UserIsModuleAdmin()
            => Log.Intercept(nameof(UserIsModuleAdmin), 
                () => Module != null && ModulePermissionController.CanAdminModule(Module));
    }
}