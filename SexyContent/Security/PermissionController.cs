using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;

namespace ToSic.SexyContent.Security
{
    /// <summary>
    /// Permissions object which checks if the user is allowed to do soemthing based on specific permission
    /// This checks permissions based on EAV data related to an entity - so pure EAV, no DNN
    /// </summary>
    public class PermissionController: HasLog
    {
        // todo: move consts to Constants
        public string CustomPermissionKey = ""; // "CONTENT";
        readonly string _salPrefix = "SecurityAccessLevel.".ToLower();

        private IContentType TargetType { get; }

        public IEntity TargetItem { get; set; }


        public IEnumerable<IEntity> PermissionList => _permissionList ?? (_permissionList =
                                                          (TargetType?.Metadata ?? TargetItem.Metadata).Where(md => md.Type.StaticName == Constants.PermissionTypeName));

        private IEnumerable<IEntity> _permissionList;

        public ModuleInfo Module { get; }

        /// <summary>
        /// Initialize this object so it can then give information regarding the permissions of an entity.
        /// Uses a GUID as identifier because that survives export/import. 
        /// </summary>
        /// <param name="targetItem"></param>
        /// <param name="parentLog"></param>
        /// <param name="module">DNN Module - necessary for SecurityAccessLevel checks</param>
        public PermissionController(IEntity targetItem, Log parentLog, ModuleInfo module = null)
            : base("App.PermCk", parentLog, $"init for itm:{targetItem?.EntityGuid} ({targetItem?.EntityId})")
        {
            TargetItem = targetItem;
            TargetType = null; // important that it doesn't exist, otherwise the security check will use that instead of the item
            Module = module;
        }

        public PermissionController(IContentType targetType, IEntity targetItem, Log parentLog, ModuleInfo module = null)
            : base("App.PermCk", parentLog, $"init for type:{targetType?.StaticName}, itm:{targetItem?.EntityGuid} ({targetItem?.EntityId})")
        {
            TargetType = targetType;
            TargetItem = targetItem;
            Module = module;
        }


        /// <summary>
        /// Check if a user may do something based on the permissions in the background. 
        /// </summary>
        /// <param name="actionCode">Short-code for r=read, u=update etc.</param>
        /// <returns></returns>
        public bool UserMay(char actionCode) => DoesPermissionsListAllow(actionCode);

        public bool UserMay(PermissionGrant action) => DoesPermissionsListAllow((Char)action);

        /// <summary>
        /// Check if the permission-list would allow such an action
        /// </summary>
        /// <param name="desiredActionCode">The desired action like c, r, u, d etc.</param>
        /// <returns></returns>
        private bool DoesPermissionsListAllow(char desiredActionCode)
        {
            foreach (var perm in PermissionList)
                if (DoesPermissionAllow(perm, desiredActionCode)) return true;
            return false;
        }

        /// <summary>
        /// Check if a specific permission entity allows for the desired permission
        /// </summary>
        /// <param name="permissionEntity">The entity describing a permission</param>
        /// <param name="desiredActionCode">A key like r (for read), u (for update) etc. which is the level you want to check</param>
        /// <returns></returns>
        private bool DoesPermissionAllow(IEntity permissionEntity, char desiredActionCode)
        {

            // Check if it's a grant-read permission - otherwise stop here
            var grnt = permissionEntity.GetBestValue(Constants.PermissionGrant).ToString();
            if (grnt.IndexOf(desiredActionCode) == -1) // Grant doesn't contain read, so stop here
                return false;

            // Check if the current user fits the reason for this grant
            try
            {   
                // check general permissions
                var condition = permissionEntity.GetBestValue(Constants.PermissionCondition).ToString();
                if (condition.ToLower().StartsWith(_salPrefix))
                {
                    var salWord = condition.Substring(_salPrefix.Length);
                    var sal = (SecurityAccessLevel)Enum.Parse(typeof(SecurityAccessLevel), salWord);
                    // check anonymous - this is always valid, even if not in a module context
                    if (sal == SecurityAccessLevel.Anonymous)
                        return true;
                    
                    // check within module context
                    if (Module == null)
                    {
                        Log.Add("trying to check permission " + _salPrefix + ", but don't have module in context");
                        throw new Exception("trying to check permission " + _salPrefix + ", but don't have module in context");
                    }

                    return DotNetNuke.Security.Permissions.ModulePermissionController
                        .HasModuleAccess(sal, CustomPermissionKey, Module);
                }

                // check owner conditions
                if (condition == Constants.PermissionKeyOwner)
                    // if it's an entity, possibly also check owner-permissions
                    if (TargetItem != null && TargetItem.Owner == Environment.Dnn7.UserIdentity.CurrentUserIdentityToken)
                        return true;
            }
            catch
            {
                // something happened, in this case we assume that this rule cannot described a "is allowed"
                return false;
            }

            // If the code gets here, we apparently don't know what the rule is about - return false
            return false;
        }
    }
}