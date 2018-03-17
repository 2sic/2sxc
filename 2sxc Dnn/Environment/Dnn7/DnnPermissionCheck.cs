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

        public DnnPermissionCheck(
            Log parentLog,
            IContentType targetType = null,
            IEntity targetItem = null,
            IInstanceInfo instance = null,
            IApp app = null,
            IMetadataOfItem meta1 = null, // optional additional metadata, like of an app
            //IMetadataOfItem meta2 = null, // optional additional metadata, like of a zone
            PortalSettings portal = null
            )
            : base(parentLog, targetType, targetItem, meta1, app?.Metadata)
        {
            App = app;
            Instance = instance;
            Portal = portal;
        }



        protected override IUser User => new DnnUser();

        protected override bool EnvironmentAllows(List<PermissionGrant> grants) 
            => UserIsSuperuser() // superusers are always ok
            || UserIsTenantAdmin()
            || UserIsModuleAdmin()
            || UserIsModuleEditor();


        protected override bool EnvironmentApprovesCondition(string condition)
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

        public bool UserIsTenantAdmin() => Portal?.UserInfo?.IsInRole(Portal?.AdministratorRoleName) ?? false;

        private bool UserIsModuleEditor()
            => Module != null && ModulePermissionController
                   .HasModuleAccess(SecurityAccessLevel.Edit, "" /*"EDIT"*/, Module);

        private bool UserIsModuleAdmin() 
            => Module != null && ModulePermissionController.CanAdminModule(Module);
    }
}