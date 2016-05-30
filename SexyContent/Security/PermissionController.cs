using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using ToSic.Eav;
using ToSic.SexyContent.Security;

namespace ToSic.SexyContent.Security
{
    /// <summary>
    /// Permissions object which checks if the user is allowed to do soemthing based on specific permission
    /// This checks permissions based on EAV data related to an entity - so pure EAV, no DNN
    /// </summary>
    public class PermissionController
    {
        public const int AssignmentObjectId = 4;
        public const string ContentTypeName = "PermissionConfiguration";
        public const string Condition = "Condition";
        public const string Grant = "Grant";
        public string CustomPermissionKey = ""; // "CONTENT";
        readonly string _salPrefix = "SecurityAccessLevel.".ToLower();
        private readonly string _keyOwner = "Owner";

        public int AppId { get; private set; }
        public int ZoneId { get; private set; }
        public Guid TypeGuid { get; private set; }

        // private IEntity _targetItem;

        public IEntity TargetItem { get; set; }
        //{
        //    get
        //    {
        //        return _targetItem;
        //    }
        //    set
        //    {
        //        _targetItem = value;
        //        // TargetGuid = _targetItem.EntityGuid;
        //    } 
        //}

        private IEnumerable<IEntity> _permissionList;

        public IEnumerable<IEntity> PermissionList
        {
            get
            {
                if (_permissionList == null)
                {
                    var ds = DataSource.GetMetaDataSource(ZoneId, AppId);
                    _permissionList = ds.GetAssignedEntities(AssignmentObjectId, TypeGuid, ContentTypeName);
                }
                return _permissionList;
            }
            private set { _permissionList = value; }
        }

        public ModuleInfo Module { get; private set; }

        /// <summary>
        /// Initialize this object so it can then give information regarding the permissions of an entity.
        /// Uses a GUID as identifier because that survives export/import. 
        /// </summary>
        /// <param name="zoneId">EAV Zone</param>
        /// <param name="appId">EAV APP</param>
        /// <param name="typeGuid">Entity GUID to check permissions against</param>
        /// <param name="module">DNN Module - necessary for SecurityAccessLevel checks</param>
        public PermissionController(int zoneId, int appId, Guid typeGuid, ModuleInfo module = null)
        {
            ZoneId = zoneId;
            AppId = appId;
            TypeGuid = typeGuid;
            Module = module;
        }

        public PermissionController(int zoneId, int appId, Guid typeGuid, IEntity targetItem, ModuleInfo module = null)
        {
            ZoneId = zoneId;
            AppId = appId;
            TypeGuid = typeGuid;
            if (targetItem != null)
                TargetItem = targetItem;
            Module = module;
        }


        /// <summary>
        /// Check if a user may do something based on the permissions in the background. 
        /// </summary>
        /// <param name="actionCode">Short-code for r=read, u=update etc.</param>
        /// <returns></returns>
        public bool UserMay(char actionCode)
        {
            return DoesPermissionsListAllow(actionCode);
        }

        public bool UserMay(PermissionGrant action)
        {
            return DoesPermissionsListAllow((Char)action);
        }

        /// <summary>
        /// Check if the permission-list would allow such an action
        /// </summary>
        /// <param name="desiredActionCode">The desired action like c, r, u, d etc.</param>
        /// <returns></returns>
        private bool DoesPermissionsListAllow(char desiredActionCode)
        {
            var allow = false;
            foreach (var perm in PermissionList)
            {
                allow = DoesPermissionAllow(perm, desiredActionCode);
                if (allow) // stop checking if permission ok
                    break;
            }
            return allow;
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
            var grnt = permissionEntity.GetBestValue(Grant).ToString();
            if (grnt.IndexOf(desiredActionCode) == -1) // Grant doesn't contain read, so stop here
                return false;

            // Check if the current user fits the reason for this grant
            try
            {   
                // check general permissions
                var condition = permissionEntity.GetBestValue(Condition).ToString();
                if (condition.ToLower().StartsWith(_salPrefix))
                {
                    var salWord = condition.Substring(_salPrefix.Length);
                    var sal = (SecurityAccessLevel)Enum.Parse(typeof(SecurityAccessLevel), salWord);
                    // check anonymous - this is always valid, even if not in a module context
                    if (sal == SecurityAccessLevel.Anonymous)
                        return true;
                    
                    // check within module context
                    return DotNetNuke.Security.Permissions.ModulePermissionController
                        .HasModuleAccess(sal, CustomPermissionKey, Module);
                }

                // check owner conditions
                if (condition == _keyOwner)
                    // if it's an entity, possibly also check owner-permissions
                    if (TargetItem != null && TargetItem.Owner == Environment.Dnn7.UserIdentity.CurrentUserIdentityToken/* PortalSettings.Current.UserInfo.Username*/)
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