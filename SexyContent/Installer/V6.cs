using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.BLL;
using ToSic.Eav.Import;
using ToSic.SexyContent.Internal;

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
            var dsrcSqlDataSource = ImportAttributeSet.SystemAttributeSet("|Config ToSic.SexyContent.DataSources.DnnSqlDataSource", "used to configure a DNN SqlDataSource",
                new List<ImportAttribute>
                {
                    ImportAttribute.StringAttribute("ContentType", "ContentType", null, true),
                    ImportAttribute.StringAttribute("SelectCommand", "SelectCommand", null, true, rowCount: 10)
                }, alwaysShareConfiguration:true);

            // Collect AttributeSets for use in Import
            var attributeSets = new List<ImportAttributeSet>
            {
                dsrcSqlDataSource
            };
            var import = new Import(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId/*, Settings.InternalUserName*/);
            import.RunImport(attributeSets, null);

            // 2017-04-01 2dm centralizing eav access
            //EavBridge.ForceShareOnGlobalContentType(dsrcSqlDataSource.StaticName);
            //var metaDataCtx = EavDataController.Instance(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId);
            //metaDataCtx.AttribSet.GetAttributeSet(dsrcSqlDataSource.StaticName).AlwaysShareConfiguration = true;
            //metaDataCtx.SqlDb.SaveChanges();

            // Run EAV Version Upgrade (also ensures Content Type sharing)
            var eavVersionUpgrade = new VersionUpgrade(Settings.InternalUserName);
            eavVersionUpgrade.EnsurePipelineDesignerAttributeSets();

            logger.LogStep("06.00.00", "EnsurePipelineDesignerAttributeSets done", false);
        }

    }
}