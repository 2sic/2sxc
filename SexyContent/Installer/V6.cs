using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.BLL;
using ToSic.Eav.Import;

namespace ToSic.SexyContent.Installer
{
    public class V6
    {
        /// <summary>
        /// Add new Content Types for Pipeline Designer
        /// </summary>
        /// <remarks>Some Content Types are defined in EAV but some only in 2sxc. EAV.VersionUpgrade ensures Content Types are shared across all Apps.</remarks>
        internal void EnsurePipelineDesignerAttributeSets()
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
            var import = new Import(Constants.DefaultZoneId, Constants.MetaDataAppId, Settings.InternalUserName);
            import.RunImport(attributeSets, null);

            var metaDataCtx = EavDataController.Instance(Constants.DefaultZoneId, Constants.MetaDataAppId);
            metaDataCtx.AttribSet.GetAttributeSet(dsrcSqlDataSource.StaticName).AlwaysShareConfiguration = true;
            metaDataCtx.SqlDb.SaveChanges();

            // Run EAV Version Upgrade (also ensures Content Type sharing)
            var eavVersionUpgrade = new VersionUpgrade(Settings.InternalUserName);
            eavVersionUpgrade.EnsurePipelineDesignerAttributeSets();
        }

    }
}