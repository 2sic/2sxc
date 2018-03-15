using System;
using DotNetNuke.Entities.Modules;
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
    public class DnnPermissionController: PermissionController
    {
        public string CustomPermissionKey = ""; // "CONTENT";

        private readonly string _salPrefix = "SecurityAccessLevel.".ToLower();

        public ModuleInfo Module { get; }

        /// <inheritdoc />
        /// <summary>
        /// Initialize this object so it can then give information regarding the permissions of an entity.
        /// Uses a GUID as identifier because that survives export/import. 
        /// </summary>
        /// <param name="targetItem"></param>
        /// <param name="parentLog"></param>
        /// <param name="module">DNN Module - necessary for SecurityAccessLevel checks</param>
        public DnnPermissionController(IEntity targetItem, Log parentLog, IInstanceInfo module = null)
            : base(targetItem, parentLog)
        {
            Module = ((InstanceInfo<ModuleInfo>)module)?.Info;
        }

        public DnnPermissionController(
            IContentType targetType, 
            IEntity targetItem, 
            Log parentLog, 
            IInstanceInfo module = null,
            IMetadataOfItem addMeta1 = null, // optional additional metadata, like of an app
            IMetadataOfItem addMeta2 = null  // optional additional metadata, like of a zone
            )
            : base(targetType, targetItem, parentLog, addMeta1, addMeta2)
        {
            Module = ((InstanceInfo<ModuleInfo>)module)?.Info;
        }

        public DnnPermissionController(
            IInstanceInfo module = null,
            IMetadataOfItem meta1 = null, // optional additional metadata, like of an app
            IMetadataOfItem meta2 = null, // optional additional metadata, like of a zone
            Log parentLog = null
        ) : base(meta1, meta2, parentLog)
        {
            Module = ((InstanceInfo<ModuleInfo>)module)?.Info;
        }


        //protected override string CurrentUser => new DnnUser().IdentityToken;
        protected override IUser User => new DnnUser();

        protected override bool EnvironmentGivesPermission(string condition)
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
            throw new Exception("trying to check permission " + _salPrefix +
                                ", but don't have module in context");
        }


    }
}