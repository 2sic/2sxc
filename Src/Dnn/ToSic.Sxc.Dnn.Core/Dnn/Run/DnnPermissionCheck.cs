using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Security;
using ToSic.Sxc.Context;


namespace ToSic.Sxc.Dnn.Run
{
    /// <summary>
    /// Permissions object which checks if the user is allowed to do something based on specific permission
    /// This checks permissions based on EAV data related to an entity - so pure EAV, no DNN
    /// </summary>
    public class DnnPermissionCheck: AppPermissionCheck
    {
        #region Constructor / DI

        public DnnPermissionCheck(IAppStates appStates, Dependencies dependencies) : base(appStates, dependencies, DnnConstants.LogName)
        {
        }

        #endregion

        public string CustomPermissionKey = ""; // "CONTENT";

        private readonly string _salPrefix = "SecurityAccessLevel.".ToLowerInvariant();

        /// <summary>
        /// The DNN module on the container
        /// </summary>
        /// <remarks>
        /// In some cases the container is a ContainerNull object without ModuleInfo, so we must really do null-checks
        /// </remarks>
        protected ModuleInfo Module =>
            _module ?? (_module = ((Context as IContextOfBlock)?.Module as Module<ModuleInfo>)?.UnwrappedContents);
        private ModuleInfo _module;


        protected override bool EnvironmentAllows(List<Grants> grants)
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


        protected override bool VerifyConditionOfEnvironment(string condition)
        {
            if (!condition.ToLowerInvariant().StartsWith(_salPrefix)) return false;

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
        private bool CurrentZoneMatchesSiteZone()
        {
            var wrapLog = Log.Call<bool>();
            // but is the current portal also the one we're asking about?
            if (Context.Site == null || Context.Site.Id == Eav.Constants.NullId) return wrapLog("no", false); // this is the case when running out-of http-context
            if (AppIdentity == null) return wrapLog("yes", true); // this is the case when an app hasn't been selected yet, so it's an empty module, must be on current portal
            var pZone = Context.Site.ZoneId;
            var result = pZone == AppIdentity.ZoneId; // must match, to accept user as admin
            return wrapLog($"{result}", result);
        }

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

        private bool UserIsModuleAdmin()
            => Log.Intercept(nameof(UserIsModuleAdmin), 
                () => Module != null && ModulePermissionController.CanAdminModule(Module));
    }
}