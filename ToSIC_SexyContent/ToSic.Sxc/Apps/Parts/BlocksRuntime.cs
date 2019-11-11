using System;
using ToSic.Eav;
using ToSic.Eav.Data.Query;
using ToSic.Eav.DataSources;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Interfaces;

namespace ToSic.Sxc.Apps
{
    public class BlocksRuntime: CmsRuntimeBase
    {
        internal const string BlockTypeName = "2SexyContent-ContentGroup";

        internal BlocksRuntime(CmsRuntime cmsRuntime, ILog parentLog) : base(cmsRuntime, parentLog, "Sxc.BlkRdr")
        {
        }

        internal IDataSource ContentGroupSource()
        {
            var dataSource = DataSource.GetInitialDataSource(CmsRuntime.ZoneId, CmsRuntime.AppId, CmsRuntime.ShowDrafts);
            var onlyCGs = DataSource.GetDataSource<EntityTypeFilter>(CmsRuntime.ZoneId, CmsRuntime.AppId, dataSource);
            onlyCGs.TypeName = BlockTypeName;
            return dataSource;
        }

        public BlockConfiguration GetBlockConfig(Guid contentGroupGuid)
        {
            Log.Add($"get CG#{contentGroupGuid}");
            var dataSource = ContentGroupSource();
            // ToDo: Should use an indexed guid source
            var groupEntity = dataSource.List.One(contentGroupGuid);
            return groupEntity != null
                ? new BlockConfiguration(groupEntity, CmsRuntime.ZoneId, CmsRuntime.AppId, CmsRuntime.ShowDrafts , CmsRuntime.WithPublishing, Log)
                : new BlockConfiguration(Guid.Empty, CmsRuntime.ZoneId, CmsRuntime.AppId, CmsRuntime.ShowDrafts, CmsRuntime.WithPublishing, Log)
                {
                    DataIsMissing = true
                };
        }


        public BlockConfiguration GetInstanceContentGroup(int instanceId, int? pageId)
            => Factory.Resolve<IMapAppToInstance>().GetInstanceContentGroup(this, Log, instanceId, pageId);

        internal BlockConfiguration GetContentGroupOrGeneratePreview(Guid groupGuid, Guid previewTemplateGuid)
        {
            Log.Add($"get CG or gen preview for grp#{groupGuid}, preview#{previewTemplateGuid}");
            // Return a "faked" ContentGroup if it does not exist yet (with the preview templateId)
            return groupGuid == Guid.Empty
                ? new BlockConfiguration(previewTemplateGuid, CmsRuntime.ZoneId, CmsRuntime.AppId, CmsRuntime.ShowDrafts, CmsRuntime.WithPublishing, Log)
                : GetBlockConfig(groupGuid);
        }

    }
}
