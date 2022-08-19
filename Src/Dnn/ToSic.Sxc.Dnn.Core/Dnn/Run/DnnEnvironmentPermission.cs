using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Logging;
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
            _module ?? (_module = ((Context as IContextOfBlock)?.Module as Module<ModuleInfo>)?.GetContents());
        private ModuleInfo _module;

        public override bool EnvironmentAllows(List<Grants> grants)
        {
            var logWrap = Log.Fn<bool>(() => $"[{string.Join(",", grants)}]");
            if (UserIsAnonymous()) logWrap.Return(false, "user anonymous");
            var ok = UserIsSuperuser(); // superusers are always ok
            if (!ok && CurrentZoneMatchesSiteZone())
                ok = UserIsSiteAdmin()
                     || UserIsModuleAdmin()
                     || UserIsModuleEditor();
            if (ok) GrantedBecause = Conditions.EnvironmentGlobal;
            return logWrap.Return(ok, $"{ok} because:{GrantedBecause}");
        }

        public override bool VerifyConditionOfEnvironment(string condition)
        {
            var wrapLog = Log.Fn<bool>(condition);
            if (!condition.ToLowerInvariant().StartsWith(_salPrefix)) 
                return wrapLog.ReturnFalse("unknown condition: false");

            var salWord = condition.Substring(_salPrefix.Length);
            var sal = (SecurityAccessLevel)Enum.Parse(typeof(SecurityAccessLevel), salWord);
            // check anonymous - this is always valid, even if not in a module context
            if (sal == SecurityAccessLevel.Anonymous)
                return wrapLog.ReturnTrue("anonymous, always true");

            // check within module context
            if (Module != null)
            {
                // TODO: STV WHERE DOES THE MODULE COME FROM?
                // IT APPEARS THAT IT'S MISSING IN NORMAL REST CALLS
                var result = ModulePermissionController.HasModuleAccess(sal, CustomPermissionKey, Module);
                return wrapLog.Return(result, $"module: {result}");
            }

            Log.A("trying to check permission " + _salPrefix + ", but don't have module in context");
            return wrapLog.ReturnFalse("can't verify: false");
        }

        private bool UserIsModuleAdmin() => Log.Return(() => Module != null && ModulePermissionController.CanAdminModule(Module));

        private bool UserIsModuleEditor()
            => Log.Return(() =>
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
