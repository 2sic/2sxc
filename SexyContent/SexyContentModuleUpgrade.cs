using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Upgrade;
using DotNetNuke.Web.Client.ClientResourceManagement;
using ToSic.Eav;
using ToSic.Eav.DataSources.Caches;
using ToSic.Eav.Import;
using ToSic.SexyContent.ImportExport;
using System.Text;
using System.Web.Hosting;

namespace ToSic.SexyContent
{
	public class SexyContentModuleUpgrade
	{
		public static readonly bool UpgradeComplete;

		private static string _logDirectory = "~/DesktopModules/ToSIC_SexyContent/Upgrade/Log/";

		static SexyContentModuleUpgrade()
		{
			UpgradeComplete = IsUpgradeComplete(SexyContent.ModuleVersion);
		}

		public static string UpgradeModule(string version)
		{
			if (IsUpgradeComplete(version))
				throw new Exception("2sxc upgrade for version " + version + " started, but it looks like the upgrade for this version is already complete. Aborting upgrade.");

			if (IsUpgradeRunning)
				throw new Exception("2sxc upgrade for version " + version + " started, but the upgrade is already running. Aborting upgrade.");

			IsUpgradeRunning = true;
			LogUpgradeStep("----- Upgrade to " + version + " started -----");

			try
			{

				switch (version)
				{
					case "01.00.00": // Make sure that log folder is not existent on new installations
						if (Directory.Exists(HostingEnvironment.MapPath(_logDirectory)))
							Directory.Delete(HostingEnvironment.MapPath(_logDirectory), true);
						break;
					case "05.05.00":
						Version050500();
						break;
					case "06.06.00":
					case "06.06.04":
						EnsurePipelineDesignerAttributeSets();
						break;
					case "07.00.00":
						Version070000();
						break;
					case "07.00.03":
						Version070003();
						break;
					case "07.02.00":
						Version070200();
						break;
					case "07.02.02":
						// Make sure upgrades between 07.00.00 and 07.02.02 do not run again when FinishAbortedUpgrade is triggered
						LogSuccessfulUpgrade("07.00.00", false);
						LogSuccessfulUpgrade("07.00.03", false);
						LogSuccessfulUpgrade("07.02.00", false);
						break;
				}

				// Increase ClientDependency version upon each upgrade (System and all Portals)
				// prevents browsers caching old JS and CSS files for editing, which could cause several errors
				ClientResourceManager.UpdateVersion();

				LogSuccessfulUpgrade(version);
				LogUpgradeStep("----- Upgrade to " + version + " completed -----");

			}
			catch (Exception e)
			{
				LogUpgradeStep(version, "Upgrade failed - " + e.Message);
				throw;
			}
			finally
			{
				IsUpgradeRunning = false;
			}

			return version;
		}

		internal static void FinishAbortedUpgrade() {
			// Maybe this list can somehow be extracted from the module manifest?
			var upgradeVersionList = new[] { "07.00.00", "07.00.03", "07.02.00", "07.02.02" };

			// Run upgrade again for all versions that do not have a corresponding logfile
			foreach(var upgradeVersion in upgradeVersionList) {
				if(!IsUpgradeComplete(upgradeVersion))
					UpgradeModule(upgradeVersion);
			}

			// Restart application
			HttpRuntime.UnloadAppDomain();
		}

		internal static void LogSuccessfulUpgrade(string version, bool appendToFile = true)
		{
			if(!Directory.Exists(HostingEnvironment.MapPath(_logDirectory)))
				Directory.CreateDirectory(HostingEnvironment.MapPath(_logDirectory));

			var logFilePath = HostingEnvironment.MapPath(_logDirectory + version + ".resources");
			if(appendToFile || !File.Exists(logFilePath))
			File.AppendAllText(logFilePath, DateTime.UtcNow.ToString(@"yyyy-MM-ddTHH\:mm\:ss.fffffffzzz"), Encoding.UTF8);
		}

		internal static bool IsUpgradeComplete(string version) {
			var logFilePath = HostingEnvironment.MapPath(_logDirectory + version + ".resources");
			return File.Exists(logFilePath);
		}

