using System;
using System.Threading;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Permissions;
using DNNSec = DotNetNuke.Security.Membership;

namespace ToSic.SexyContent.API
{
    public class SecurityContext
    {
        public const string ModulePermissionView = "VIEW";
        public const string ModulePermissionEdit = "EDIT";
        public const string ModuleSettingKeyAsmx = "asmx";

        #region Static methods... you want 'em you got 'em

        /// this method works when DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo(); does not
        /// because we only need the portalId to make it work
        /// <returns>null if there is no current user</returns>
        public static UserInfo getCurrentUser(int portalId)
        {
            if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
                return null;
            var memProvider = DNNSec.MembershipProvider.Instance();
            var retVal = memProvider.GetUserByUserName(portalId, Thread.CurrentPrincipal.Identity.Name, false);
            return retVal;
        }

        public static ModuleInfo getModuleInfo(int moduleId, int tabId)
        {
            var modController = new ModuleController();
            var modInfo = modController.GetModule(moduleId, tabId);
            return modInfo;

        }

        /// <param name="moduleId"></param>
        /// <param name="tabId"></param>
        /// <param name="permissionKey">You can use the constants, but for modules there are only
        /// those two</param>
        /// <returns></returns>
        public static bool canUserAccessModule(UserInfo user, int portalId, int tabId, ModuleInfo moduleInfo, string permissionKey)
        {
            var retVal = false;
            string permissionsString = null;
            if (moduleInfo.InheritViewPermissions)
            {
                var tabPermissionController = new TabPermissionController();
                var tabPermissionCollection =
                    tabPermissionController.GetTabPermissionsCollectionByTabID(tabId, portalId);
                permissionsString = tabPermissionController.GetTabPermissions(tabPermissionCollection, permissionKey);
            }
            else
            {
                var modulePermissionController = new ModulePermissionController();
                var permissionCollection =
                    modulePermissionController.GetModulePermissionsCollectionByModuleID(moduleInfo.ModuleID, tabId);
                permissionsString = modulePermissionController.GetModulePermissions(permissionCollection, permissionKey);
            }

            char[] splitter = { ';' };
            var roles = permissionsString.Split(splitter);
            foreach (var role in roles)
            {
                if (role.Length > 0)
                {
                    if (user != null && user.IsInRole(role))
                        retVal = true;
                    else if (user == null && role.ToLower().Equals("all users"))
                        retVal = true;
                }
                
            }
            return retVal;
        }


        /// <summary>
        /// returns the portalId or -1 if something goes wrong
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int getPortalId(HttpContext context)
        {
            var retVal = -1;
            var url = getUriWithoutProtocol(context.Request.Url);
            var controller = new PortalAliasController();
            var aliasCollection = controller.GetPortalAliases();
            foreach (string key in aliasCollection.Keys)
            {
                var info = aliasCollection[key];
                if (url.StartsWith(info.HTTPAlias))
                    retVal = info.PortalID;
            }
            return retVal;
        }

        /// <summary>
        /// pass me the Request.Url
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        static string getUriWithoutProtocol(Uri uri)
        {
            var protocol = uri.Scheme;
            var retVal = uri.AbsoluteUri;
            retVal = retVal.Substring(protocol.Length + 3); // strip off the //:
            return retVal;
        }
        #endregion static methods


        int _portalId;
        UserInfo _userInfo;
        ModuleInfo _moduleInfo;
        int _tabId;
        string _asmxName;

        #region accessors, boring...

        public int tabId
        {
            get { return _tabId; }
            set { _tabId = value; }
        }

        public ModuleInfo moduleInfo
        {
            get { return _moduleInfo; }
            set { _moduleInfo = value; }
        }
        public int portalId
        {
            get { return _portalId; }
            set { _portalId = value; }
        }

        public UserInfo userInfo
        {
            get { return _userInfo; }
            set { _userInfo = value; }
        }

        public string asmxName
        {
            get { return _asmxName; }
            set {
                if (value == null)
                    _asmxName = null;
                else
                    _asmxName = value.ToLower(); 
            }
        }

        #endregion accessors



        /// <summary>
        /// I throw an exception if I can't find the portalId!
        /// </summary>
        public SecurityContext(HttpContext context, int moduleId, int tabId)
        {
            this.tabId = tabId;
            setPortalIdAndPageName(context);
            if (portalId == -1)
                throw new Exception("Cannot find portal for this URL " + context.Request.Url.AbsoluteUri);
            userInfo = getCurrentUser(portalId);
            moduleInfo = getModuleInfo(moduleId, tabId);
        }

        /// <summary>
        /// for this you need to have put a setting in the 
        /// moduleController.UpdateTabModuleSetting(moduleInfo.tabModuleId, MODULE_SETTING_KEY_ASMX, "this.asmx");  
        /// this answers whether the moduleId and tabId received go with the Url they accessed
        /// This is an important security check
        /// </summary>
        /// <returns></returns>
        public bool canModuleAccessCurrentUrl()
        {
            if (moduleInfo == null)
                return false;
            var retVal = false;
            var moduleController = new ModuleController();
            var settings = moduleController.GetTabModuleSettings(moduleInfo.TabModuleID);
            if (settings.ContainsKey(ModuleSettingKeyAsmx))
            {
                var setting = settings[ModuleSettingKeyAsmx] as string;
                if (setting.Length > 0) // else false, of course
                    retVal = asmxName.ToLower().Contains(setting.ToLower());
            }
            return retVal;
        }

        public bool canUserAccessModule(string permissionKey)
        {
            if (moduleInfo == null)
                return false; // userInfo can be null if we're not logged in
            return canUserAccessModule(userInfo, portalId, tabId, moduleInfo, permissionKey);
        }

        /// <summary>
        /// returns the portalId or -1 if something goes wrong
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public void setPortalIdAndPageName(HttpContext context)
        {
            portalId = -1;
            asmxName = null;

            var url = getUriWithoutProtocol(context.Request.Url);
            var controller = new PortalAliasController();
            var aliasCollection = controller.GetPortalAliases();
            foreach (string key in aliasCollection.Keys)
            {
                var info = aliasCollection[key];
                if (url.StartsWith(info.HTTPAlias))
                {
                    portalId = info.PortalID;
                    asmxName = url.Substring(info.HTTPAlias.Length + 1);
                }
            }
        }
    }
}