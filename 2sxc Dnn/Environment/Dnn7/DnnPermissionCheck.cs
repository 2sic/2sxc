using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
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
        protected IApp App { get; }

        public DnnPermissionCheck(
            Log parentLog,
            IContentType targetType = null,
            IEntity targetItem = null,
            IInstanceInfo instance = null,
            IApp app = null,
            IMetadataOfItem meta1 = null, // optional additional metadata, like of an app
            IMetadataOfItem meta2 = null  // optional additional metadata, like of a zone
            )
            : base(parentLog, targetType, targetItem, meta1, meta2)
        {
            App = app;
            Instance = instance;
        }



        protected override IUser User => new DnnUser();

        protected override bool EnvironmentAllows(List<PermissionGrant> grants) 
            => UserIsSuperuser() 
            || UserMayEditModule();

        private static bool UserIsSuperuser()
        {
            return PortalSettings.Current?.UserInfo.IsSuperUser ?? false;
        }

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
                return DotNetNuke.Security.Permissions.ModulePermissionController
                    .HasModuleAccess(sal, CustomPermissionKey, Module);

            Log.Add("trying to check permission " + _salPrefix + ", but don't have module in context");
            return false;
        }


        private bool UserMayEditModule()
        {
            if (Module == null)
                return false;

            return DotNetNuke.Security.Permissions.ModulePermissionController
                .HasModuleAccess(SecurityAccessLevel.Edit, "" /*"EDIT"*/, Module);
        }
    }
}