		private static FileStream upgradeFileHandle = null;
		private static StreamWriter upgradeFileStreamWriter = null;
		internal static bool IsUpgradeRunning
		{
			get
			{
				var lockFilePath = HostingEnvironment.MapPath(_logDirectory + "lock.resources");
				if (!File.Exists(lockFilePath))
					return false;

				FileStream stream = null;
				try
				{
					stream = new FileStream(lockFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
				}
				catch (IOException)
				{
					// The file is unavailable because it is:
					// - being processed by another thread
					// - does not exist (has already been processed)
					return true;
				}
				finally
				{
					if (stream != null)
						stream.Close();
				}

				return false;
			}
			private set
			{
				var lockFilePath = HostingEnvironment.MapPath(_logDirectory + "lock.resources");
				if (value)
				{
					upgradeFileHandle =  new FileStream(lockFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
					upgradeFileStreamWriter = new StreamWriter(upgradeFileHandle);
				}
				else
				{
					upgradeFileHandle.Close();
					var renamedLockFilePath =
						HostingEnvironment.MapPath(_logDirectory + DateTime.UtcNow.ToString(@"yyyy-MM-dd HH-mm-ss-fffffff") + ".log.resources");
					File.Move(lockFilePath, renamedLockFilePath);
				}
			}
		}

		private static void LogUpgradeStep(string message)
		{
			upgradeFileStreamWriter.WriteLine(DateTime.UtcNow.ToString(@"yyyy-MM-ddTHH\:mm\:ss") + " " + message);
			upgradeFileStreamWriter.Flush();
		}

		private static void LogUpgradeStep(string version, string message)
		{
			LogUpgradeStep(version + " - " + message);
		}

		/// <summary>
		/// While upgrading to 05.04.02, make sure the template folders get renamed to "2sxc"
		/// </summary>
		private static void Version050500()
		{
			var portalController = new PortalController();
			var portals = portalController.GetPortals();
			var pathsToCopy = portals.Cast<PortalInfo>().Select(p => p.HomeDirectoryMapPath).ToList();
			pathsToCopy.Add(HttpContext.Current.Server.MapPath("~/Portals/_default/"));
			foreach (var path in pathsToCopy)
			{
				var portalFolder = new DirectoryInfo(path);
				if (portalFolder.Exists)
				{
					var oldSexyFolder = new DirectoryInfo(Path.Combine(path, "2sexy"));
					var newSexyFolder = new DirectoryInfo(Path.Combine(path, "2sxc"));
					var newSexyContentFolder = new DirectoryInfo(Path.Combine(newSexyFolder.FullName, "Content"));
					if (oldSexyFolder.Exists && !newSexyFolder.Exists)
					{
						// Move 2sexy directory to 2scx/Content
						DirectoryCopy(oldSexyFolder.FullName, newSexyContentFolder.FullName, true);

						// Leave info message in the content folder
						File.WriteAllText(Path.Combine(oldSexyFolder.FullName, "__WARNING - old copy of files - READ ME.txt"), "This is a short information\r\n\r\n2sxc renamed the main folder from \"[Portal]/2Sexy\" to \"[Portal]/2sxc\" in version 5.5.\r\n\r\nTo make sure that links to images/css/js still work, the old folder was copied and this was left. Please clean up and delete the entire \"[Portal]/2Sexy/\" folder once you're done. \r\n\r\nMany thanks!\r\n2sxc\r\n\r\nPS: Remember that you might have ClientDependency activated, so maybe you still have bundled & minified  JS/CSS-Files in your cache pointing to the old location. When done cleaning up, we recommend increasing the version just to be sure you're not seeing an old files that don't exist any more. ");

						// Move web.config (should be directly in 2sxc)
						if (File.Exists(Path.Combine(newSexyContentFolder.FullName, "web.config")))
							File.Move(Path.Combine(newSexyContentFolder.FullName, "web.config"), Path.Combine(newSexyFolder.FullName, "web.config"));

					}
				}
			}
		}

		/// <summary>
		/// Add new Content Types for Pipeline Designer
		/// </summary>
		/// <remarks>Some Content Types are defined in EAV but some only in 2sxc. EAV.VersionUpgrade ensures Content Types are shared across all Apps.</remarks>
		private static void EnsurePipelineDesignerAttributeSets()
		{
			// Ensure DnnSqlDataSource Configuration
			var dsrcSqlDataSource = ImportAttributeSet.SystemAttributeSet("|Config ToSic.SexyContent.DataSources.DnnSqlDataSource", "used to configure a DNN SqlDataSource",
				new List<ImportAttribute>
				{
					ImportAttribute.StringAttribute("ContentType", "ContentType", null, true),
					ImportAttribute.StringAttribute("SelectCommand", "SelectCommand", null, true, rowCount: 10)
				});

			// Collect AttributeSets for use in Import
            var attributeSets = new List<ImportAttributeSet>
			{
				dsrcSqlDataSource
			};
			var import = new Eav.Import.Import(DataSource.DefaultZoneId, DataSource.MetaDataAppId, SexyContent.InternalUserName);
			import.RunImport(attributeSets, null);

			var metaDataCtx = EavContext.Instance(DataSource.DefaultZoneId, DataSource.MetaDataAppId);
			metaDataCtx.GetAttributeSet(dsrcSqlDataSource.StaticName).AlwaysShareConfiguration = true;
			metaDataCtx.SaveChanges();

			// Run EAV Version Upgrade (also ensures Content Type sharing)
			var eavVersionUpgrade = new VersionUpgrade(SexyContent.InternalUserName);
			eavVersionUpgrade.EnsurePipelineDesignerAttributeSets();
		}

		/// <summary>
		/// Add ContentTypes for ContentGroup and move all 2sxc data to EAV
		/// </summary>
		private static void Version070000()
		{
			var userName = "System-ModuleUpgrade-070000";

			#region 1. Import new ContentTypes for ContentGroups and Templates

			if (DataSource.GetCache(DataSource.DefaultZoneId, DataSource.MetaDataAppId).GetContentType("2SexyContent-Template") == null)
			{

				var xmlToImport =
					File.ReadAllText(HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/Upgrade/07.00.00.xml"));
				//var xmlToImport = File.ReadAllText("../../../../Upgrade/07.00.00.xml");
				var xmlImport = new XmlImport("en-US", userName, true);
				var success = xmlImport.ImportXml(DataSource.DefaultZoneId, DataSource.MetaDataAppId, XDocument.Parse(xmlToImport));

				if (!success)
				{
					var messages = String.Join("\r\n- ", xmlImport.ImportLog.Select(p => p.Message).ToArray());
					throw new Exception("The 2sxc module upgrade to 07.00.00 failed: " + messages);
				}
			}

			#endregion

			

			// 2. Move all existing data to the new ContentTypes - Append new IDs to old data (ensures that we can fix things that went wrong after upgrading the module)

			#region Prepare Templates

			var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString);
			var templates = new DataTable();
			const string sqlCommand = @"SELECT        ToSIC_SexyContent_Templates.TemplateID, ToSIC_SexyContent_Templates.PortalID, ToSIC_SexyContent_Templates.Name, ToSIC_SexyContent_Templates.Path, 
                         ToSIC_SexyContent_Templates.AttributeSetID, ToSIC_SexyContent_Templates.DemoEntityID, ToSIC_SexyContent_Templates.Script, 
                         ToSIC_SexyContent_Templates.IsFile, ToSIC_SexyContent_Templates.Type, ToSIC_SexyContent_Templates.IsHidden, ToSIC_SexyContent_Templates.Location, 
                         ToSIC_SexyContent_Templates.UseForList, ToSIC_SexyContent_Templates.UseForItem, ToSIC_SexyContent_Templates.SysCreated, 
                         ToSIC_SexyContent_Templates.SysCreatedBy, ToSIC_SexyContent_Templates.SysModified, ToSIC_SexyContent_Templates.SysModifiedBy, 
                         ToSIC_SexyContent_Templates.SysDeleted, ToSIC_SexyContent_Templates.SysDeletedBy, ToSIC_SexyContent_Templates.AppID, 
                         ToSIC_SexyContent_Templates.PublishData, ToSIC_SexyContent_Templates.StreamsToPublish, ToSIC_SexyContent_Templates.PipelineEntityID, 
                         ToSIC_SexyContent_Templates.ViewNameInUrl, ToSIC_SexyContent_Templates.Temp_PresentationTypeID, 
                         ToSIC_SexyContent_Templates.Temp_PresentationDemoEntityID, ToSIC_SexyContent_Templates.Temp_ListContentTypeID, 
                         ToSIC_SexyContent_Templates.Temp_ListContentDemoEntityID, ToSIC_SexyContent_Templates.Temp_ListPresentationTypeID, 
                         ToSIC_SexyContent_Templates.Temp_ListPresentationDemoEntityID, ToSIC_SexyContent_Templates.Temp_NewTemplateGuid, ToSIC_EAV_Apps.ZoneID, 
                         ToSIC_EAV_Entities_1.EntityGUID AS ContentDemoEntityGuid, ToSIC_EAV_Entities_2.EntityGUID AS PresentationDemoEntityGuid, 
                         ToSIC_EAV_Entities_3.EntityGUID AS ListContentDemoEntityGuid, ToSIC_EAV_Entities_4.EntityGUID AS ListPresentationDemoEntityGuid, 
                         ToSIC_EAV_Entities.EntityGUID AS PipelineEntityGuid
FROM            ToSIC_SexyContent_Templates INNER JOIN
                         ToSIC_EAV_Apps ON ToSIC_SexyContent_Templates.AppID = ToSIC_EAV_Apps.AppID LEFT OUTER JOIN
                         ToSIC_EAV_Entities ON ToSIC_SexyContent_Templates.PipelineEntityID = ToSIC_EAV_Entities.EntityID LEFT OUTER JOIN
                         ToSIC_EAV_Entities AS ToSIC_EAV_Entities_3 ON ToSIC_SexyContent_Templates.Temp_ListContentDemoEntityID = ToSIC_EAV_Entities_3.EntityID LEFT OUTER JOIN
                         ToSIC_EAV_Entities AS ToSIC_EAV_Entities_1 ON ToSIC_SexyContent_Templates.DemoEntityID = ToSIC_EAV_Entities_1.EntityID LEFT OUTER JOIN
                         ToSIC_EAV_Entities AS ToSIC_EAV_Entities_2 ON 
                         ToSIC_SexyContent_Templates.Temp_PresentationDemoEntityID = ToSIC_EAV_Entities_2.EntityID LEFT OUTER JOIN
                         ToSIC_EAV_Entities AS ToSIC_EAV_Entities_4 ON ToSIC_SexyContent_Templates.Temp_ListPresentationDemoEntityID = ToSIC_EAV_Entities_4.EntityID
WHERE        (ToSIC_SexyContent_Templates.SysDeleted IS NULL) AND ((SELECT COUNT(*) FROM ToSIC_EAV_Entities WHERE EntityGUID = ToSIC_SexyContent_Templates.Temp_NewTemplateGuid) = 0)";

			var adapter = new SqlDataAdapter(sqlCommand, sqlConnection);
			adapter.Fill(templates);

			var existingTemplates = templates.AsEnumerable().Select(t =>
			{
				var templateId = (int)t["TemplateID"];
				var zoneId = (int)t["ZoneID"];
				var appId = (int)t["AppID"];
				var cache = ((BaseCache)DataSource.GetCache(zoneId, appId)).GetContentTypes();

				#region Helper Functions
				Func<int?, string> getContentTypeStaticName = contentTypeId =>
				{
					if (!contentTypeId.HasValue || contentTypeId == 0)
						return "";
					if (cache.Any(c => c.Value.AttributeSetId == contentTypeId))
						return cache[contentTypeId.Value].StaticName;
					return "";
				};

				#endregion

				// Create anonymous object to validate the types
				var tempTemplate = new
				{
					TemplateID = templateId,
					Name = (string)t["Name"],
					Path = (string)t["Path"],
					NewEntityGuid = Guid.Parse((string)t["Temp_NewTemplateGuid"]),
					//AlreadyImported = t["Temp_NewTemplateGuid"] != DBNull.Value,

					ContentTypeId = getContentTypeStaticName(t["AttributeSetID"] == DBNull.Value ? new int?() : (int)t["AttributeSetID"]),
					ContentDemoEntityGuids = t["ContentDemoEntityGuid"] == DBNull.Value ? new List<Guid>() : new List<Guid> { (Guid)t["ContentDemoEntityGuid"] },
					PresentationTypeId = getContentTypeStaticName((int)t["Temp_PresentationTypeID"]),
					PresentationDemoEntityGuids = t["PresentationDemoEntityGuid"] == DBNull.Value ? new List<Guid>() : new List<Guid> { (Guid)t["PresentationDemoEntityGuid"] },
					ListContentTypeId = getContentTypeStaticName((int)t["Temp_ListContentTypeID"]),
					ListContentDemoEntityGuids = t["ListContentDemoEntityGuid"] == DBNull.Value ? new List<Guid>() : new List<Guid> { (Guid)t["ListContentDemoEntityGuid"] },
					ListPresentationTypeId = getContentTypeStaticName((int)t["Temp_ListPresentationTypeID"]),
					ListPresentationDemoEntityGuids = t["ListPresentationDemoEntityGuid"] == DBNull.Value ? new List<Guid>() : new List<Guid> { (Guid)t["ListPresentationDemoEntityGuid"] },

					Type = (string)t["Type"],
					IsHidden = (bool)t["IsHidden"],
					Location = (string)t["Location"],
					UseForList = (bool)t["UseForList"],
					AppId = appId,
					PublishData = (bool)t["PublishData"],
					StreamsToPublish = (string)t["StreamsToPublish"],
					PipelineEntityGuids = t["PipelineEntityGuid"] == DBNull.Value ? new List<Guid>() : new List<Guid> { (Guid)t["PipelineEntityGuid"] },
					ViewNameInUrl = t["ViewNameInUrl"].ToString(),
					ZoneId = zoneId
				};

				return tempTemplate;
			}).ToList();

			#endregion


			#region Prepare ContentGroups

			var contentGroupItemsTable = new DataTable();
			const string sqlCommandContentGroups = @"SELECT DISTINCT        ToSIC_SexyContent_ContentGroupItems.ContentGroupItemID, ToSIC_SexyContent_ContentGroupItems.ContentGroupID, 
                         ToSIC_SexyContent_ContentGroupItems.TemplateID, ToSIC_SexyContent_ContentGroupItems.SortOrder, ToSIC_SexyContent_ContentGroupItems.Type, 
                         ToSIC_SexyContent_ContentGroupItems.SysCreated, ToSIC_SexyContent_ContentGroupItems.SysCreatedBy, ToSIC_SexyContent_ContentGroupItems.SysModified, 
                         ToSIC_SexyContent_ContentGroupItems.SysModifiedBy, ToSIC_SexyContent_ContentGroupItems.SysDeleted, 
                         ToSIC_SexyContent_ContentGroupItems.SysDeletedBy, ToSIC_SexyContent_Templates.AppID, ToSIC_EAV_Apps.ZoneID, 
                         ToSIC_EAV_Entities.EntityGUID, ToSIC_SexyContent_ContentGroupItems.EntityID, ToSIC_SexyContent_ContentGroupItems.Temp_NewContentGroupGuid, ToSIC_SexyContent_Templates.Temp_NewTemplateGuid
FROM            ToSIC_SexyContent_Templates INNER JOIN
                         ModuleSettings INNER JOIN
                         ToSIC_SexyContent_ContentGroupItems ON ModuleSettings.SettingValue = ToSIC_SexyContent_ContentGroupItems.ContentGroupID ON 
                         ToSIC_SexyContent_Templates.TemplateID = ToSIC_SexyContent_ContentGroupItems.TemplateID INNER JOIN
                         ToSIC_EAV_Apps ON ToSIC_SexyContent_Templates.AppID = ToSIC_EAV_Apps.AppID LEFT OUTER JOIN
                         ToSIC_EAV_Entities ON ToSIC_SexyContent_ContentGroupItems.EntityID = ToSIC_EAV_Entities.EntityID
WHERE        (ToSIC_SexyContent_ContentGroupItems.SysDeleted IS NULL) AND (ModuleSettings.SettingName = N'ContentGroupID') AND 
                         ((SELECT COUNT(*) FROM ToSIC_EAV_Entities WHERE EntityGUID = ToSIC_SexyContent_ContentGroupItems.Temp_NewContentGroupGuid) = 0) ORDER BY SortOrder";

			var adapterContentGroups = new SqlDataAdapter(sqlCommandContentGroups, sqlConnection);
			adapterContentGroups.Fill(contentGroupItemsTable);

			var contentGroupItems = contentGroupItemsTable.AsEnumerable().Select(c => new
			{
				ContentGroupId = (int)c["ContentGroupID"],
				NewContentGroupGuid = Guid.Parse((string)c["Temp_NewContentGroupGuid"]),
				EntityId = c["EntityID"] == DBNull.Value ? new int?() : (int)c["EntityID"],
				EntityGuid = c["EntityGUID"] == DBNull.Value ? (Guid?)null : ((Guid)c["EntityGUID"]),
				TemplateId = c["TemplateID"] == DBNull.Value ? new int?() : (int)c["TemplateID"],
				SortOrder = (int)c["SortOrder"],
				Type = (string)c["Type"],
				AppId = (int)c["AppID"],
				ZoneId = (int)c["ZoneID"],
				TemplateEntityGuids = new List<Guid>() { Guid.Parse((string)c["Temp_NewTemplateGuid"]) }
			});

			var existingContentGroups = contentGroupItems.GroupBy(c => c.ContentGroupId, c => c, (id, items) =>
			{
				var itemsList = items.ToList();
				var contentGroup = new
				{
					NewEntityGuid = itemsList.First().NewContentGroupGuid,
					itemsList.First().AppId,
					itemsList.First().ZoneId,
					ContentGroupId = id,
					TemplateGuids = itemsList.First().TemplateEntityGuids,
					ContentGuids = itemsList.Where(p => p.Type == "Content").Select(p => p.EntityGuid).ToList(),
					PresentationGuids = itemsList.Where(p => p.Type == "Presentation").Select(p => p.EntityGuid).ToList(),
					ListContentGuids = itemsList.Where(p => p.Type == "ListContent").Select(p => p.EntityGuid).ToList(),
					ListPresentationGuids = itemsList.Where(p => p.Type == "ListPresentation").Select(p => p.EntityGuid).ToList()
				};
				return contentGroup;
			}).ToList();

			#endregion


			// Import all entities
			var apps = existingTemplates.Select(p => p.AppId).ToList();
			apps.AddRange(existingContentGroups.Select(p => p.AppId));
			apps = apps.Distinct().ToList();

			foreach (var app in apps)
			{
				LogUpgradeStep("07.00.00", "Starting to migrate data for app " + app + "...");

				var currentApp = app;
				var entitiesToImport = new List<ImportEntity>();

				foreach (var t in existingTemplates.Where(t => t.AppId == currentApp))
				{
                    var entity = new ImportEntity
					{
						AttributeSetStaticName = "2SexyContent-Template",
						EntityGuid = t.NewEntityGuid,
						IsPublished = true,
						AssignmentObjectTypeId = SexyContent.AssignmentObjectTypeIDDefault
					};
					entity.Values = new Dictionary<string, List<IValueImportModel>>
					{
						{"Name", new List<IValueImportModel> {new ValueImportModel<string>(entity) { Value = t.Name }}},
						{"Path", new List<IValueImportModel> {new ValueImportModel<string>(entity) { Value = t.Path }}},
						{"ContentTypeStaticName", new List<IValueImportModel> {new ValueImportModel<string>(entity) { Value = t.ContentTypeId }}},
						{"ContentDemoEntity", new List<IValueImportModel> {new ValueImportModel<List<Guid>>(entity) { Value = t.ContentDemoEntityGuids }}},
						{"PresentationTypeStaticName", new List<IValueImportModel> {new ValueImportModel<string>(entity) { Value = t.PresentationTypeId }}},
						{"PresentationDemoEntity", new List<IValueImportModel> {new ValueImportModel<List<Guid>>(entity) { Value = t.PresentationDemoEntityGuids }}},
						{"ListContentTypeStaticName", new List<IValueImportModel> {new ValueImportModel<string>(entity) { Value = t.ListContentTypeId }}},
						{"ListContentDemoEntity", new List<IValueImportModel> {new ValueImportModel<List<Guid>>(entity) { Value = t.ListContentDemoEntityGuids }}},
						{"ListPresentationTypeStaticName", new List<IValueImportModel> {new ValueImportModel<string>(entity) { Value = t.ListPresentationTypeId }}},
						{"ListPresentationDemoEntity", new List<IValueImportModel> {new ValueImportModel<List<Guid>>(entity) { Value = t.ListPresentationDemoEntityGuids }}},
						{"Type", new List<IValueImportModel> {new ValueImportModel<string>(entity) { Value = t.Type }}},
						{"IsHidden", new List<IValueImportModel> {new ValueImportModel<bool?>(entity) { Value = t.IsHidden }}},
						{"Location", new List<IValueImportModel> {new ValueImportModel<string>(entity) { Value = t.Location }}},
						{"UseForList", new List<IValueImportModel> {new ValueImportModel<bool?>(entity) { Value = t.UseForList }}},
						{"PublishData", new List<IValueImportModel> {new ValueImportModel<bool?>(entity) { Value = t.PublishData }}},
						{"StreamsToPublish", new List<IValueImportModel> {new ValueImportModel<string>(entity) { Value = t.StreamsToPublish }}},
						{"Pipeline", new List<IValueImportModel> {new ValueImportModel<List<Guid>>(entity) { Value = t.PipelineEntityGuids }}},
						{"ViewNameInUrl", new List<IValueImportModel> {new ValueImportModel<string>(entity) { Value = t.ViewNameInUrl }}}
					};
					entitiesToImport.Add(entity);

					//if (sqlConnection.State != ConnectionState.Open)
					//	sqlConnection.Open();
					//var sqlCmd = new SqlCommand("UPDATE ToSIC_SexyContent_Templates SET Temp_NewTemplateGuid = N'" + entity.EntityGuid + "' WHERE TemplateID = " + t.TemplateID, sqlConnection);
					//sqlCmd.ExecuteNonQuery();
				}

				foreach (var t in existingContentGroups.Where(t => t.AppId == app))
				{
                    var entity = new ImportEntity
					{
						AttributeSetStaticName = "2SexyContent-ContentGroup",
						EntityGuid = t.NewEntityGuid,
						IsPublished = true,
						AssignmentObjectTypeId = SexyContent.AssignmentObjectTypeIDDefault
					};
					entity.Values = new Dictionary<string, List<IValueImportModel>>
					{
						{"Template", new List<IValueImportModel> {new ValueImportModel<List<Guid>>(entity) { Value = t.TemplateGuids }}},
						{"Content", new List<IValueImportModel> {new ValueImportModel<List<Guid?>>(entity) { Value = t.ContentGuids }}},
						{"Presentation", new List<IValueImportModel> {new ValueImportModel<List<Guid?>>(entity) { Value = t.PresentationGuids }}},
						{"ListContent", new List<IValueImportModel> {new ValueImportModel<List<Guid?>>(entity) { Value = t.ListContentGuids }}},
						{"ListPresentation", new List<IValueImportModel> {new ValueImportModel<List<Guid?>>(entity) { Value = t.ListPresentationGuids }}}
					};
					entitiesToImport.Add(entity);

					//if (sqlConnection.State != ConnectionState.Open)
					//	sqlConnection.Open();
					//var sqlCmd = new SqlCommand("UPDATE ToSIC_SexyContent_ContentGroupItems SET Temp_NewContentGroupGuid = N'" + entity.EntityGuid + "' WHERE ContentGroupID = " + t.ContentGroupId, sqlConnection);
					//sqlCmd.ExecuteNonQuery();
				}

				var import = new Eav.Import.Import(null, app, userName);
				import.RunImport(null, entitiesToImport);

				LogUpgradeStep("07.00.00", "Migrated data for app " + app);
			}

			// 4. Use new GUID ContentGroup-IDs on module settings
//			if (sqlConnection.State != ConnectionState.Open)
//				sqlConnection.Open();
//			var sqlCmdUpdateModuleSettings = new SqlCommand(@"INSERT INTO ModuleSettings
//                         (ModuleID, CreatedByUserID, CreatedOnDate, LastModifiedByUserID, LastModifiedOnDate, SettingName, SettingValue)
//SELECT DISTINCT       ModuleSettings_1.ModuleID, ModuleSettings_1.CreatedByUserID, ModuleSettings_1.CreatedOnDate, ModuleSettings_1.LastModifiedByUserID, 
//                         ModuleSettings_1.LastModifiedOnDate, 'ToSIC_SexyContent_ContentGroupGuid' AS SettingName, 
//                         ToSIC_SexyContent_ContentGroupItems.Temp_NewContentGroupGuid AS SettingValue
//FROM            ModuleSettings AS ModuleSettings_1 LEFT OUTER JOIN
//                         ToSIC_SexyContent_ContentGroupItems ON ModuleSettings_1.SettingValue = ToSIC_SexyContent_ContentGroupItems.ContentGroupID
//WHERE        (ModuleSettings_1.SettingName = N'ContentGroupID') AND (NOT (ToSIC_SexyContent_ContentGroupItems.Temp_NewContentGroupGuid IS NULL)) AND
//                             ((SELECT        COUNT(*) AS Expr1
//                                 FROM            ModuleSettings AS ModuleSettings_2
//                                 WHERE        (ModuleID = ModuleSettings_1.ModuleID) AND (SettingName = N'ToSIC_SexyContent_ContentGroupGuid')) = 0)", sqlConnection);
//			sqlCmdUpdateModuleSettings.ExecuteNonQuery();
		}

		private static void Version070003()
		{
			var userName = "System-ModuleUpgrade-070003";

			// Import new ContentType for permissions
			if (DataSource.GetCache(DataSource.DefaultZoneId, DataSource.MetaDataAppId).GetContentType("PermissionConfiguration") == null)
			{

				var xmlToImport =
					File.ReadAllText(HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/Upgrade/07.00.03.xml"));
				//var xmlToImport = File.ReadAllText("../../../../Upgrade/07.00.00.xml");
				var xmlImport = new XmlImport("en-US", userName, true);
				var success = xmlImport.ImportXml(DataSource.DefaultZoneId, DataSource.MetaDataAppId, XDocument.Parse(xmlToImport));

				if (!success)
				{
					var messages = String.Join("\r\n- ", xmlImport.ImportLog.Select(p => p.Message).ToArray());
					throw new Exception("The 2sxc module upgrade to 07.00.03 failed: " + messages);
				}
			}

		}

		private static void Version070200()
		{
			var userName = "System-ModuleUpgrade-070200";

			// Import new ContentType for permissions
			if (DataSource.GetCache(DataSource.DefaultZoneId, DataSource.MetaDataAppId).GetContentType("|Config ToSic.Eav.DataSources.Paging") == null)
			{

				var xmlToImport =
					File.ReadAllText(HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/Upgrade/07.02.00.xml"));
				//var xmlToImport = File.ReadAllText("../../../../Upgrade/07.00.00.xml");
				var xmlImport = new XmlImport("en-US", userName, true);
				var success = xmlImport.ImportXml(DataSource.DefaultZoneId, DataSource.MetaDataAppId, XDocument.Parse(xmlToImport));

				if (!success)
				{
					var messages = String.Join("\r\n- ", xmlImport.ImportLog.Select(p => p.Message).ToArray());
					throw new Exception("The 2sxc module upgrade to 07.02.00 failed: " + messages);
				}
			}

		}

		/// <summary>
		/// Copy a Directory recursive
		/// </summary>
		/// <remarks>Source: http://msdn.microsoft.com/en-us/library/bb762914(v=vs.110).aspx </remarks>
		private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			// Get the subdirectories for the specified directory.
			var dir = new DirectoryInfo(sourceDirName);
			var dirs = dir.GetDirectories();

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
			}

			// If the destination directory doesn't exist, create it. 
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			// Get the files in the directory and copy them to the new location.
			var files = dir.GetFiles();
			foreach (var file in files)
			{
				var temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, false);
			}

			// If copying subdirectories, copy them and their contents to new location. 
			if (copySubDirs)
			{
				foreach (var subdir in dirs)
				{
					var temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
		}
	}
}