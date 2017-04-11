using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.ImportExport.Models;
using Microsoft.Practices.Unity;
using ToSic.Eav.ImportExport.Interfaces;

namespace ToSic.SexyContent.Installer
{
    public class V6: VersionBase
    {
        public V6(string version, Logger sharedLogger) : base(version, sharedLogger)  { }


        /// <summary>
        /// Add new Content Types for Pipeline Designer
        /// </summary>
        /// <remarks>Some Content Types are defined in EAV but some only in 2sxc. EAV.VersionUpgrade ensures Content Types are shared across all Apps.</remarks>
        internal void EnsurePipelineDesignerAttributeSets()
        {
            logger.LogStep("06.00.00", "EnsurePipelineDesignerAttributeSets start", false);

            // Ensure DnnSqlDataSource Configuration
            var dsrcSqlDataSource = ImpAttrSet.SystemAttributeSet("|Config ToSic.SexyContent.DataSources.DnnSqlDataSource", "used to configure a DNN SqlDataSource",
                new List<ImpAttribute>
                {
                    ImpAttribute.StringAttribute("ContentType", "ContentType", null, true),
                    ImpAttribute.StringAttribute("SelectCommand", "SelectCommand", null, true, rowCount: 10)
                }, alwaysShareConfiguration:true);

            // Collect AttributeSets for use in Import
            var attributeSets = new List<ImpAttrSet>
            {
                dsrcSqlDataSource
            };
            // 2017-04-11 2dm remove dependencies on BLL
            var importer = Factory.Container.Resolve<IRepositoryImporter>();
            importer.Import(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId, attributeSets, null);
            //var import = new DbImport(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId/*, Settings.InternalUserName*/);
            //import.ImportIntoDb(attributeSets, null);

            // 2017-04-01 2dm centralizing eav access
            //EavBridge.ForceShareOnGlobalContentType(dsrcSqlDataSource.StaticName);
            //var metaDataCtx = EavDataController.Instance(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId);
            //metaDataCtx.AttribSet.GetAttributeSet(dsrcSqlDataSource.StaticName).AlwaysShareConfiguration = true;
            //metaDataCtx.SqlDb.SaveChanges();

            // Run EAV Version Upgrade (also ensures Content Type sharing)
            var eavVersionUpgrade = new VersionUpgrade(Settings.InternalUserName);

            eavVersionUpgrade.EnsurePipelineDesignerAttributeSets();
            SystemManager.PurgeEverything();

            logger.LogStep("06.00.00", "EnsurePipelineDesignerAttributeSets done", false);
        }

    }
}