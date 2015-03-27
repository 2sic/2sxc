using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Web.Client.ClientResourceManagement;
using ToSic.Eav;
using ToSic.SexyContent.ImportExport;
using System.Data;

namespace ToSic.SexyContent
{
	public class SexyContentModuleUpgrade
	{
		public static string UpgradeModule(string version)
		{
			switch (version)
			{
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
			}

			// Increase ClientDependency version upon each upgrade (System and all Portals)
			// prevents browsers caching old JS and CSS files for editing, which could cause several errors
			ClientResourceManager.UpdateVersion();

			return version;
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
			var dsrcSqlDataSource = Eav.Import.AttributeSet.SystemAttributeSet("|Config ToSic.SexyContent.DataSources.DnnSqlDataSource", "used to configure a DNN SqlDataSource",
				new List<Eav.Import.Attribute>
				{
					Eav.Import.Attribute.StringAttribute("ContentType", "ContentType", null, true),
					Eav.Import.Attribute.StringAttribute("SelectCommand", "SelectCommand", null, true, rowCount: 10)
				});

			// Collect AttributeSets for use in Import
			var attributeSets = new List<Eav.Import.AttributeSet>
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
			// 1. Import new ContentTypes for ContentGroups and Templates
			var xmlToImport= File.ReadAllText("~/DesktopModules/ToSIC_SexyContent/Upgrade/07.00.00.xml");
			var xmlImport = new XmlImport("en-US", "System-ModuleUpgrade-070000", true);
			var success = xmlImport.ImportXml(DataSource.DefaultZoneId, DataSource.MetaDataAppId, xmlToImport);

			if (!success)
			{
				var messages = String.Join("\r\n- ", xmlImport.ImportLog.Select(p => p.Message).ToArray());
				throw new Exception("The 2sxc module upgrade to 07.00.00 failed: " + messages);
			}

			// 2. Move all existing data to the new ContentTypes - Append new IDs to old data (ensures that we can fix things that went wrong after upgrading the module)
			var templates = new DataTable();
			const string sqlCommand = @"SELECT        ToSIC_SexyContent_Templates.TemplateID, ToSIC_SexyContent_Templates.PortalID, ToSIC_SexyContent_Templates.Name, ToSIC_SexyContent_Templates.Path, 
                         ToSIC_SexyContent_Templates.AttributeSetID, ToSIC_SexyContent_Templates.DemoEntityID, ToSIC_SexyContent_Templates.Type, 
                         ToSIC_SexyContent_Templates.IsHidden, ToSIC_SexyContent_Templates.Location, ToSIC_SexyContent_Templates.UseForList, 
                         ToSIC_SexyContent_Templates.AppID, ToSIC_SexyContent_Templates.PublishData, ToSIC_SexyContent_Templates.StreamsToPublish, 
                         ToSIC_SexyContent_Templates.PipelineEntityID, ToSIC_SexyContent_Templates.ViewNameInUrl, ToSIC_EAV_Apps.ZoneID
FROM            ToSIC_SexyContent_Templates INNER JOIN
                         ToSIC_EAV_Apps ON ToSIC_SexyContent_Templates.AppID = ToSIC_EAV_Apps.AppID
WHERE        (ToSIC_SexyContent_Templates.SysDeleted IS NULL)";

			var adapter = new SqlDataAdapter(sqlCommand, Config.GetConnectionString());
			adapter.Fill(templates);

			foreach (DataRow template in templates.Rows)
			{
				var templateId = (int) template["TemplateID"];
				var zoneId = (int) template["ZoneID"];
				var appId = (int) template["AppID"];

				var templateDefaults = DataSource.GetMetaDataSource(zoneId, appId).GetAssignedEntities(SexyContent.AssignmentObjectTypeIDSexyContentTemplate, templateId, SexyContent.AttributeSetStaticNameTemplateContentTypes).ToList();
				var presentationDefault = templateDefaults.FirstOrDefault(t => (string) t.Attributes["ItemType"][0] == "Presentation");
				var listContentDefault = templateDefaults.FirstOrDefault(t => (string) t.Attributes["ItemType"][0] == "ListContent");
				var listPresentationDefault = templateDefaults.FirstOrDefault(t => (string) t.Attributes["ItemType"][0] == "ListPresentation");


				var tempTemplate = new
				{
					TemplateID = templateId,
					Name = (string)template["Name"],
					Path = (string)template["Path"],
					
					ContentTypeId = (int?)template["AttributeSetID"],
					ContentDemoEntityId = (int?)template["DemoEntityID"],
					PresentationTypeId = presentationDefault != null ? presentationDefault.GetBestValue("ContentTypeID") : new int?(),
					PresentationDemoEntityId = presentationDefault != null ? presentationDefault.GetBestValue("DemoEntityID") : new int?(),

					//ListContentTypeId = listContentDefault != null && listContentDefault.Attributes.ContainsKey("ContentTypeID") && listContentDefault.Attributes["ContentTypeID"][0] != null && (decimal)listContentDefault.Attributes["ContentTypeID"][0] != 0 ? Convert.ToInt32((decimal)listContentDefault.Attributes["ContentTypeID"][0]) : new int?(),
					//ListContentDemoEntityId = listContentDefault != null && listContentDefault.Attributes.ContainsKey("DemoEntityID") && listContentDefault.Attributes["DemoEntityID"][0] != null && (decimal)listContentDefault.Attributes["DemoEntityID"][0] != 0 ? Convert.ToInt32((decimal)listContentDefault.Attributes["DemoEntityID"][0]) : new int?(),

					//ListPresentationTypeId = listPresentationDefault != null && listPresentationDefault.Attributes.ContainsKey("ContentTypeID") && listPresentationDefault.Attributes["ContentTypeID"][0] != null && (decimal)listPresentationDefault.Attributes["ContentTypeID"][0] != 0 ? Convert.ToInt32((decimal)listPresentationDefault.Attributes["ContentTypeID"][0]) : new int?(),
					//ListPresentationDemoEntityId = listPresentationDefault != null && listPresentationDefault.Attributes.ContainsKey("DemoEntityID") && listPresentationDefault.Attributes["DemoEntityID"][0] != null && (decimal)listPresentationDefault.Attributes["DemoEntityID"][0] != 0 ? Convert.ToInt32((decimal)listPresentationDefault.Attributes["DemoEntityID"][0]) : new int?(),
					
					Type = (string)template["Type"],
					IsHidden = (bool)template["IsHidden"],
					Location = (string)template["Location"],
					UseForList = (bool)template["UseForList"],
					AppId = appId,
					PublishData = (bool)template["PublishData"],
					StreamsToPublish = (string)template["StreamsToPublish"],
					PipelineEntityID = (int?)template["PipelineEntityID"],
					ViewNameInUrl = (string)template["ViewNameInUrl"],
					ZoneId = zoneId
				};

			}

			// 4. Use new GUID ContentGroup-IDs on module settings

		}

		/// <summary>
		/// Copy a Directory recursive
		/// </summary>
		/// <remarks>Source: http://msdn.microsoft.com/en-us/library/bb762914(v=vs.110).aspx </remarks>
		private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);
			DirectoryInfo[] dirs = dir.GetDirectories();

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
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, false);
			}

			// If copying subdirectories, copy them and their contents to new location. 
			if (copySubDirs)
			{
				foreach (DirectoryInfo subdir in dirs)
				{
					string temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
		}
	}
}