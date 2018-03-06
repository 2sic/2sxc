using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Data.Builder;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging.Simple;

//using Microsoft.Practices.Unity;

namespace ToSic.SexyContent.Environment.Dnn7.Installation
{
    internal class V7: VersionBase
    {
        public V7(string version, Logger sharedLogger, Log parentLog) : base(version, sharedLogger, "UpgV7", parentLog)  { }
        
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
                var xmlImport = new XmlImportWithFiles(null, "en-US", /*userName,*/ true);
                var success = xmlImport.ImportXml(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId, XDocument.Parse(xmlToImport));

                if (!success)
                {
                    var messages = String.Join("\r\n- ", xmlImport.Messages.Select(p => p.Text).ToArray());
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
            var xmlImport = new XmlImportWithFiles(null, "en-US", /*userName,*/ true);
            var success = xmlImport.ImportXml(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId, XDocument.Parse(xmlToImport));

            if (!success)
            {
                var messages = String.Join("\r\n- ", xmlImport.Messages.Select(p => p.Text).ToArray());
                throw new Exception("The 2sxc module upgrade to 07.03.03-01 failed: " + messages);
            }

            // 2. Import ContentType-InputType and entities for it
            xmlToImport =
                File.ReadAllText(HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/Upgrade/07.03.03-02.xml"));
            xmlImport = new XmlImportWithFiles(null, "en-US", /*userName,*/ true);
            success = xmlImport.ImportXml(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId, XDocument.Parse(xmlToImport));

            if (!success)
            {
                var messages = String.Join("\r\n- ", xmlImport.Messages.Select(p => p.Text).ToArray());
                throw new Exception("The 2sxc module upgrade to 07.03.03-02 failed: " + messages);
            }

            // 3. Hide all unneeded fields - all fields for string, number: all but "Number of Decimals", Minimum and Maximum
            xmlToImport =
                File.ReadAllText(HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/Upgrade/07.03.03-03.xml"));
            xmlImport = new XmlImportWithFiles(null, "en-US", /*userName,*/ true);
            success = xmlImport.ImportXml(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId, XDocument.Parse(xmlToImport), false); // special note - change existing values

            if (!success)
            {
                var messages = String.Join("\r\n- ", xmlImport.Messages.Select(p => p.Text).ToArray());
                throw new Exception("The 2sxc module upgrade to 07.03.03-03 failed: " + messages);
            }

            logger.LogStep("07.03.03", "Done", false);


        }

        /// <summary>
        /// Add new Content Types for Pipeline Designer
        /// </summary>
        /// <remarks>Some Content Types are defined in EAV but some only in 2sxc. EAV.VersionUpgrade ensures Content Types are shared across all Apps.</remarks>
        internal void EnsurePipelineDesignerAttributeSets()
        {
            logger.LogStep("06.00.00", "EnsurePipelineDesignerAttributeSets start", false);

            // Ensure DnnSqlDataSource Configuration
            var dsrcSqlDataSource = ContentTypeBuilder.SystemAttributeSet(Eav.Constants.MetaDataAppId, "|Config ToSic.SexyContent.DataSources.DnnSqlDataSource", "used to configure a DNN SqlDataSource",
                new List<IAttributeDefinition>
                {
                    AttDefBuilder.StringAttribute(Eav.Constants.MetaDataAppId, "ContentType", "ContentType", null, true),
                    AttDefBuilder.StringAttribute(Eav.Constants.MetaDataAppId, "SelectCommand", "SelectCommand", null, true, rowCount: 10)
                }, alwaysShareConfiguration: true);

            // Collect AttributeSets for use in Import
            var attributeSets = new List<Eav.Data.ContentType>
            {
                dsrcSqlDataSource
            };

            var importer = Eav.Factory.Resolve<IRepositoryImporter>();
            importer.Import(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId, attributeSets, null);

            // Run EAV Version Upgrade (also ensures Content Type sharing)
            var eavVersionUpgrade = new VersionUpgrade(Settings.InternalUserName, Log);

            eavVersionUpgrade.EnsurePipelineDesignerAttributeSets();
            SystemManager.PurgeZoneList();

            logger.LogStep("06.00.00", "EnsurePipelineDesignerAttributeSets done", false);
        }

    }
}