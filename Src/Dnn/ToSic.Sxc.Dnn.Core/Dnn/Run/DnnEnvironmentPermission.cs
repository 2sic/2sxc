using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Security;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnEnvironmentPermission : EnvironmentPermission
    {
        public DnnEnvironmentPermission() : base(DnnConstants.LogName) { }

        private readonly string _salPrefix = "SecurityAccessLevel.".ToLowerInvariant();
        public string CustomPermissionKey = ""; // "CONTENT";

        /// <summary>
        /// The DNN module on the container
        /// </summary>
        /// <remarks>
        /// In some cases the container is a ContainerNull object without ModuleInfo, so we must really do null-checks
        /// </remarks>
        protected ModuleInfo Module =>
            _module ?? (_module = ((Context as IContextOfBlock)?.Module as Module<ModuleInfo>)?.UnwrappedContents);
        private ModuleInfo _module;

        public override bool EnvironmentAllows(List<Grants> grants)
        {
            var logWrap = Log.Call(() => $"[{string.Join(",", grants)}]");
            var ok = UserIsSuperuser(); // superusers are always ok
            if (!ok && CurrentZoneMatchesSiteZone())
                ok = UserIsSiteAdmin()
                     || UserIsModuleAdmin()
                     || UserIsModuleEditor();
            if (ok) GrantedBecause = Conditions.EnvironmentGlobal;
            logWrap($"{ok} because:{GrantedBecause}");
            return ok;
        }

        public override bool VerifyConditionOfEnvironment(string condition)
        {
            if (!condition.ToLowerInvariant().StartsWith(_salPrefix)) return false;

            var salWord = condition.Substring(_salPrefix.Length);
            var sal = (SecurityAccessLevel)Enum.Parse(typeof(SecurityAccessLevel), salWord);
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

        private bool UserIsModuleAdmin()
            => Log.Intercept(nameof(UserIsModuleAdmin),
                () => Module != null && ModulePermissionController.CanAdminModule(Module));

        private bool UserIsModuleEditor()
            => Log.Intercept(nameof(UserIsModuleEditor),
                () =>
                {
                    if (Module == null) return false;

                    // This seems to throw errors during search :(
                    try
                    {
                        // skip during search (usual HttpContext is missing for search)
                        if (System.Web.HttpContext.Current == null) return false;

                        return ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "", Module);
                    }
                    catch
                    {
                        return false;
                    }
                });
    }
}
