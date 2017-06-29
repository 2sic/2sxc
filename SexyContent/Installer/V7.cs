using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources.Caches;
using ToSic.Eav.Interfaces;

//using Microsoft.Practices.Unity;

namespace ToSic.SexyContent.Installer
{
    internal class V7: VersionBase
    {
        public V7(string version, Logger sharedLogger) : base(version, sharedLogger)  { }

        /// <summary>
        /// Add ContentTypes for ContentGroup and move all 2sxc data to EAV
        /// </summary>
        internal void Version070000()
        {
            logger.LogStep("07.00.00", "Start", false);

            //var userName = "System-ModuleUpgrade-070000";

            #region 1. Import new ContentTypes for ContentGroups and Templates

            logger.LogStep("07.00.00", "1. Import new ContentTypes for ContentGroups and Templates", false);
            if (DataSource.GetCache(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId).GetContentType("2SexyContent-Template") == null)
            {

                var xmlToImport =
                    File.ReadAllText(HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/Upgrade/07.00.00.xml"));
                //var xmlToImport = File.ReadAllText("../../../../Upgrade/07.00.00.xml");
                var xmlImport = new XmlImportWithFiles("en-US", /*userName,*/ true);
                var success = xmlImport.ImportXml(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId, XDocument.Parse(xmlToImport));

                if (!success)
                {
                    var messages = String.Join("\r\n- ", xmlImport.ImportLog.Select(p => p.Message).ToArray());
                    throw new Exception("The 2sxc module upgrade to 07.00.00 failed: " + messages);
                }
            }

            #endregion



            // 2. Move all existing data to the new ContentTypes - Append new IDs to old data (ensures that we can fix things that went wrong after upgrading the module)

            #region Prepare Templates
            logger.LogStep("07.00.00", "2. Move all existing data to the new ContentTypes - Append new IDs to old data (ensures that we can fix things that went wrong after upgrading the module)", false);

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
            adapter.SelectCommand.CommandTimeout = 3600;
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
                    if (cache.Any(c => c.Value.ContentTypeId == contentTypeId))
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
            logger.LogStep("07.00.00", "2. Prepare Content Groups", false);

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
            adapterContentGroups.SelectCommand.CommandTimeout = 3600;
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
                    ContentGuids = itemsList.Where(p => p.Type == AppConstants.Content).Select(p => p.EntityGuid).ToList(),
                    PresentationGuids = itemsList.Where(p => p.Type == AppConstants.Presentation).Select(p => p.EntityGuid).ToList(),
                    ListContentGuids = itemsList.Where(p => p.Type == "ListContent").Select(p => p.EntityGuid).ToList(),
                    ListPresentationGuids = itemsList.Where(p => p.Type == "ListPresentation").Select(p => p.EntityGuid).ToList()
                };
                return contentGroup;
            }).ToList();

            #endregion


            // Import all entities
            logger.LogStep("07.00.00", "2. Import all entities", false);
            var apps = existingTemplates.Select(p => p.AppId).ToList();
            apps.AddRange(existingContentGroups.Select(p => p.AppId));
            apps = apps.Distinct().ToList();

