using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging.Simple;

namespace ToSic.SexyContent.Environment.Dnn7
{
    /// <summary>
    /// Permissions object which checks if the user is allowed to do soemthing based on specific permission
    /// This checks permissions based on EAV data related to an entity - so pure EAV, no DNN
    /// </summary>
    public class DnnPermissionController: Eav.Security.Permissions.PermissionController
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
        public DnnPermissionController(IEntity targetItem, Log parentLog, ModuleInfo module = null)
            : base(targetItem, parentLog)
        {
            Module = module;
        }

        public DnnPermissionController(IContentType targetType, IEntity targetItem, Log parentLog, ModuleInfo module = null)
            : base(targetType, targetItem, parentLog)
        {
            Module = module;
        }

        
        // 2017-12-19 moved this to EAV - keep in case something fails till ca. Jan 2018, then delete
        ///// <summary>
        ///// Check if a specific permission entity allows for the desired permission
        ///// </summary>
        ///// <param name="permissionEntity">The entity describing a permission</param>
        ///// <param name="desiredActionCode">A key like r (for read), u (for update) etc. which is the level you want to check</param>
        ///// <returns></returns>
        //private bool DoesPermissionAllow(IEntity permissionEntity, char desiredActionCode)
        //{

        //    // Check if it's a grant-read permission - otherwise stop here
        //    var grnt = permissionEntity.GetBestValue(Constants.PermissionGrant).ToString();
        //    if (grnt.IndexOf(desiredActionCode) == -1) // Grant doesn't contain read, so stop here
        //        return false;

        //    // Check if the current user fits the reason for this grant
        //    try
        //    {   
        //        // check general permissions
        //        var condition = permissionEntity.GetBestValue(Constants.PermissionCondition).ToString();
        //        if (condition.ToLower().StartsWith(_salPrefix))
        //        {
        //            var salWord = condition.Substring(_salPrefix.Length);
        //            var sal = (SecurityAccessLevel)Enum.Parse(typeof(SecurityAccessLevel), salWord);
        //            // check anonymous - this is always valid, even if not in a module context
        //            if (sal == SecurityAccessLevel.Anonymous)
        //                return true;
                    
        //            // check within module context
        //            if (Module == null)
        //            {
        //                Log.Add("trying to check permission " + _salPrefix + ", but don't have module in context");
        //                throw new Exception("trying to check permission " + _salPrefix + ", but don't have module in context");
        //            }

        //            return DotNetNuke.Security.Permissions.ModulePermissionController
        //                .HasModuleAccess(sal, CustomPermissionKey, Module);
        //        }

        //        // check owner conditions
        //        if (condition == Constants.PermissionKeyOwner)
        //            // if it's an entity, possibly also check owner-permissions
        //            if (TargetItem != null && TargetItem.Owner == Environment.Dnn7.UserIdentity.CurrentUserIdentityToken)
        //                return true;
        //    }
        //    catch
        //    {
        //        // something happened, in this case we assume that this rule cannot described a "is allowed"
        //        return false;
        //    }

        //    // If the code gets here, we apparently don't know what the rule is about - return false
        //    return false;
        //}

        protected override string CurrentUser 
            => UserIdentity.CurrentUserIdentityToken;

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