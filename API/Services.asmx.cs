using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;

namespace ToSic.SexyContent.API
{
	[WebService(Namespace = "http://schemas.2sic.com/2013/ToSexyContent/Services/01.00")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	[ScriptService]
	public class Services : WebService
	{
		[WebMethod]
		[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
		public FileInfo GetFileByPath(int portalId, string relativePath)
		{
			var file = FileManager.Instance.GetFile(portalId, relativePath);
			if (CanUserViewFile(file))
				return (FileInfo)file;

			return null;
		}

		[WebMethod]
		[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
		public FileInfo GetFileById(int fileId)
		{
			var file = FileManager.Instance.GetFile(fileId);
			if (CanUserViewFile(file))
				return (FileInfo)file;

			return null;
		}

		private bool CanUserViewFile(IFileInfo file)
		{
			if (file != null)
			{
				var folder = (FolderInfo)FolderManager.Instance.GetFolder(file.FolderId);
				if (FolderPermissionController.CanViewFolder(folder))
					return true;
			}

			return false;
		}

		/// <remarks>
		/// Source: DotNetNuke.Modules.Admin.Portals.SiteSettings.BindPages
		/// Note: GetPortalTabs() with checkViewPermisison = true doesn't work in Webservice context! Use manual permission check for each tab instead
		/// </remarks>
		[WebMethod]
		[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
		public List<TabInfo> GetPortalTabs(int portalId, string activeLanguage)
		{
			var listTabs = TabController.GetPortalTabs(TabController.GetTabsBySortOrder(portalId, activeLanguage, true), Null.NullInteger, true, "<" + Localization.GetString("None_Specified") + ">", true, false, false, false, false);

			var currentPortalId = SecurityContext.getPortalId(Context);
			var user = SecurityContext.getCurrentUser(currentPortalId);

			return (from tabInfo in listTabs.Where(t => !t.DisableLink)
					where CanUserViewTab(user, currentPortalId, tabInfo.TabID) || tabInfo.TabID == -1
					select new TabInfo { TabID = tabInfo.TabID, IndentedTabName = tabInfo.IndentedTabName }).ToList();
		}

		/// <remarks>Source/Template from SecurityContext.canUserAccessModule()</remarks>
		private static bool CanUserViewTab(UserInfo user, int portalId, int tabId, string permissionKey = "VIEW")
		{
			//var retVal = false;
			var tabPermissionController = new TabPermissionController();
			var tabPermissionCollection = tabPermissionController.GetTabPermissionsCollectionByTabID(tabId, portalId);
			var permissionsString = tabPermissionController.GetTabPermissions(tabPermissionCollection, permissionKey);

			char[] splitter = { ';' };
			var roles = permissionsString.Split(splitter);

			foreach (var role in roles.Where(role => role.Length > 0))
			{
				if (user != null && user.IsInRole(role))
					return true;
				if (role.ToLower().Equals("all users"))
					return true;
			}

			return false;
		}

		public struct TabInfo
		{
			public int TabID { get; set; }
			public string IndentedTabName { get; set; }
		}
	}
}
