using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Data.Builder;
using ToSic.Eav.Interfaces;

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
            var dsrcSqlDataSource = ContentTypeBuilder.SystemAttributeSet(Eav.Constants.MetaDataAppId, "|Config ToSic.SexyContent.DataSources.DnnSqlDataSource", "used to configure a DNN SqlDataSource",
                new List<IAttributeDefinition>
                {
                    AttDefBuilder.StringAttribute(Eav.Constants.MetaDataAppId, "ContentType", "ContentType", null, true),
                    AttDefBuilder.StringAttribute(Eav.Constants.MetaDataAppId, "SelectCommand", "SelectCommand", null, true, rowCount: 10)
                }, alwaysShareConfiguration:true);

            // Collect AttributeSets for use in Import
            var attributeSets = new List<Eav.Data.ContentType>
            {
                dsrcSqlDataSource
            };

            var importer = Factory.Resolve<IRepositoryImporter>();
            importer.Import(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId, attributeSets, null);

            // Run EAV Version Upgrade (also ensures Content Type sharing)
            var eavVersionUpgrade = new VersionUpgrade(Settings.InternalUserName);

            eavVersionUpgrade.EnsurePipelineDesignerAttributeSets();
            SystemManager.PurgeZoneList();

            logger.LogStep("06.00.00", "EnsurePipelineDesignerAttributeSets done", false);
        }

    }
}