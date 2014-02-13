/*
The MIT License

Copyright (c) 2008 Daniel Rosenstark (http://www.confusionists.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 */
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;
using DotNetNuke.Entities.Users;
using DNNSec = DotNetNuke.Security.Membership;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Entities.Portals;
using System.Collections;

//<2sic modified>
//namespace Confusionists.IWebLite
namespace ToSic.SexyContent.API
//</2sic modified>
{
    /// <summary>
    /// I know, you'll say, "doesn't this go know the Microsoft code conventions?
    /// They're annoying, capital letters are for classes only.
    /// I use them for namespaces too so as not to annoy too many other people.
    /// </summary>
    public class SecurityContext
    {
        public const string MODULE_PERMISSION_VIEW = "VIEW";
        public const string MODULE_PERMISSION_EDIT = "EDIT";
        public const string MODULE_SETTING_KEY_ASMX = "asmx";

        #region Static methods... you want 'em you got 'em

        /// this method works when DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo(); does not
        /// because we only need the portalId to make it work
        /// <returns>null if there is no current user</returns>
        public static UserInfo getCurrentUser(int portalId)
        {
            if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
                return null;
            DNNSec.MembershipProvider memProvider = DNNSec.MembershipProvider.Instance();
            UserInfo retVal = memProvider.GetUserByUserName(portalId, Thread.CurrentPrincipal.Identity.Name, false);
            return retVal;
        }

        public static ModuleInfo getModuleInfo(int moduleId, int tabId)
        {
            ModuleController modController = new ModuleController();
            ModuleInfo modInfo = modController.GetModule(moduleId, tabId);
            return modInfo;

        }

        /// <param name="moduleId"></param>
        /// <param name="tabId"></param>
        /// <param name="permissionKey">You can use the constants, but for modules there are only
        /// those two</param>
        /// <returns></returns>
        public static bool canUserAccessModule(UserInfo user, int portalId, int tabId, ModuleInfo moduleInfo, string permissionKey)
        {
            bool retVal = false;
            string permissionsString = null;
            if (moduleInfo.InheritViewPermissions)
            {
                TabPermissionController tabPermissionController = new TabPermissionController();
                TabPermissionCollection tabPermissionCollection =
                    tabPermissionController.GetTabPermissionsCollectionByTabID(tabId, portalId);
                permissionsString = tabPermissionController.GetTabPermissions(tabPermissionCollection, permissionKey);
            }
            else
            {
                ModulePermissionController modulePermissionController = new ModulePermissionController();
                ModulePermissionCollection permissionCollection =
                    modulePermissionController.GetModulePermissionsCollectionByModuleID(moduleInfo.ModuleID, tabId);
                permissionsString = modulePermissionController.GetModulePermissions(permissionCollection, permissionKey);
            }

            char[] splitter = { ';' };
            string[] roles = permissionsString.Split(splitter);
            foreach (string role in roles)
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
            int retVal = -1;
            string url = getUriWithoutProtocol(context.Request.Url);
            PortalAliasController controller = new PortalAliasController();
            PortalAliasCollection aliasCollection = controller.GetPortalAliases();
            foreach (string key in aliasCollection.Keys)
            {
                PortalAliasInfo info = aliasCollection[key] as PortalAliasInfo;
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
            string protocol = uri.Scheme;
            string retVal = uri.AbsoluteUri;
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
        /// <param name="context"></param>
        public SecurityContext(HttpContext context, int moduleId, int tabId)
        {
            this.tabId = tabId;
            setPortalIdAndPageName(context);
            if (this.portalId == -1)
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
            bool retVal = false;
            ModuleController moduleController = new ModuleController();
            Hashtable settings = moduleController.GetTabModuleSettings(moduleInfo.TabModuleID);
            if (settings.ContainsKey(MODULE_SETTING_KEY_ASMX))
            {
                string setting = settings[MODULE_SETTING_KEY_ASMX] as string;
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

            string url = getUriWithoutProtocol(context.Request.Url);
            PortalAliasController controller = new PortalAliasController();
            PortalAliasCollection aliasCollection = controller.GetPortalAliases();
            foreach (string key in aliasCollection.Keys)
            {
                PortalAliasInfo info = aliasCollection[key] as PortalAliasInfo;
                if (url.StartsWith(info.HTTPAlias))
                {
                    portalId = info.PortalID;
                    asmxName = url.Substring(info.HTTPAlias.Length + 1);
                }
            }
        }
    }
}