            foreach (var appId in apps)
            {
                logger.LogStep("07.00.00", "Starting to migrate data for app " + appId + "...");

                var currentApp = appId;
                var entitiesToImport = new List<Entity>();

                foreach (var t in existingTemplates.Where(t => t.AppId == currentApp))
                {
                    // old, before 2017-06-12 using Entities
                    //var entity = new ImpEntity("2SexyContent-Template")
                    //{
                    //    //AttributeSetStaticName = "2SexyContent-Template",
                    //    EntityGuid = t.NewEntityGuid,
                    //    //auto: IsPublished = true
                    //    // , KeyTypeId = Constants.NotMetadata// Configuration.AssignmentObjectTypeIdDefault
                    //};
                    //entity.ImpAttributes = new Dictionary<string, List<IValue>>
                    //{
                    //    {"Name", new List<IValue> {new Value<string>(/*entity,*/ t.Name )}},
                    //    {"Path", new List<IValue> {new Value<string>(/*entity,*/ t.Path)}},
                    //    {"ContentTypeStaticName", new List<IValue> {new Value<string>(/*entity,*/ t.ContentTypeId)}},
                    //    {"ContentDemoEntity", new List<IValue> {new Value<List<Guid>>(/*entity,*/ t.ContentDemoEntityGuids)}},
                    //    {"PresentationTypeStaticName", new List<IValue> {new Value<string>(/*entity,*/ t.PresentationTypeId)}},
                    //    {"PresentationDemoEntity", new List<IValue> {new Value<List<Guid>>(/*entity,*/ t.PresentationDemoEntityGuids)}},
                    //    {"ListContentTypeStaticName", new List<IValue> {new Value<string>(/*entity,*/ t.ListContentTypeId)}},
                    //    {"ListContentDemoEntity", new List<IValue> {new Value<List<Guid>>(/*entity,*/ t.ListContentDemoEntityGuids)}},
                    //    {"ListPresentationTypeStaticName", new List<IValue> {new Value<string>(/*entity,*/ t.ListPresentationTypeId)}},
                    //    {"ListPresentationDemoEntity", new List<IValue> {new Value<List<Guid>>(/*entity,*/ t.ListPresentationDemoEntityGuids)}},
                    //    {"Type", new List<IValue> {new Value<string>(/*entity,*/ t.Type)}},
                    //    {"IsHidden", new List<IValue> {new Value<bool?>(/*entity,*/ t.IsHidden)}},
                    //    {"Location", new List<IValue> {new Value<string>(/*entity,*/ t.Location)}},
                    //    {"UseForList", new List<IValue> {new Value<bool?>(/*entity,*/ t.UseForList)}},
                    //    {"PublishData", new List<IValue> {new Value<bool?>(/*entity,*/ t.PublishData)}},
                    //    {"StreamsToPublish", new List<IValue> {new Value<string>(/*entity,*/ t.StreamsToPublish)}},
                    //    {"Pipeline", new List<IValue> {new Value<List<Guid>>(/*entity,*/ t.PipelineEntityGuids)}},
                    //    {"ViewNameInUrl", new List<IValue> {new Value<string>(/*entity,*/ t.ViewNameInUrl)}}
                    //};

                    // new, after 2017-06-12

                    var x = new Dictionary<string, object>
                    {
                        {"Name", t.Name },
                        {"Path",  t.Path},
                        {"ContentTypeStaticName", t.ContentTypeId},
                        {"ContentDemoEntity", t.ContentDemoEntityGuids},
                        {"PresentationTypeStaticName",  t.PresentationTypeId},
                        {"PresentationDemoEntity", t.PresentationDemoEntityGuids},
                        {"ListContentTypeStaticName", t.ListContentTypeId},
                        {"ListContentDemoEntity", t.ListContentDemoEntityGuids},
                        {"ListPresentationTypeStaticName", t.ListPresentationTypeId},
                        {"ListPresentationDemoEntity", t.ListPresentationDemoEntityGuids},
                        {"Type",  t.Type},
                        {"IsHidden", t.IsHidden},
                        {"Location", t.Location},
                        {"UseForList", t.UseForList},
                        {"PublishData", t.PublishData},
                        {"StreamsToPublish", t.StreamsToPublish},
                        {"Pipeline", t.PipelineEntityGuids},
                        {"ViewNameInUrl", t.ViewNameInUrl}
                    };
                    var entity = new Entity(appId, t.NewEntityGuid, "2SexyContent-Template", x);

                    entitiesToImport.Add(entity);
                }

                foreach (var t in existingContentGroups.Where(t => t.AppId == appId))
                {
                    // old, before 2017-06-12 using Entities
                    //var entity = new ImpEntity("2SexyContent-ContentGroup")
                    //{
                    //    //AttributeSetStaticName = "2SexyContent-ContentGroup",
                    //    EntityGuid = t.NewEntityGuid,
                    //    IsPublished = true
                    //    // , KeyTypeId = Constants.NotMetadata // Configuration.AssignmentObjectTypeIdDefault
                    //};
                    //entity.Attributes = new Dictionary<string, List<IValue>>
                    //{
                    //    {"Template", new List<IValue> {new Value<List<Guid>>(/*entity,*/ t.TemplateGuids)}},
                    //    {Constants.ContentKey, new List<IValue> {new Value<List<Guid?>>(/*entity,*/ t.ContentGuids)}},
                    //    {Constants.PresentationKey, new List<IValue> {new Value<List<Guid?>>(/*entity,*/ t.PresentationGuids)}},
                    //    {AppConstants.ListContent, new List<IValue> {new Value<List<Guid?>>(/*entity,*/ t.ListContentGuids)}},
                    //    {AppConstants.ListPresentation, new List<IValue> {new Value<List<Guid?>>(/*entity,*/ t.ListPresentationGuids)}}
                    //};
                    var x = new Dictionary<string, object>
                    {
                        {"Template",  t.TemplateGuids},
                        {AppConstants.Content, t.ContentGuids},
                        {AppConstants.Presentation, t.PresentationGuids},
                        {AppConstants.ListContent, t.ListContentGuids},
                        {AppConstants.ListPresentation, t.ListPresentationGuids}
                    };
                    var entity = new Entity(appId, t.NewEntityGuid, "2SexyContent-ContentGroup", x);

                    entitiesToImport.Add(entity);
                }

                var importer = Factory.Resolve<IRepositoryImporter>();
                importer.Import(null, appId, null, entitiesToImport);

                logger.LogStep("07.00.00", "Migrated data for app " + appId);
            }
            logger.LogStep("07.00.00", "Done", false);
        }

        internal  void Version070003()
        {
            logger.LogStep("07.00.03", "Start", false);

            //var userName = "System-ModuleUpgrade-070003";

            // Import new ContentType for permissions
            if (DataSource.GetCache(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId).GetContentType("PermissionConfiguration") == null)
            {

                var xmlToImport =
                    File.ReadAllText(HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/Upgrade/07.00.03.xml"));
                //var xmlToImport = File.ReadAllText("../../../../Upgrade/07.00.00.xml");
                var xmlImport = new XmlImportWithFiles("en-US", /*userName,*/ true);
                var success = xmlImport.ImportXml(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId, XDocument.Parse(xmlToImport));

                if (!success)
                {
                    var messages = String.Join("\r\n- ", xmlImport.ImportLog.Select(p => p.Message).ToArray());
                    throw new Exception("The 2sxc module upgrade to 07.00.03 failed: " + messages);
                }
            }
            logger.LogStep("07.00.03", "Done", false);


        }

        internal void Version070200()
        {
            logger.LogStep("07.02.00", "Start", false);

            //var userName = "System-ModuleUpgrade-070200";

            // Import new ContentType for permissions
            if (DataSource.GetCache(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId).GetContentType("|Config ToSic.Eav.DataSources.Paging") == null)
            {

                var xmlToImport =
                    File.ReadAllText(HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/Upgrade/07.02.00.xml"));
                //var xmlToImport = File.ReadAllText("../../../../Upgrade/07.00.00.xml");
                var xmlImport = new XmlImportWithFiles("en-US", /*userName,*/ true);
                var success = xmlImport.ImportXml(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId, XDocument.Parse(xmlToImport));

                if (!success)
                {
                    var messages = String.Join("\r\n- ", xmlImport.ImportLog.Select(p => p.Message).ToArray());
                    throw new Exception("The 2sxc module upgrade to 07.02.00 failed: " + messages);
                }
            }
            logger.LogStep("07.02.00", "Done", false);

        }

        internal void Version070303()
        {
            logger.LogStep("07.03.03", "Start", false);

            //var userName = "System-ModuleUpgrade-070303";

            // 1. Import new Attributes for @All content type
            var xmlToImport =
                File.ReadAllText(HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/Upgrade/07.03.03-01.xml"));
            var xmlImport = new XmlImportWithFiles("en-US", /*userName,*/ true);
            var success = xmlImport.ImportXml(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId, XDocument.Parse(xmlToImport));

            if (!success)
            {
                var messages = String.Join("\r\n- ", xmlImport.ImportLog.Select(p => p.Message).ToArray());
                throw new Exception("The 2sxc module upgrade to 07.03.03-01 failed: " + messages);
            }

            // 2. Import ContentType-InputType and entities for it
            xmlToImport =
                File.ReadAllText(HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/Upgrade/07.03.03-02.xml"));
            xmlImport = new XmlImportWithFiles("en-US", /*userName,*/ true);
            success = xmlImport.ImportXml(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId, XDocument.Parse(xmlToImport));

            if (!success)
            {
                var messages = String.Join("\r\n- ", xmlImport.ImportLog.Select(p => p.Message).ToArray());
                throw new Exception("The 2sxc module upgrade to 07.03.03-02 failed: " + messages);
            }

            // 3. Hide all unneeded fields - all fields for string, number: all but "Number of Decimals", Minimum and Maximum
            xmlToImport =
                File.ReadAllText(HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/Upgrade/07.03.03-03.xml"));
            xmlImport = new XmlImportWithFiles("en-US", /*userName,*/ true);
            success = xmlImport.ImportXml(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId, XDocument.Parse(xmlToImport), false); // special note - change existing values

            if (!success)
            {
                var messages = String.Join("\r\n- ", xmlImport.ImportLog.Select(p => p.Message).ToArray());
                throw new Exception("The 2sxc module upgrade to 07.03.03-03 failed: " + messages);
            }

            logger.LogStep("07.03.03", "Done", false);


        }

    }
}